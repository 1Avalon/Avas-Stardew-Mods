using System;
using System.IO.Enumeration;
using System.Security.Cryptography.X509Certificates;
using DynamicNPCPaintings.Framework;
using DynamicNPCPaintings.UI;
using DynamicNPCPaintings.UI.UIElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Inventories;
using StardewValley.Menus;

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

        public static Dictionary<string, Texture2D> backgroundImages = new Dictionary<string, Texture2D>();

        public static Dictionary<string, Frame> frames = new Dictionary<string, Frame>();

        public static readonly string FRAME_KEY = "AvaloNMFX.DynamicNPCPaintings/Frames";

        public static readonly string BACKGROUND_KEY = "AvalonMFX.DynamicNPCPaintings/Backgrounds";

        public static SavedDataManager dataManager;

        public static IModHelper modHelper;

        private Button button;

        public override void Entry(IModHelper helper)
        {
            modHelper = helper;
            dataManager = new();

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.Content.AssetRequested += OnAssetRequested;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Display.Rendered += OnRendered;
            helper.Events.Display.MenuChanged += OnMenuChanged;
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
            foreach (string path in Directory.GetFiles(Path.Combine(Helper.DirectoryPath, "assets", "backgrounds"), "*.png"))
            {
                string fileName = Path.GetFileName(path);
                Monitor.Log($"Found Background {fileName}");
                backgroundImages.Add(fileName.Replace(".png", ""), Helper.ModContent.Load<Texture2D>($"assets/backgrounds/{fileName}"));
            }

            frame = Helper.ModContent.Load<Texture2D>("assets/frames/frame1.png");
            instance = this;
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
                        data.Add(kvp.Key, kvp.Value);
                    }
                    Monitor.Log(data.Count.ToString());
                });
            }

            else if (e.NameWithoutLocale.Name.StartsWith("AvalonMFX.DynamicNPCPaintings.Picture_"))
            {
                foreach(var kvp in dataManager.TextureData)
                {
                    if (e.NameWithoutLocale.IsEquivalentTo(kvp.Key))
                    {
                        var file = dataManager.TextureData[e.NameWithoutLocale.Name];
                        var tex = Texture2D.FromFile(Game1.graphics.GraphicsDevice, file);
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
        private void OnRendered(object sender, RenderedEventArgs e)
        {
            if (Game1.activeClickableMenu is ShopMenu menu && menu.ShopId == "Catalogue")
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
            if (Game1.activeClickableMenu is ShopMenu menu && menu.ShopId == "Catalogue")
            {
                if (button.bounds.Contains(Game1.getMouseX(), Game1.getMouseY()))
                    button.CallEvent();
                    
            }
        }
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is ShopMenu menu && menu.ShopId == "Catalogue")
            {
                button.active = true;
                button.setPosition(menu.inventory.xPositionOnScreen - button.width - 30, menu.yPositionOnScreen + menu.height - menu.inventory.height - 12 + 80);
            }
            else if (e.OldMenu is ShopMenu oldMenu && oldMenu.ShopId == "Catalogue")
            {
                button.active = false;
            }
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            button = new Button("NPC Paintings", delegate
            {
                Game1.playSound("dwop");
                Game1.activeClickableMenu = new Customiser();
            });
            button.active = false;

            frames = Helper.GameContent.Load<Dictionary<string, Frame>>(FRAME_KEY);
            Monitor.Log($"Found {frames.Count} frames");
        }
    }
}