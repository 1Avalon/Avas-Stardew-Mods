using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Minigames;
using StardewValley;

namespace GiftMoney.UI
{

    public enum GiftType
    {
        Loved,
        Liked,
        Neutral
    }

    public class Button : ClickableTextureComponent
    {
        private Action Action;

        public string Label;

        public int width;

        public int height;

        public bool active;

        public Color textColor = Game1.textColor;

        private ClickableTextureComponent icon;

        public bool drawIcon = true;

        private Rectangle iconSourceRect;

        private int textAfterIconSpacing = 10;

        private int moneyRequired;

        private string textLabel;
        public Button(string label, Action action, GiftType type, int moneyRequired, bool isActive = true) : base(Rectangle.Empty, null, Rectangle.Empty, 1f)
        {

            switch (type)
            {
                case GiftType.Liked:
                    iconSourceRect = new Rectangle(0, 0, 13, 13);
                    break;

                case GiftType.Loved:
                    iconSourceRect = new Rectangle(13, 0, 13, 13);
                    break;

                case GiftType.Neutral:
                    iconSourceRect = new Rectangle(26, 0, 13, 13);
                    break;
            }

            Label = label;
            textLabel = label;
            Action = action;
            width = (int)Game1.dialogueFont.MeasureString(label).X + 64 + 13 * 2;
            height = 68;
            bounds = new Rectangle(0, 0, width, height);
            active = isActive;
            this.moneyRequired = moneyRequired;
            icon = new ClickableTextureComponent(new Rectangle(bounds.X, bounds.Y, 13 * 4, 13 * 4), ModEntry.icons, iconSourceRect, 4f);
        }
        public void SetPosition(int x, int y)
        {
            bounds.X = x; bounds.Y = y;
            icon.bounds = bounds;
            icon.bounds.X += 4;
            icon.bounds.Y += 8;
        }
        public void CallEvent()
        {
            if (active)
                Action();
        }

        public void changeLabel(string text, int spacing = 26)
        {
            if (spacing != 26)
                textAfterIconSpacing = 0;

            else
                textAfterIconSpacing = 10;

            this.Label = text;
            width = (int)Game1.dialogueFont.MeasureString(Label).X + 64 + spacing;
            height = 68;
            bounds = new Rectangle(bounds.X, bounds.Y, width, height);
        }

        public void PerformHover(int x, int y)
        {
            if (this.containsPoint(x, y))
            {
                drawIcon = false;
                changeLabel(moneyRequired.ToString(), 0);
                textColor = Color.White;
            }

            else
            {
                drawIcon = true;
                changeLabel(textLabel);
                textColor = Game1.textColor;
            }

        }

        public override void draw(SpriteBatch b)
        {
            if (!active)
                return;

            float draw_layer = 0.8f - (bounds.X + bounds.Y) * 1E-06f;
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(432, 439, 9, 9), bounds.X, bounds.Y, bounds.Width, bounds.Height, Color.White, 4f, drawShadow: true, draw_layer);
            Vector2 string_center = Game1.dialogueFont.MeasureString(Label) / 2f;
            string_center.X = (int)(string_center.X / 4f) * 4;
            string_center.Y = (int)(string_center.Y / 4f) * 4;

            if (drawIcon)
                icon.draw(b);

            Utility.drawTextWithShadow(b, Label, Game1.dialogueFont, new Vector2(bounds.Center.X + textAfterIconSpacing, bounds.Center.Y) - string_center, textColor, 1f, draw_layer + 1E-06f, -1, -1, 0f);
        }
    }
}