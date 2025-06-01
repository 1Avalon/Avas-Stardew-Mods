using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using DynamicNPCPaintings.Framework;
using Background = DynamicNPCPaintings.Framework.Background;
using DynamicNPCPaintings.UI.UIElements;
using CustomNPCPaintings;
using CustomNPCPaintings.UI;

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

        public Checkbox flipCheckbox;

        public Texture2D previewTexture;

        public Texture2D backgroundTexture;

        private Texture2D looseSprites = Game1.content.Load<Texture2D>("LooseSprites/Cursors");

        public Picture picture = Picture.GetDefaultPicture();

        private OffsetWheel npcOffsetWheel;

        private OffsetWheel backgroundOffsetWheel;

        public FrameSwitcher switcher;


        public Customiser() 
        {
            int width = 960;
            int height = 720;

            Vector2 center = Utility.getTopLeftPositionForCenteringOnScreen(width, height);

            base.initialize((int)center.X, (int)center.Y, width, height);

            previewTexture = picture.GetTexture();
            preview = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + 50, yPositionOnScreen + 120, 48, 32), previewTexture, new Rectangle(0, 0, 48, 32), 6f);

            npcListButton = new Button(I18n.Menu_Customize(), delegate
            {
                Game1.playSound("dwop");
                Game1.activeClickableMenu = new NPCModifierMenu(this);
            });
            npcListButton.SetPosition(xPositionOnScreen + width - npcListButton.width - 64, yPositionOnScreen + 150);

            backgroundListButton = new Button(I18n.Menu_OpenBackgroundList(), delegate
            {
                Game1.playSound("dwop");
                Game1.activeClickableMenu = new SelectBackgroundUI(this);
            });
            backgroundListButton.SetPosition(npcListButton.bounds.X - (backgroundListButton.width - npcListButton.width), npcListButton.bounds.Y + 100);

            frameListButton = new Button(I18n.Menu_OpenFrameList(), delegate
            {
                Game1.playSound("dwop");
                Game1.activeClickableMenu = new SelectFrameMenu(this);
            });
            frameListButton.SetPosition(backgroundListButton.bounds.X - (frameListButton.width - backgroundListButton.width), backgroundListButton.bounds.Y + 100);

            exportButton = new Button(I18n.Menu_Export(), delegate
            {
                Game1.playSound("dwop");
                TextureHelper.ExportToPainting(picture);
            });
            exportButton.SetPosition(frameListButton.bounds.X - (exportButton.width - frameListButton.width), frameListButton.bounds.Y + 100);

            buttons = new List<Button>()
            { exportButton, frameListButton,  backgroundListButton, npcListButton};

            backgroundOffsetWheel = new OffsetWheel(xPositionOnScreen + 125, yPositionOnScreen + 450, I18n.Menu_Background(), 20, 3);

            int arrowScale = 4;

            upperRightCloseButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 50, yPositionOnScreen + 69, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);
        }
        public void UpdatePreview()
        {
            preview.texture = picture.GetTexture();
            preview.sourceRect = new Rectangle(0, 0, picture.frame.frameTexture.Width, picture.frame.frameTexture.Height);
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
            else if (upperRightCloseButton.containsPoint(x, y))
                exitThisMenu();

            //flipCheckbox.click(x, y, ref picture.npcFlipped);
            //npcOffsetWheel.click(x, y, ref picture.npcOffsetX, ref picture.npcOffsetY);
            backgroundOffsetWheel.click(x, y, ref picture.background.offsetX, ref picture.background.offsetY);
            //switcher.click(x, y, ref picture);
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

        public void drawTileSizeText(SpriteBatch b)
        {
            Utility.drawTextWithShadow(b, $"{I18n.Menu_TileSize()}: {picture.frame.frameTexture.Width / 16}x{picture.frame.frameTexture.Height / 16}", Game1.smallFont, new Vector2(preview.bounds.X + 16, preview.bounds.Bottom + 150), Game1.textColor, 0.8f);
        }
        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
            drawTileSizeText(b);
            //string frameText = $"{picture.npcFrame + 1}/{picture.npcFrameAmount}";
            //Utility.drawTextWithShadow(b, frameText, Game1.smallFont, new Vector2((switcher.positionX + switcher.width / 2 - Game1.smallFont.MeasureString(frameText).X), switcher.positionY + 40), Game1.textColor, 0.8f);
            //Utility.drawTextWithShadow(b, "Frame", Game1.smallFont, new Vector2(increaseFrameArrow.bounds.X + 100, increaseFrameArrow.bounds.Y), Game1.textColor, 1.5f);
            npcListButton.draw(b);
            backgroundListButton.draw(b);
            frameListButton.draw(b);
            exportButton.draw(b);
            //increaseFrameArrow.draw(b);
            preview.draw(b);
            //npcOffsetWheel.draw(b);
            backgroundOffsetWheel.draw(b);
            upperRightCloseButton.draw(b);
            drawMouse(b);
        }
    }
}
