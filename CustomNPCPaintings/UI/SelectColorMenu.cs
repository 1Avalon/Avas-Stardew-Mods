using DynamicNPCPaintings.UI;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace CustomNPCPaintings.UI
{
    public class SelectColorMenu : IClickableMenu
    {
        private Customiser customiser;

        private ColorPicker colorPicker;

        private ClickableTextureComponent preview;

        private Color backgroundColor;

        private Rectangle originalBounds;

        private ClickableTextureComponent randomButton;

        private ClickableTextureComponent okButton;

        private ClickableTextureComponent cancelButton;
        public SelectColorMenu(Customiser customiser)
        {
            int width = 960;
            int height = 720;
            this.customiser = customiser;
            Vector2 center = Utility.getTopLeftPositionForCenteringOnScreen(width, height);
            base.initialize((int)center.X, (int)center.Y, width, height);
            originalBounds = customiser.preview.bounds;
            preview = customiser.preview;
            preview.bounds.X = (int) (xPositionOnScreen + width / 2 - preview.bounds.Width * 3f);
            upperRightCloseButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 50, yPositionOnScreen + 69, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);
            colorPicker = new ColorPicker("Background", preview.bounds.X + preview.bounds.Width / 2, preview.bounds.Y + 200);

            if (customiser.picture.backgroundColor.A == 0)
                colorPicker.setColor(GetRandomColor());
            else
                colorPicker.setColor(customiser.picture.backgroundColor);

            float buttonScale = 6.4f;
            randomButton = new ClickableTextureComponent(new Rectangle(preview.bounds.X + 10, preview.bounds.Y + 280, (int)(10 * buttonScale), (int)(10 * buttonScale)), Game1.mouseCursors, new Rectangle(50, 428, 10, 10), buttonScale);

            cancelButton = new ClickableTextureComponent(new Rectangle(randomButton.bounds.X + 74, randomButton.bounds.Y, 64, 64), Game1.mouseCursors, new Rectangle(192, 256, 64, 64), 1);

            okButton = new ClickableTextureComponent(new Rectangle(cancelButton.bounds.X + 74, cancelButton.bounds.Y, 64, 64), Game1.mouseCursors, new Rectangle(128, 256, 64, 64), 1);

            customiser.picture.backgroundColor = colorPicker.getSelectedColor();
            customiser.UpdatePreview();
            exitFunction = () =>
            {
                customiser.preview.bounds = originalBounds;
                Game1.activeClickableMenu = customiser;
            };
        }

        private Color GetRandomColor()
        {
            Random r = Game1.random;
            return new Color(r.Next(0, 256), r.Next(0, 256), r.Next(0, 256));
        }
        public override void update(GameTime time)
        {
            base.update(time);
        }
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            if (colorPicker.containsPoint(x, y))
            {
                backgroundColor = colorPicker.click(x, y);
                customiser.picture.backgroundColor = backgroundColor;
                customiser.UpdatePreview();
            }
            else if (randomButton.containsPoint(x, y))
            {
                Color rndmColor = GetRandomColor();
                colorPicker.setColor(rndmColor);
                backgroundColor = rndmColor;
                customiser.picture.backgroundColor = backgroundColor;
                customiser.UpdatePreview();
            }
            else if (cancelButton.containsPoint(x, y))
            {
                customiser.picture.backgroundColor.A = 0;
                customiser.UpdatePreview();
                exitThisMenu();
            }
            else if (okButton.containsPoint(x, y))
            {
                exitThisMenu();
            }
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            if (randomButton.containsPoint(x, y))
            {
                randomButton.scale = 7.4f;
            }
            else
                randomButton.scale = 6.4f;

            if (okButton.containsPoint(x, y))
            {
                okButton.scale = 1.1f;
            }
            else
                okButton.scale = 1f;

            if (cancelButton.containsPoint(x, y))
            {
                cancelButton.scale = 1.1f;
            }
            else
                cancelButton.scale = 1f;
        }

        public override void leftClickHeld(int x, int y)
        {
            backgroundColor = colorPicker.clickHeld(x, y);
            if (colorPicker.containsPoint(x, y))
            {
                customiser.picture.backgroundColor = backgroundColor;
                customiser.UpdatePreview();
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            colorPicker.releaseClick();
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
            upperRightCloseButton.draw(b);
            preview.draw(b);
            colorPicker.draw(b);
            randomButton.draw(b);
            okButton.draw(b);
            cancelButton.draw(b);
            drawMouse(b);
        }
    }
}
