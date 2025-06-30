﻿using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Minigames;
using StardewValley;

namespace NPCSpriteCreator.UIElements
{
    public class Button : ClickableTextureComponent
    {
        private Action Action;

        public string Label;

        public int width;

        public int height;

        public bool active;

        public Color textColor = Game1.textColor;
        public Button(string label, Action action, bool isActive = true) : base(Rectangle.Empty, null, Rectangle.Empty, 1f)
        {
            Label = label;
            Action = action;
            width = (int)Game1.dialogueFont.MeasureString(label).X + 64;
            height = 68;
            bounds = new Rectangle(0, 0, width, height);
            active = isActive;
        }
        public void SetPosition(int x, int y)
        {
            bounds.X = x; bounds.Y = y;
        }
        public void CallEvent()
        {
            if (active)
                Action();
        }

        public void PerformHover(int x, int y)
        {
            if (this.containsPoint(x, y))
                textColor = Color.White;
            else
                textColor = Game1.textColor;
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
            Utility.drawTextWithShadow(b, Label, Game1.dialogueFont, new Vector2(bounds.Center.X, bounds.Center.Y) - string_center, textColor, 1f, draw_layer + 1E-06f, -1, -1, 0f);
        }
    }
}