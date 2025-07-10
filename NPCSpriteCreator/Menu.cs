using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NPCSpriteCreator.UIElements;
using StardewModdingAPI.Events;
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
        private int width = 960;

        private int height = 1000;

        private ClickableTextureComponent templatePreview;

        private ClickableTextureComponent farmerPreview;

        private Button fetchFarmerSpriteButton;

        private Button exportButton;

        private Button outfitButton;

        public Menu()
        {
            Vector2 center = Utility.getTopLeftPositionForCenteringOnScreen(width, height);

            base.initialize((int)center.X, (int)center.Y, width, height);

            templatePreview = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + 50, yPositionOnScreen + 120, 64 * 2, 416 * 2), ModEntry.templateTexture, Rectangle.Empty, 2f);
            farmerPreview = new ClickableTextureComponent(templatePreview.bounds, ModEntry.resultTexture, Rectangle.Empty, templatePreview.scale);

            outfitButton = new Button("Outfit Catalogue", delegate {
                Game1.activeClickableMenu = new OutfitCatalogue();
            });
            outfitButton.setPosition(xPositionOnScreen + width - outfitButton.width - 64, yPositionOnScreen + 124);

            fetchFarmerSpriteButton = new Button("Add regular Sprites", delegate {
                ModEntry.enableSnap = true;
            });
            fetchFarmerSpriteButton.setPosition(outfitButton.bounds.X - (fetchFarmerSpriteButton.width - outfitButton.width), outfitButton.bounds.Y + 100);

            exportButton = new Button("Export", delegate
            {
                TextureHelper.ExportSheet();
                Game1.activeClickableMenu = null;
            });
            exportButton.setPosition(fetchFarmerSpriteButton.bounds.X - (exportButton.width - fetchFarmerSpriteButton.width), fetchFarmerSpriteButton.bounds.Y + 100);
        }

        public override void performHoverAction(int x, int y)
        {
            fetchFarmerSpriteButton.PerformHover(x, y);
            exportButton.PerformHover(x, y);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (fetchFarmerSpriteButton.containsPoint(x, y))
                fetchFarmerSpriteButton.CallEvent();

            if (exportButton.containsPoint(x, y))
                exportButton.CallEvent();

            if (outfitButton.containsPoint(x, y))
                outfitButton.CallEvent();
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
            templatePreview.draw(b);
            farmerPreview.draw(b);
            fetchFarmerSpriteButton.draw(b);
            exportButton.draw(b);
            outfitButton.draw(b);
            drawMouse(b);
        }
    }
}
