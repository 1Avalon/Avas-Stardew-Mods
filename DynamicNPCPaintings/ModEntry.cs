using System;
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

        public static Dictionary<string, string> PaintingsData = new Dictionary<string, string>();

        public static Dictionary<string, string> TextureData = new Dictionary<string, string>(); //key = unique name of texture ; value = path to the img

        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.Content.AssetRequested += OnAssetRequested;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            background = Helper.ModContent.Load<Texture2D>("assets/backgrounds/forest.png");
            Texture2D sunset = Helper.ModContent.Load<Texture2D>("assets/backgrounds/sunset.png");
            backgroundImages.Add("Forest", background);
            backgroundImages.Add("Sunset", sunset);
            backgroundImages.Add("Night Sky", Helper.ModContent.Load<Texture2D>("assets/backgrounds/night_sky.png"));
            backgroundImages.Add("Green Hill", Helper.ModContent.Load<Texture2D>("assets/backgrounds/Green_Hill.png"));
            frame = Helper.ModContent.Load<Texture2D>("assets/frames/frame1.png");
            instance = this;
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            PaintingsData.Clear();
            TextureData.Clear();
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            Monitor.Log($"Locale Name: {e.NameWithoutLocale.Name}");
            Monitor.Log("AvalonMFX.DynamicNPCPaintings.Picture_1_IMG");
            Monitor.Log((e.NameWithoutLocale.Name == "AvalonMFX.DynamicNPCPaintings.Picture_1_IMG").ToString());
            if (e.NameWithoutLocale.IsEquivalentTo(FRAME_KEY))
            {
                e.LoadFrom(() => new Dictionary<string, Frame>(), AssetLoadPriority.Exclusive);
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Furniture"))
            {
                e.Edit((IAssetData asset) =>
                {
                    var data = asset.AsDictionary<string, string>().Data;
                    foreach (var kvp in PaintingsData)
                    {
                        data.Add(kvp.Key, kvp.Value);
                    }
                    Monitor.Log(data.Count.ToString());
                });
            }

            else if (e.NameWithoutLocale.Name.StartsWith("AvalonMFX.DynamicNPCPaintings.Picture_"))
            {
                foreach(var kvp in TextureData)
                {
                    if (e.NameWithoutLocale.IsEquivalentTo(kvp.Key))
                    {
                        var file = TextureData[e.NameWithoutLocale.Name];
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