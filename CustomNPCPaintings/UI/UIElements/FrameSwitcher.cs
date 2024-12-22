using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using Microsoft.Xna.Framework.Graphics;
using DynamicNPCPaintings.Framework;
using CustomNPCPaintings.Framework;
using System.ComponentModel.DataAnnotations;

namespace DynamicNPCPaintings.UI.UIElements
{
    public class FrameSwitcher
    {
        public ClickableTextureComponent arrowLeft;

        public ClickableTextureComponent arrowRight;

        public string Label;

        public int positionX;

        public int positionY;

        public int width;

        public int height;

        public int scale;

        public int spacing;
        public FrameSwitcher(string label, int positionX, int positionY, int spacing = 5, int scale = 1)
        {
            this.Label = label;
            this.positionX = positionX;
            this.positionY = positionY;
            this.scale = scale;
            this.spacing = spacing;
            Vector2 labelLength = Game1.dialogueFont.MeasureString(label); 
            arrowLeft = new ClickableTextureComponent(new Rectangle(positionX - spacing, positionY, 12 * scale, 11 * scale), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), scale);
            arrowRight = new ClickableTextureComponent(new Rectangle(positionX + (int)labelLength.X  + 12 * scale + spacing, positionY, 12 * scale, 11 * scale), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), scale);
            height = (int)labelLength.Y;
            width = 12 * scale * 2 + spacing * 2 + (int)labelLength.X;

        }

        public void draw(SpriteBatch b)
        {
            arrowRight.draw(b);
            arrowLeft.draw(b);
            Utility.drawTextWithShadow(b, Label, Game1.dialogueFont, new Vector2(positionX + 12 * scale, positionY), Game1.textColor);
        }

        public void click(int x, int y, ref CharacterLayer layer)
        {

            if (arrowLeft.containsPoint(x, y) && layer.npcFrame  > 0)
            {
                Game1.playSound("smallSelect");
                layer.npcFrame--;
            }
                
            else if (arrowRight.containsPoint(x, y) && layer.npcFrame < layer.npcFrameAmount -1)
            {
                Game1.playSound("smallSelect");
                layer.npcFrame++;
            }

        }

        public void click(int x, int y, ref int value, int min = 0, int max = 999)
        {
            if (arrowLeft.containsPoint(x, y) && value > min)
                value--;
            else if (arrowRight.containsPoint(x, y) && value < max)
                value++;
        }
    }
}
