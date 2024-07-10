using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using WeddingPhoto;

namespace DynamicNPCPaintings.UI
{
    public class Customiser : IClickableMenu
    {

        public ClickableTextureComponent preview;

        public Button npcListButton;

        public Button backgroundList;

        public NPC target;

        public Texture2D previewTexture;

        public Texture2D backgroundTexture;

        private Texture2D looseSprites = Game1.content.Load<Texture2D>("LooseSprites/Cursors");

        public ClickableTextureComponent increaseFrameArrow;

        public ClickableTextureComponent decreaseFrameArrow;

        public int currentNPCFrame = 1;
        public Customiser() 
        {
            int width = 960;
            int height = 720;
            base.initialize(Game1.viewport.Width / 2 - width / 2, Game1.viewport.Height / 2 - height / 2, width, height);

            backgroundTexture = TextureHelper.BackgroundWithFrame(ModEntry.frame, ModEntry.background, 4, 5, 45, 25, 0, 5, Game1.graphics.GraphicsDevice);
            preview = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + 50, yPositionOnScreen + 120, 48, 32), backgroundTexture, new Rectangle(0, 0, 48, 32), 6f);

            npcListButton = npcListButton = new Button("Open NPC List", delegate
            {
                Game1.playSound("dwoop");
                Game1.activeClickableMenu = new SelectNPCMenu(this);
            });

            npcListButton.SetPosition(xPositionOnScreen + width - npcListButton.width - 64, yPositionOnScreen + 150);
            backgroundList = new Button("Open Background List", delegate
            {
                Game1.activeClickableMenu = new SelectNPCMenu(this);
            });

            backgroundList.SetPosition(npcListButton.bounds.X - (backgroundList.width - npcListButton.width), npcListButton.bounds.Y + 100);

            int arrowScale = 4;
            increaseFrameArrow = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + 50, yPositionOnScreen + 350, 12 * arrowScale, 11 * arrowScale), looseSprites, new Rectangle(352, 495, 12, 11), arrowScale);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (npcListButton.containsPoint(x, y))
                npcListButton.CallEvent();

            else if (backgroundList.containsPoint(x, y))
                backgroundList.CallEvent();
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
            Utility.drawTextWithShadow(b, "Frame", Game1.smallFont, new Vector2(increaseFrameArrow.bounds.X + 50, increaseFrameArrow.bounds.Y), Game1.textColor, 1.5f);
            npcListButton.draw(b);
            backgroundList.draw(b);
            increaseFrameArrow.draw(b);
            preview.draw(b);
            drawMouse(b);
        }
    }
}
