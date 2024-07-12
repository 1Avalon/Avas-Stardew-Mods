using DynamicNPCPaintings.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicNPCPaintings.UI
{
    internal class SelectFrameMenu : IClickableMenu
    {
        private string hoverText = "";

        public Dictionary<string, Frame> frames = new Dictionary<string, Frame>();

        public List<ClickableTextureComponent> components = new List<ClickableTextureComponent>();

        private Customiser customiser;
        public SelectFrameMenu(Customiser customiser) 
        {
            int width = 960;
            int height = 720;
            this.customiser = customiser;
            base.initialize(Game1.viewport.Width / 2 - width / 2, Game1.viewport.Height / 2 - height / 2, width, height);

            int startPositionX = xPositionOnScreen + 50;
            int startPositionY = yPositionOnScreen + 110;
            int frameScale = 4;

            frames = ModEntry.instance.Helper.GameContent.Load<Dictionary<string, Frame>>(ModEntry.FRAME_KEY);

            foreach (var kvp in frames)
            {
                int bgWidth = kvp.Value.frameTexture.Width;
                int bgHeight = kvp.Value.frameTexture.Height;
                ClickableTextureComponent component = new ClickableTextureComponent(new Rectangle(startPositionX, startPositionY, bgWidth * frameScale, bgHeight * frameScale), kvp.Value.frameTexture, new Rectangle(0, 0, kvp.Value.frameTexture.Width, kvp.Value.frameTexture.Height), frameScale);
                component.name = kvp.Key;
                components.Add(component);
                startPositionX += bgWidth * frameScale + 10;
                if (startPositionX > xPositionOnScreen + this.width - 16 * frameScale)
                {
                    startPositionX = xPositionOnScreen + 30;
                    startPositionY += 32 * frameScale + 5;
                }
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            foreach (ClickableTextureComponent component in components)
            {
                if (component.containsPoint(x, y))
                {
                    customiser.picture.frame = frames[component.name];
                    customiser.picture.npcOffsetX = frames[component.name].spaceWidth / 2 - 4;
                    customiser.UpdatePreview();
                    Game1.activeClickableMenu = customiser;
                }
            }
        }
        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
            foreach (ClickableTextureComponent component in components)
                component.draw(b);

            drawHoverText(b, hoverText, Game1.smallFont);
            drawMouse(b);
        }
    }
}
