using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DynamicNPCPaintings.UI
{
    public class OffsetWheel
    {
        private readonly List<Vector2> arrowPositionsLooseSprites = new List<Vector2>()
        {
            new Vector2(352, 495),
            new Vector2(421, 459),
            new Vector2(365, 495),
            new Vector2(421, 472)
        };

        public List<ClickableTextureComponent> components = new List<ClickableTextureComponent>(); //left up right down arrows

        public string label;

        public int positionX;

        public int positionY;

        public int scale;

        public int spacing;

        private Texture2D looseSprites = Game1.content.Load<Texture2D>("LooseSprites/Cursors");

        public OffsetWheel(int positionX, int positionY, string label, int spacing = 0, int scale = 1)
        {
            this.label = label;
            this.positionX = positionX;
            this.positionY = positionY;
            this.scale = scale;
            this.spacing = spacing;

            int width = 12;
            int height = 11;
            int _ = 0;

            Vector2 labelLength = Game1.dialogueFont.MeasureString(label);

            List<Vector2> arrowPositions = new List<Vector2>()
            {
                new Vector2(positionX - 12 * scale - spacing, positionY + labelLength.Y / 4 - 5),
                new Vector2(positionX + labelLength.X / 2 - 11 * (scale - 1), positionY - labelLength.Y / 2 - spacing),
                new Vector2(positionX + labelLength.X + spacing, positionY + labelLength.Y / 4 - 5),
                new Vector2(positionX + labelLength.X / 2 - 11 * (scale - 1), positionY + labelLength.Y / 2 + 12 + spacing)
            };

            int i = 0;
            foreach (Vector2 position in arrowPositionsLooseSprites)
            {
                Vector2 arrowPosition = arrowPositions[i];
                ClickableTextureComponent component = new ClickableTextureComponent(new Rectangle((int)arrowPosition.X, (int)arrowPosition.Y, 12 * scale, 11 * scale), looseSprites, new Rectangle((int)position.X, (int)position.Y, width, height), scale);
                _ = width;
                width = height;
                height = _;
                i++;
                components.Add(component);
            }
        }
        public void click(int x, int y, ref int offsetX, ref int offsetY)
        {
            int i = 0;
            foreach (ClickableTextureComponent component in components)
            {
                if (component.containsPoint(x, y))
                {
                    Game1.playSound("smallSelect");
                    switch (i)
                    {
                        case 0:
                            offsetX--;
                            break;
                        case 1: 
                            offsetY--;
                            break;
                        case 2:
                            offsetX++;
                            break;
                        case 3: 
                            offsetY++;
                            break;
                    }
                }
                i++;
            }
        }
        public void draw(SpriteBatch b)
        {
            Utility.drawTextWithShadow(b, label, Game1.dialogueFont, new Vector2(positionX, positionY), Color.Black);
            components.ForEach(component => { component.draw(b); });

        }
    }
}
