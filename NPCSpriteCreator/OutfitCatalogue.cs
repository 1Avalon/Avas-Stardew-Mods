using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPCSpriteCreator
{
    public class OutfitCatalogue : IClickableMenu
    {

        private int width = 960;

        private int height = 540;

        public List<ClickableTextureComponent> maleWeddingOutfits;

        
        private void GetWeddingOutfit(string outfitName)
        {
            
            switch (outfitName)
            {
                case "Male_Wedding_1.png":
                    Clothing shirt = new Clothing("1010");
                    Clothing pants = new Clothing("0");
                    Boots boots = new Boots("511");
                    Game1.player.Equip<Clothing>(shirt, Game1.player.shirtItem);
                    Game1.player.Equip<Clothing>(pants, Game1.player.pantsItem);
                    Game1.player.Equip<Boots>(boots, Game1.player.boots);
                    Game1.player.changePantsColor(new Color(51, 50, 39));
                    Game1.player.UpdateClothing();
                    break;
            }
    }
        

        public OutfitCatalogue()
        {
            Vector2 center = Utility.getTopLeftPositionForCenteringOnScreen(width, height);

            base.initialize((int)center.X, (int)center.Y, width, height);

            int scale = 4;

            maleWeddingOutfits = new List<ClickableTextureComponent>();

            foreach (var kvp in ModEntry.maleWeddingOutfits)
            {
                ClickableTextureComponent c = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + 50, yPositionOnScreen + 120, 16 * scale, 32 * scale), kvp.Value, Rectangle.Empty, scale)
                {
                    name = kvp.Key
                };

                maleWeddingOutfits.Add(c);
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            foreach (ClickableTextureComponent c in maleWeddingOutfits)
            {
                if (c.containsPoint(x, y)) 
                {
                    GetWeddingOutfit(c.name);
                }
            }
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);

            foreach(var c in maleWeddingOutfits)
                c.draw(b);

            drawMouse(b);
        }
    }
}
