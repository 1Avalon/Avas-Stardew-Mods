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

        private int snapCounter = 0;

        public static bool enableSnap = false;

        public static Vector2 greenScreenRectangle = new Vector2(0, 0);

        public static Texture2D farmerSpriteSheet;

        public static ModEntry instance;

        public static Dictionary<string, Texture2D> maleWeddingOutfits = new Dictionary<string, Texture2D>();
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.Display.RenderedActiveMenu += this.OnRenderedActiveMenu;
            templateTexture = helper.ModContent.Load<Texture2D>("assets\\template.png");
            instance = this;

            foreach (string path in Directory.GetFiles(Path.Combine(Helper.DirectoryPath, "assets", "outfits"), "*.png"))
            {
                string fileName = Path.GetFileName(path);

                Texture2D tex = Helper.ModContent.Load<Texture2D>($"assets/outfits/{fileName}");
                maleWeddingOutfits.Add(fileName, tex);
                
            }

        }

        Vector2 farmerOffset = new Vector2(125, 125);
        private void OnRenderedActiveMenu(object sender, RenderedActiveMenuEventArgs e) 
        {
            if (Game1.activeClickableMenu is Menu && snapCounter < 30 && enableSnap)
            {
                Farmer farmer = Game1.player;

                if (greenScreenRectangle.X != 0)
                    e.SpriteBatch.Draw(Game1.staminaRect, new Rectangle((int)(125 / Game1.options.uiScale), (int)(125 / Game1.options.uiScale), 1408, (int)((greenScreenRectangle.Y + 5) / Game1.options.uiScale)), new Color(0, 255, 0));

                int maxWidth = (int)(1533 / Game1.options.uiScale);

                for (int i = 0; i < 126; i++)
                {
                    if (farmerOffset.X >= 1533 * Game1.options.uiScale)
                    {
                        farmerOffset.Y += 128 * Game1.options.uiScale;
                        farmerOffset.X = 125;
                    }

                    if (i == 101)
                    {
                        TextureHelper.RenderFarmer(e.SpriteBatch, farmer, i, new Vector2((int)((farmerOffset.X - 15) / Game1.options.uiScale), (int)(farmerOffset.Y / Game1.options.uiScale)));
                        farmerOffset.X += 64 * Game1.options.uiScale;
                        continue;
                    }

                    TextureHelper.RenderFarmer(e.SpriteBatch, farmer, i, new Vector2((int)(farmerOffset.X / Game1.options.uiScale), (int)(farmerOffset.Y / Game1.options.uiScale)));
                    farmerOffset.X += 64 * Game1.options.uiScale; //das ist für die screenshot area, also nicht anfassen
                }
                snapCounter++;
                greenScreenRectangle = new Vector2(farmerOffset.X * Game1.options.uiScale, farmerOffset.Y);
                farmerOffset = new Vector2(125, 125);
            }

        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (snapCounter == 15)
            {
                TextureHelper.InitFarmerSpriteSheet();
                snapCounter = 0;
                enableSnap = false;

                //Walking Front

                TextureHelper.CopyFrameToNewSpritesheet(farmerSpriteSheet, resultTexture, 0, 0);
                TextureHelper.CopyFrameToNewSpritesheet(farmerSpriteSheet, resultTexture, 2, 1);
                TextureHelper.CopyFrameToNewSpritesheet(farmerSpriteSheet, resultTexture, 0, 2);
                TextureHelper.CopyFrameToNewSpritesheet(farmerSpriteSheet, resultTexture, 1, 3);

                //Walking Right

                TextureHelper.CopyFrameToNewSpritesheet(farmerSpriteSheet, resultTexture, 6, 4);
                TextureHelper.CopyFrameToNewSpritesheet(farmerSpriteSheet, resultTexture, 7, 5);
                TextureHelper.CopyFrameToNewSpritesheet(farmerSpriteSheet, resultTexture, 6, 6);
                TextureHelper.CopyFrameToNewSpritesheet(farmerSpriteSheet, resultTexture, 8, 7);

                //Walking Back

                TextureHelper.CopyFrameToNewSpritesheet(farmerSpriteSheet, resultTexture, 12, 8);
                TextureHelper.CopyFrameToNewSpritesheet(farmerSpriteSheet, resultTexture, 14, 9);
                TextureHelper.CopyFrameToNewSpritesheet(farmerSpriteSheet, resultTexture, 12, 10);
                TextureHelper.CopyFrameToNewSpritesheet(farmerSpriteSheet, resultTexture, 13, 11);

                //Walking Left

                TextureHelper.CopyFrameToNewSpritesheet(farmerSpriteSheet, resultTexture, 6, 12, true);
                TextureHelper.CopyFrameToNewSpritesheet(farmerSpriteSheet, resultTexture, 7, 13, true);
                TextureHelper.CopyFrameToNewSpritesheet(farmerSpriteSheet, resultTexture, 6, 14, true);
                TextureHelper.CopyFrameToNewSpritesheet(farmerSpriteSheet, resultTexture, 8, 15, true);

                //Kiss
                TextureHelper.CopyFrameToNewSpritesheet(farmerSpriteSheet, resultTexture, 101, 28);
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