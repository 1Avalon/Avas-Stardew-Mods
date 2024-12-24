using StardewValley.Menus;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using Microsoft.Xna.Framework.Graphics;

namespace DynamicNPCPaintings.UI.UIElements
{
    public class Checkbox : ClickableTextureComponent
    {
        public Checkbox(string name, Rectangle bounds, string label, bool value = false)
            : base(name, bounds, label, "", Game1.mouseCursors, new Rectangle(227, 425, 9, 9), 4f)
        {
        }

        public void click(int x, int y, ref bool value)
        {
            if (containsPoint(x, y))
            {
                value = !value;
                //sourceRect.X = sourceRect.X == 227 ? 236 : 227;
            }
            sourceRect.X = value == true ? 236 : 227;
        }
        public override void draw(SpriteBatch b)
        {
            b.Draw(texture, bounds, sourceRect, Color.White);
            Utility.drawTextWithShadow(b, label, Game1.smallFont, new Vector2(bounds.X + bounds.Width + 8, bounds.Y + 8), Game1.textColor, 1);
        }
    }
}
