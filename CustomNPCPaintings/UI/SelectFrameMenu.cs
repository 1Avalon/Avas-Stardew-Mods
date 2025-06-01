using DynamicNPCPaintings.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicNPCPaintings.UI
{
    internal class SelectFrameMenu : IClickableMenu
    {
        private string hoverText = "";

        public Dictionary<string, Frame> frames = new Dictionary<string, Frame>();

        public List<ClickableTextureComponent> components = new List<ClickableTextureComponent>();

        public Dictionary<ClickableTextureComponent, string> componentUniqueKeys = new Dictionary<ClickableTextureComponent, string>();

        private Customiser customiser;

        private ClickableTextureComponent upArrow;

        private ClickableTextureComponent downArrow;

        private ClickableTextureComponent scrollBar;

        private Rectangle scrollBarRunner;

        private int startPositionX;

        private int startPositionY;

        private int currentIndex = 0;

        private int maxScrollIndex = 0;

        private List<int> elementsInRows = new List<int>();

        private bool keepAddingToElementsInRow;

        private bool canScrollDown;

        private int frameScale;

        private int rowIndex;
        public SelectFrameMenu(Customiser customiser) 
        {
            int width = 960;
            int height = 720;
            this.customiser = customiser;
            Vector2 center = Utility.getTopLeftPositionForCenteringOnScreen(width, height);
            base.initialize((int)center.X, (int)center.Y, width, height);

            frameScale = 4;
            rowIndex = 0;
            keepAddingToElementsInRow = true;

            frames = ModEntry.instance.Helper.GameContent.Load<Dictionary<string, Frame>>(ModEntry.FRAME_KEY);

            upperRightCloseButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 50, yPositionOnScreen + 69, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);

            upArrow = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen + 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f);
            downArrow = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen + height - 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f);
            scrollBar = new ClickableTextureComponent(new Rectangle(upArrow.bounds.X + 12, upArrow.bounds.Y + upArrow.bounds.Height + 4, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
            scrollBarRunner = new Rectangle(scrollBar.bounds.X, upArrow.bounds.Y + upArrow.bounds.Height + 4, scrollBar.bounds.Width, height - 128 - upArrow.bounds.Height - 8);
            /*
            foreach (var kvp in frames)
            {
                int bgWidth = kvp.Value.frameTexture.Width;
                int bgHeight = kvp.Value.frameTexture.Height;
                ClickableTextureComponent component = new ClickableTextureComponent(new Rectangle(startPositionX, startPositionY, bgWidth * frameScale, bgHeight * frameScale), kvp.Value.frameTexture, new Rectangle(0, 0, kvp.Value.frameTexture.Width, kvp.Value.frameTexture.Height), frameScale);
                component.name = kvp.Key;
                components.Add(component);
                startPositionX += bgWidth * frameScale + 10;
                if (startPositionX + bgHeight * frameScale > xPositionOnScreen + this.width - 32)
                {
                    startPositionX = xPositionOnScreen + 50;
                    startPositionY += 32 * frameScale + 5;
                }
            }
            */
            EvaluateMaxScrollIndex();
            exitFunction = () => { Game1.activeClickableMenu = this.customiser; };
        }

        private void UpdateFrameList(int indexOffset)
        {
            currentIndex += indexOffset;
            if (currentIndex < 0)
                currentIndex = 0;

            startPositionX = xPositionOnScreen + 50;
            startPositionY = yPositionOnScreen + 110;

            components.Clear();
            bool keepAddingToComponents = true;
            int elementCounter = 0;
            int addedElements = 0;
            int maxFrameHeight = 0;

            foreach (var kvp in frames)
            {
                int frameWidth = kvp.Value.frameTexture.Width;
                int frameHeight = kvp.Value.frameTexture.Height;

                if (frameHeight > maxFrameHeight)
                    maxFrameHeight = frameHeight;

                // Skip elements until reaching the current index
                elementCounter++;
                if (elementCounter <= currentIndex)
                    continue;

                // Check if the next component would exceed the width of the screen
                if (startPositionX + frameWidth * frameScale > xPositionOnScreen + this.width - 50)
                {
                    startPositionX = xPositionOnScreen + 50;
                    startPositionY += maxFrameHeight * frameScale + 5;
                    maxFrameHeight = frameHeight; // Reset max height for the new row

                    if (keepAddingToElementsInRow)
                        elementsInRows.Add(addedElements);

                    addedElements = 0;
                }

                // Check if the next component would exceed the height of the screen
                if (startPositionY + frameHeight * frameScale > yPositionOnScreen + this.height - 50)
                {
                    keepAddingToComponents = false;
                    canScrollDown = true;
                    break; // Exit the loop if we can't add more components
                }

                ClickableTextureComponent component = new ClickableTextureComponent(
                    new Rectangle(startPositionX, startPositionY, frameWidth * frameScale, frameHeight * frameScale),
                    kvp.Value.frameTexture, new Rectangle(0, 0, frameWidth, frameHeight), frameScale)
                {
                    name = kvp.Value.displayName
                };


                if (keepAddingToComponents)
                {
                    components.Add(component);
                    componentUniqueKeys.Add(component, kvp.Key);
                }
                addedElements++;
                startPositionX += frameWidth * frameScale + 10; // Update the X position for the next component
            }

            if (keepAddingToElementsInRow)
                elementsInRows.Add(addedElements);

            if (keepAddingToComponents)
                canScrollDown = false;

            keepAddingToElementsInRow = false;
        }

        private void EvaluateMaxScrollIndex()
        {
            UpdateFrameList(currentIndex);

            int index = 0;
            while (canScrollDown)
            {
                UpdateFrameList(elementsInRows[index]);
                index++;
            }
            maxScrollIndex = index;
            currentIndex = 0;
            UpdateFrameList(0);
        }

        private void downArrowClick()
        {
            if (rowIndex == maxScrollIndex)
               return;

            Game1.playSound("shiny4");
            UpdateFrameList(elementsInRows[rowIndex]);
            rowIndex++;
        }

        private void upArrowClick()
        {
            Game1.playSound("shiny4");
            rowIndex = Math.Max(0, --rowIndex);
            UpdateFrameList(-elementsInRows[rowIndex]);
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            hoverText = "";
            foreach (ClickableTextureComponent component in components)
            {
                if (component.containsPoint(x, y))
                {
                    hoverText = component.name;
                }
            }
        }

        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);
            if (direction > 0)
                upArrowClick();
            else if (direction < 0)
                downArrowClick();
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (upperRightCloseButton.containsPoint(x, y))
                Game1.activeClickableMenu = customiser;
            else if (upArrow.containsPoint(x, y))
                upArrowClick();
            else if (downArrow.containsPoint(x, y))
                downArrowClick();
            else
            {
                foreach (ClickableTextureComponent component in components)
                {
                    if (component.containsPoint(x, y))
                    {

                        string uniqueKey = componentUniqueKeys[component];

                        Frame targetFrame = frames[uniqueKey];

                        //if (customiser.picture.frame.spaceWidth != targetFrame.spaceWidth)
                            //customiser.picture.npcOffsetX = targetFrame.spaceWidth / 2 - 4;

                        customiser.picture.frame = targetFrame;

                        if (!customiser.picture.background.FitsInFrame(customiser.picture.frame))
                            customiser.picture.background = Framework.Background.GetDefaultBackground();

                        customiser.UpdatePreview();
                        Game1.activeClickableMenu = customiser;
                    }
                }
            }
        }
        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
            foreach (ClickableTextureComponent component in components)
                component.draw(b);

            upperRightCloseButton.draw(b);
            upArrow.draw(b);
            downArrow.draw(b);
            drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), scrollBarRunner.X, scrollBarRunner.Y, scrollBarRunner.Width, scrollBarRunner.Height, Color.White, 4f);
            scrollBar.draw(b);
            drawHoverText(b, hoverText, Game1.smallFont);
            drawMouse(b);
        }
    }
}
