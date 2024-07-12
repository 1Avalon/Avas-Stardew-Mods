using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicNPCPaintings.UI
{
    public class SelectBackgroundUI : IClickableMenu
    {
        private Customiser customiser;

        public List<ClickableTextureComponent> validBackgrounds = new List<ClickableTextureComponent>();

        private string hoverText = "";
        public SelectBackgroundUI(Customiser customiser) 
        {
            int width = 960;
            int height = 720;
            this.customiser = customiser;
            base.initialize(Game1.viewport.Width / 2 - width / 2, Game1.viewport.Height / 2 - height / 2, width, height);

            int startPositionX = xPositionOnScreen + 50;
            int startPositionY = yPositionOnScreen + 110;
            int backgroundScale = 4;

            foreach (var kvp in ModEntry.backgroundImages)
            {
                ClickableTextureComponent component = new ClickableTextureComponent(new Rectangle(startPositionX, startPositionY, 32 * backgroundScale, 48 * backgroundScale), kvp.Value, new Rectangle(0, 0, kvp.Value.Width, kvp.Value.Height), backgroundScale);
                component.name = kvp.Key;
                if (kvp.Value.Width >= customiser.picture.frame.spaceWidth && kvp.Value.Height >= customiser.picture.frame.spaceHeight) 
                    validBackgrounds.Add(component);
                startPositionX += 48 * backgroundScale + 10;
                if (startPositionX > xPositionOnScreen + this.width - 16 * backgroundScale)
                {
                    startPositionX = xPositionOnScreen + 30;
                    startPositionY += 32 * backgroundScale + 5;
                }
            }
            exitFunction = () => { Game1.activeClickableMenu = customiser; };
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            foreach(ClickableTextureComponent component in validBackgrounds)
            {
                if (component.containsPoint(x, y))
                {
                    customiser.picture.background = new Framework.Background(0, 0, component.texture);
                    customiser.UpdatePreview();
                    Game1.activeClickableMenu = customiser;
                }
            }
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            hoverText = "";
            foreach (ClickableTextureComponent component in validBackgrounds)
            {
                if (component.containsPoint(x, y))
                {
                    hoverText = component.name;
                }
            }
        }
        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
            foreach (ClickableTextureComponent component in validBackgrounds)
                component.draw(b);

            drawHoverText(b, hoverText, Game1.smallFont);
            drawMouse(b);
        }
    }
}
