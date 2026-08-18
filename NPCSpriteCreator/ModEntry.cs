using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace NPCSpriteCreator
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        /// 

        public static Texture2D templateTexture;

        public static Texture2D resultTexture = new Texture2D(Game1.graphics.GraphicsDevice, 16 * 4, 32 * 13);

        public static Texture2D farmerSpriteSheet;

        public static ModEntry instance;

        public static Dictionary<string, Texture2D> maleWeddingOutfits = new Dictionary<string, Texture2D>();
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            templateTexture = helper.ModContent.Load<Texture2D>("assets\\template.png");
            instance = this;

            foreach (string path in Directory.GetFiles(Path.Combine(Helper.DirectoryPath, "assets", "outfits"), "*.png"))
            {
                string fileName = Path.GetFileName(path);

                Texture2D tex = Helper.ModContent.Load<Texture2D>($"assets/outfits/{fileName}");
                maleWeddingOutfits.Add(fileName, tex);
                
            }

        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if (e.Button == SButton.K)
                Game1.activeClickableMenu = new Menu();
        }
    }
}