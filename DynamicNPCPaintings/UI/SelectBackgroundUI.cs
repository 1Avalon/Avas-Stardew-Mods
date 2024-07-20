using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicNPCPaintings.UI
{
    public class SelectBackgroundUI : IClickableMenu //Next time make a select ui class and inherit from it to save a lot of work lol
    {
        private Customiser customiser;

        public List<ClickableTextureComponent> validBackgroundComponents = new List<ClickableTextureComponent>();

        private string hoverText = "";

        private Dictionary<string, Texture2D> backgroundData;

        private ClickableTextureComponent upArrow;

        private ClickableTextureComponent downArrow;

        private ClickableTextureComponent scrollBar;

        private Rectangle scrollBarRunner;

        private int currentIndex = 0;

        private int rowIndex;

        private int startPositionX;

        private int startPositionY;

        private int backgroundScale;

        private bool canScrollDown;

        private int maxScrollIndex = 0;

        private List<int> elementsInRows = new List<int>();

        private bool keepAddingToElementsInRow;
        public SelectBackgroundUI(Customiser customiser) 
        {
            int width = 960;
            int height = 720;
            this.customiser = customiser;
            base.initialize(Game1.viewport.Width / 2 - width / 2, Game1.viewport.Height / 2 - height / 2, width, height);
            backgroundScale = 4;
            keepAddingToElementsInRow = true;
            rowIndex = 0;
            canScrollDown = true;

            upArrow = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen + 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f);
            downArrow = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen + height - 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f);
            scrollBar = new ClickableTextureComponent(new Rectangle(upArrow.bounds.X + 12, upArrow.bounds.Y + upArrow.bounds.Height + 4, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
            scrollBarRunner = new Rectangle(scrollBar.bounds.X, upArrow.bounds.Y + upArrow.bounds.Height + 4, scrollBar.bounds.Width, height - 128 - upArrow.bounds.Height - 8);
            upperRightCloseButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 50, yPositionOnScreen + 69, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);


            InitBackgroundData();

            EvaluateMaxScrollIndex();
            exitFunction = () => { Game1.activeClickableMenu = customiser; };
        }

        private void EvaluateMaxScrollIndex()
        {
            UpdateBackgroundList(currentIndex);

            int index = 0;
            while (canScrollDown)
            {
                UpdateBackgroundList(elementsInRows[index]);
                index++;
            }
            maxScrollIndex = index;
            currentIndex = 0;
            UpdateBackgroundList(0);
        }
        private void UpdateBackgroundList(int indexOffset) // I have no f ing idea what im doing but it works lol
        {
            int elementCounter = 0;
            int index = 0;
            currentIndex += indexOffset;
            if (currentIndex < 0)
                currentIndex = 0;
            validBackgroundComponents.Clear();
            startPositionX = xPositionOnScreen + 50;
            startPositionY = yPositionOnScreen + 110;
            bool keepAddingToComponents = true;
            foreach (var kvp in backgroundData)
            {
                int bgWidth = kvp.Value.Width;
                int bgHeight = kvp.Value.Height;

                ClickableTextureComponent component = new ClickableTextureComponent(new Rectangle(startPositionX, startPositionY, bgWidth * backgroundScale, bgHeight * backgroundScale), kvp.Value, new Rectangle(0, 0, kvp.Value.Width, kvp.Value.Height), backgroundScale);
                component.name = kvp.Key;

                if (kvp.Value.Width < customiser.picture.frame.spaceWidth || kvp.Value.Height < customiser.picture.frame.spaceHeight)
                    continue;

                index++;
                if (index <= currentIndex)
                    continue;

                elementCounter++;
                if (keepAddingToComponents)
                    validBackgroundComponents.Add(component);

                startPositionX += bgWidth * backgroundScale + 5;
                if (startPositionX + bgWidth * backgroundScale > xPositionOnScreen + this.width - 32)
                {
                    startPositionX = xPositionOnScreen + 50;
                    startPositionY += 32 * backgroundScale + 5;
                    if (keepAddingToElementsInRow)
                        elementsInRows.Add(elementCounter);
                    elementCounter = 0;
                }
                if (startPositionY + bgHeight * backgroundScale > yPositionOnScreen + this.height + 5)
                {
                    keepAddingToComponents = false;
                    canScrollDown = true;
                }
            }
            if (keepAddingToElementsInRow)
                elementsInRows.Add(elementCounter);

            if (keepAddingToComponents)
                canScrollDown = false;

            keepAddingToElementsInRow = false;
        }

        private void downArrowClick()
        {
            if (rowIndex == maxScrollIndex)
                return;

            UpdateBackgroundList(elementsInRows[rowIndex]);
            rowIndex++;
        }

        private void upArrowClick()
        {
            rowIndex = Math.Max(0, --rowIndex);
            UpdateBackgroundList(-elementsInRows[rowIndex]);
        }

        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);
            if (direction > 0)
                upArrowClick();
            else if (direction < 0)
                downArrowClick();
        }
        private void InitBackgroundData()
        {
            backgroundData = new Dictionary<string, Texture2D>();

            foreach (var kvp in ModEntry.backgroundImages)
                backgroundData.Add(kvp.Key, kvp.Value);

            foreach (var kvp in ModEntry.instance.Helper.GameContent.Load<Dictionary<string, Framework.Background>>(ModEntry.BACKGROUND_KEY))
                backgroundData.Add(kvp.Key, kvp.Value.backgroundImage);
        }
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (upperRightCloseButton.containsPoint(x, y))
                Game1.activeClickableMenu = customiser;

            else if (downArrow.containsPoint(x, y))
                downArrowClick();
            else if (upArrow.containsPoint(x, y))
                upArrowClick();
            else
            {
                foreach (ClickableTextureComponent component in validBackgroundComponents)
                {
                    if (component.containsPoint(x, y))
                    {
                        customiser.picture.background = new Framework.Background(0, 0, component.texture);
                        customiser.UpdatePreview();
                        Game1.activeClickableMenu = customiser;
                    }
                }
            }
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            hoverText = "";
            foreach (ClickableTextureComponent component in validBackgroundComponents)
            {
                if (component.containsPoint(x, y))
                {
                    hoverText = component.name;
                }
            }
        }
        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
            foreach (ClickableTextureComponent component in validBackgroundComponents)
                component.draw(b);

            upperRightCloseButton.draw(b);
            upArrow.draw(b);
            downArrow.draw(b);
            scrollBar.draw(b);
            drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), scrollBarRunner.X, scrollBarRunner.Y, scrollBarRunner.Width, scrollBarRunner.Height, Color.White, 4f);
            drawHoverText(b, hoverText, Game1.smallFont);
            drawMouse(b);
        }
    }
}
