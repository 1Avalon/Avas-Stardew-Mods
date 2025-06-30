using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPCSpriteCreator
{
    public class Menu : IClickableMenu
    {
        private const int width = 960;

        private const int height = 720;

        public Menu()
        {
            Vector2 center = Utility.getTopLeftPositionForCenteringOnScreen(width, height);
            base.initialize((int)center.X, (int)center.Y, width, height);
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
            drawMouse(b);
        }
    }
}
