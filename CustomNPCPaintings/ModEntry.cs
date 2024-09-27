using System;
using System.IO.Enumeration;
using System.Security.Cryptography.X509Certificates;
using CustomNPCPaintings;
using DynamicNPCPaintings.Framework;
using DynamicNPCPaintings.UI;
using DynamicNPCPaintings.UI.UIElements;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.Inventories;
using StardewValley.Menus;
using StardewValley.Objects;
using Background = DynamicNPCPaintings.Framework.Background;

namespace DynamicNPCPaintings
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        /// 
        public static Texture2D frame;

        public static Texture2D background;

        public static ModEntry instance;

        public static Dictionary<string, Background> backgroundImages = new Dictionary<string, Background>();

        public static Dictionary<string, Frame> frames = new Dictionary<string, Frame>();

        public static readonly string FRAME_KEY = "AvalonMFX.CustomNPCPaintings/Frames";

        public static readonly string BACKGROUND_KEY = "AvalonMFX.CustomNPCPaintings/Backgrounds";

        public static SavedDataManager dataManager;

        public static IModHelper modHelper;

        private Button button;

        private Dictionary<string, string> translatedBackgroundImageNames = new Dictionary<string, string>();

        public static bool hasSeasonalCuteSprites = false;

        public static bool hasHappyHomeDesigner = false;

        public static ModConfig Config;
        public override void Entry(IModHelper helper)
        {
            modHelper = helper;
            dataManager = SavedDataManager.Create();

            I18n.Init(helper.Translation);
            Config = Helper.ReadConfig<ModConfig>();

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.Content.AssetRequested += OnAssetRequested;
            helper.Events.Display.Rendered += OnRendered;
            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.Player.Warped += OnWarped;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            /*
            background = Helper.ModContent.Load<Texture2D>("assets/backgrounds/forest.png");
            Texture2D sunset = Helper.ModContent.Load<Texture2D>("assets/backgrounds/sunset.png");
            backgroundImages.Add("Forest", background);
            backgroundImages.Add("Sunset", sunset);
            backgroundImages.Add("Night Sky", Helper.ModContent.Load<Texture2D>("assets/backgrounds/night_sky.png"));
            backgroundImages.Add("Green Hill", Helper.ModContent.Load<Texture2D>("assets/backgrounds/Green_Hill.png"));
            backgroundImages.Add("Blue Night Sky", Helper.ModContent.Load<Texture2D>("assets/backgrounds/blue_night_sky.png"));
            backgroundImages.Add("Castle", Helper.ModContent.Load<Texture2D>("assets/backgrounds/castle.png"));
            */

            foreach (IModInfo mod in this.Helper.ModRegistry.GetAll()) 
            {
                if (mod.Manifest.UniqueID == "Poltergeister.SeasonalCuteCharacters")
                {
                    hasSeasonalCuteSprites = true;
                    Monitor.Log("Found Seasonal Cute Characters mod by Poltergeister. Adding Paintings button to festival shops...");
                }
                else if (mod.Manifest.UniqueID == "tlitookilakin.HappyHomeDesigner")
                {
                    Monitor.Log("Found Happy Home Designer mod by tlitookilakin. Adding warning message...");
                    hasHappyHomeDesigner = true;
                }
            }

            frame = Helper.ModContent.Load<Texture2D>("assets/frames/frame1.png");
            instance = this;
        }
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            button = new Button(I18n.Menu_NPCPaintings(), delegate
            {
                Game1.playSound("dwop");
                Game1.activeClickableMenu = new Customiser();
            });
            button.active = false;

            translatedBackgroundImageNames.Clear();
            backgroundImages.Clear();
                foreach (var translation in Helper.Translation.GetTranslations())
            {
                translatedBackgroundImageNames.Add(translation.Key, translation.ToString());
            }

            foreach (string path in Directory.GetFiles(Path.Combine(Helper.DirectoryPath, "assets", "backgrounds"), "*.png"))
            {
                string fileName = Path.GetFileName(path);
                Monitor.Log($"Found Background {fileName}");

                string translatioNKey = "DefaultImg." + fileName.Replace(".png", "");
                Texture2D tex = Helper.ModContent.Load<Texture2D>($"assets/backgrounds/{fileName}");

                if (translatedBackgroundImageNames.TryGetValue(translatioNKey, out string translation))
                    backgroundImages.Add(translatioNKey, Background.Of(translation, 0, 0, tex));
                else
                    backgroundImages.Add(fileName, Background.Of(fileName.Replace(".png", ""), 0, 0, tex));
            }
        }
        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo(FRAME_KEY))
            {
                e.LoadFrom(() => new Dictionary<string, Frame>(), AssetLoadPriority.Exclusive);
            }
            else if (e.NameWithoutLocale.IsEquivalentTo(BACKGROUND_KEY))
            {
                e.LoadFrom(() => new Dictionary<string, Framework.Background>(), AssetLoadPriority.Exclusive);
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Furniture"))
            {
                e.Edit((IAssetData asset) =>
                {
                    var data = asset.AsDictionary<string, string>().Data;
                    foreach (var kvp in dataManager.FurnitureData)
                    {
                        if (data.ContainsKey(kvp.Key))
                            data.Remove(kvp.Key);

                        data.Add(kvp.Key, kvp.Value);
                    }
                    Monitor.Log(data.Count.ToString());
                });
            }

            else if (e.NameWithoutLocale.Name.StartsWith($"{Constants.SaveFolderName}.AvalonMFX.CustomNPCPaintings.Picture_") || e.NameWithoutLocale.Name.StartsWith($"{dataManager.SaveFolderName}.AvalonMFX.CustomNPCPaintings.Picture_"))
            {
                foreach(var kvp in dataManager.PictureData)
                {
                    if (e.NameWithoutLocale.IsEquivalentTo(kvp.Key))
                    {

                        if (Context.IsMultiplayer && !Context.IsMainPlayer)
                        {
                            Texture2D dataTexture = dataManager.PictureData[e.NameWithoutLocale.Name].GetTexture();
                            e.LoadFrom(() => dataTexture, AssetLoadPriority.Exclusive);
                            break;
                        }

                        var tex = dataManager.PictureData[e.NameWithoutLocale.Name].GetTexture();
                        if (tex == null)
                        {
                            var file = dataManager.TextureData[e.NameWithoutLocale.Name];
                            tex = Texture2D.FromFile(Game1.graphics.GraphicsDevice, file);
                        }

                        e.LoadFrom(() => tex, AssetLoadPriority.Exclusive);
                        break;
                    }
                }
            }
        }
        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        /// 
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: this.ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(Config)
            );

            configMenu.AddKeybind(
                mod: this.ModManifest,
                name: () => I18n.Config_OpenCustomiser(),
                tooltip: () => I18n.Config_OpenCustomiserDescription(),
                getValue: () => Config.openCustomizerButton,
                setValue: value => Config.openCustomizerButton = value
                );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => I18n.Config_EnableAllNPCs(),
                tooltip: () => I18n.Config_EnableAllNPCsDescription(),
                getValue: () => Config.enableAllNPCs,
                setValue: value => Config.enableAllNPCs = value
                );
        }
        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (!hasHappyHomeDesigner || Config.openCustomizerButton != SButton.None)
                return;

            string msg = I18n.Misc_WarningMessageHappyHomeDesigner();
            Game1.activeClickableMenu = new DialogueBox(msg);
        }
        private void OnRendered(object sender, RenderedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (button != null && button.active)
            {
                if (button.bounds.Contains(Game1.getMouseX(), Game1.getMouseY()))
                    button.textColor = Color.White;
                else
                    button.textColor = Game1.textColor;

                    button.draw(e.SpriteBatch);
                e.SpriteBatch.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 0, 16, 16), Color.White, 0f, Vector2.Zero, 4f + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
            }

        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Game1.activeClickableMenu == null && e.Button == Config.openCustomizerButton)
            {
                foreach (Furniture f in Game1.currentLocation.furniture)
                {
                    if (f.QualifiedItemId == "(F)1308")
                    {
                        Game1.activeClickableMenu = new Customiser();
                        break;
                    }
                }
            }

            if (button != null && button.active)
            {
                if (button.bounds.Contains(Game1.getMouseX(), Game1.getMouseY()))
                    button.CallEvent();
                    
            }
        }
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is ShopMenu menu && menu.ShopId == "Catalogue" || (Game1.isFestival() && e.NewMenu is ShopMenu && hasSeasonalCuteSprites))
            {
                menu = (ShopMenu)e.NewMenu;
                button.active = true;
                button.setPosition(menu.inventory.xPositionOnScreen - button.width - 30, menu.yPositionOnScreen + menu.height - menu.inventory.height - 12 + 80);
            }
            else if (e.OldMenu is ShopMenu oldMenu && oldMenu.ShopId == "Catalogue" || Game1.isFestival())
            {
                button.active = false;
            }
        }
    }
}