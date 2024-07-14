using System;
using System.IO.Enumeration;
using System.Security.Cryptography.X509Certificates;
using DynamicNPCPaintings.Framework;
using DynamicNPCPaintings.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

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

        public static SavedDataManager dataManager;

        public static IModHelper modHelper;

        public override void Entry(IModHelper helper)
        {
            modHelper = helper;
            dataManager = new();

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.Content.AssetRequested += OnAssetRequested;
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
            else if (e.NameWithoutLocale.IsEquivalentTo("DynamicNPCPaintings.Pictures"))
            {
                var file = Path.Combine(ModEntry.instance.Helper.DirectoryPath, "paintings", Constants.SaveFolderName, "Lol.png");
                string name = Path.GetFileNameWithoutExtension(file);
                var tex = Texture2D.FromFile(Game1.graphics.GraphicsDevice, file);
                e.LoadFrom(() => tex, AssetLoadPriority.Exclusive);
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
            frames = Helper.GameContent.Load<Dictionary<string, Frame>>(FRAME_KEY);
            Monitor.Log($"Found {frames.Count} frames");
        }
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
            if (e.Button == SButton.M)
                Game1.activeClickableMenu = new Customiser();
            // print button presses to the console window
        }
    }
}