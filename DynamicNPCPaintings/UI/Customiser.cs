using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using DynamicNPCPaintings.Framework;
using Background = DynamicNPCPaintings.Framework.Background;

namespace DynamicNPCPaintings.UI
{
    public class Customiser : IClickableMenu
    {

        public ClickableTextureComponent preview;

        public Button npcListButton;

        public Button backgroundListButton;

        public Button frameListButton;

        public Button exportButton;

        public List<Button> buttons;

        public Texture2D previewTexture;

        public Texture2D backgroundTexture;

        private Texture2D looseSprites = Game1.content.Load<Texture2D>("LooseSprites/Cursors");

        public ClickableTextureComponent increaseFrameArrow;

        public ClickableTextureComponent decreaseFrameArrow;

        public Picture picture = Picture.GetDefaultPicture();

        private OffsetWheel npcOffsetWheel;

        private OffsetWheel backgroundOffsetWheel;


        public Customiser() 
        {
            int width = 960;
            int height = 720;
            base.initialize(Game1.viewport.Width / 2 - width / 2, Game1.viewport.Height / 2 - height / 2, width, height);

            previewTexture = picture.GetTexture();
            preview = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + 50, yPositionOnScreen + 120, 48, 32), previewTexture, new Rectangle(0, 0, 48, 32), 6f);

            npcListButton = new Button("Open NPC List", delegate
            {
                Game1.playSound("dwoop");
                Game1.activeClickableMenu = new SelectNPCMenu(this);
            });
            npcListButton.SetPosition(xPositionOnScreen + width - npcListButton.width - 64, yPositionOnScreen + 150);

            backgroundListButton = new Button("Open Background List", delegate
            {
                Game1.playSound("dwoop");
                Game1.activeClickableMenu = new SelectBackgroundUI(this);
            });
            backgroundListButton.SetPosition(npcListButton.bounds.X - (backgroundListButton.width - npcListButton.width), npcListButton.bounds.Y + 100);

            frameListButton = new Button("Open Frame List", delegate
            {
                Game1.playSound("dwoop");
                Game1.activeClickableMenu = new SelectFrameMenu(this);
            });
            frameListButton.SetPosition(backgroundListButton.bounds.X - (frameListButton.width - backgroundListButton.width), backgroundListButton.bounds.Y + 100);

            exportButton = new Button("Export", delegate
            {
                Game1.playSound("dwoop");
                TextureHelper.ExportToPainting(picture);
            });
            exportButton.SetPosition(frameListButton.bounds.X - (exportButton.width - frameListButton.width), frameListButton.bounds.Y + 100);

            buttons = new List<Button>()
            { exportButton, frameListButton,  backgroundListButton, npcListButton};

            npcOffsetWheel = new OffsetWheel(xPositionOnScreen + 100, yPositionOnScreen + 500, "NPC", 20, 3);
            backgroundOffsetWheel = new OffsetWheel(npcOffsetWheel.positionX + 250, npcOffsetWheel.positionY, "Background", 20, 3);

            int arrowScale = 4;
            increaseFrameArrow = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + 50, yPositionOnScreen + 350, 12 * arrowScale, 11 * arrowScale), looseSprites, new Rectangle(352, 495, 12, 11), arrowScale);
        }
        public void UpdatePreview()
        {
            preview.texture = picture.GetTexture();
        }
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (npcListButton.containsPoint(x, y))
                npcListButton.CallEvent();

            else if (backgroundListButton.containsPoint(x, y))
                backgroundListButton.CallEvent();

            else if (frameListButton.containsPoint(x, y))
                frameListButton.CallEvent();

            else if (exportButton.containsPoint(x, y))
                exportButton.CallEvent();

            else if (increaseFrameArrow.containsPoint(x, y))
            {
                picture.npcFrame++;
            }
            npcOffsetWheel.click(x, y, ref picture.npcOffsetX, ref picture.npcOffsetY);
            backgroundOffsetWheel.click(x, y, ref picture.background.offsetX, ref picture.background.offsetY);
            preview.texture = picture.GetTexture();
        }

        public override void performHoverAction(int x, int y)
        {
            foreach (Button button in buttons)
            {
                if (button.containsPoint(x, y))
                    button.textColor = Color.White;
                else
                {
                    button.textColor = Game1.textColor;
                }
            }
        }
        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
            Utility.drawTextWithShadow(b, $"Tile Size: {picture.frame.frameTexture.Width / 16}x{picture.frame.frameTexture.Height / 16}", Game1.smallFont, new Vector2(preview.bounds.X + 16, preview.bounds.Bottom + 150), Game1.textColor, 0.8f);
            Utility.drawTextWithShadow(b, "Frame", Game1.smallFont, new Vector2(increaseFrameArrow.bounds.X + 100, increaseFrameArrow.bounds.Y), Game1.textColor, 1.5f);
            npcListButton.draw(b);
            backgroundListButton.draw(b);
            frameListButton.draw(b);
            exportButton.draw(b);
            increaseFrameArrow.draw(b);
            preview.draw(b);
            npcOffsetWheel.draw(b);
            backgroundOffsetWheel.draw(b);
            drawMouse(b);
        }
    }
}
