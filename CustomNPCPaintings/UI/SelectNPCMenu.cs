﻿using CustomNPCPaintings.Framework;
using CustomNPCPaintings.UI;
using DynamicNPCPaintings.UI.UIElements;
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
    public class SelectNPCMenu : IClickableMenu
    {

        List<ClickableNPCComponent> NPCComponents = new List<ClickableNPCComponent>();

        private static List<NPC> npcs;

        private static List<CharacterLayer> validNPCs;

        private string hoverText;

        private Customiser customiser;

        private ClickableTextureComponent upArrow;

        private ClickableTextureComponent downArrow;

        private ClickableTextureComponent scrollBar;

        private Rectangle scrollBarRunner;

        private int startPositionX;

        private int startPositionY;

        private int npcScale;

        private int currentIndex;

        private int maxScrollDownIndex;

        private int currentScrollIndex;
        public SelectNPCMenu(Customiser customiser)
        {
            npcs = ModEntry.Config.enableAllNPCs ? Utility.getAllCharacters().GroupBy(o => o.displayName).Select(g => g.First()).ToList() : Utility.getAllCharacters().Where(npc => npc.CanSocialize).ToList();
            validNPCs = new List<CharacterLayer>();

            if (ModEntry.Config.enableFarmerSprite)
                validNPCs.Add(new CharacterLayer(Game1.player, customiser.picture.background, 0));
            

            foreach (NPC npc in npcs)
            {
                validNPCs.Add(new CharacterLayer(npc, customiser.picture.background, 0));
            }

            maxScrollDownIndex = (int)Math.Ceiling(validNPCs.Count / 14f - 8);
            if (maxScrollDownIndex < 0)
                maxScrollDownIndex = 0;
            int width = 960;
            int height = 720;
            Vector2 center = Utility.getTopLeftPositionForCenteringOnScreen(width, height);
            base.initialize((int)center.X, (int)center.Y, width, height);

            npcScale = 4;
            currentIndex = 0;
            currentScrollIndex = 0;

            upArrow = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen + 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f);
            downArrow = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen + height - 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f);
            scrollBar = new ClickableTextureComponent(new Rectangle(upArrow.bounds.X + 12, upArrow.bounds.Y + upArrow.bounds.Height + 4, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
            scrollBarRunner = new Rectangle(scrollBar.bounds.X, upArrow.bounds.Y + upArrow.bounds.Height + 4, scrollBar.bounds.Width, height - 128 - upArrow.bounds.Height - 8);
            upperRightCloseButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 50, yPositionOnScreen + 69, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);



            this.customiser = customiser;
            UpdateNPCList(currentIndex);
            exitFunction = () => { Game1.activeClickableMenu = new NPCModifierMenu(this.customiser); };
        }

        public void UpdateNPCList(int startIndex)
        {
            startPositionX = xPositionOnScreen + 30;
            startPositionY = yPositionOnScreen + 110;
            int index = 0;
            NPCComponents.Clear();
            foreach (var npc in validNPCs)
            {
                index++;
                if (index <= startIndex)
                    continue;
                int yOffset = TextureHelper.FindFirstNonTransparentPixelY(npc.Texture);
                ClickableNPCComponent component = new ClickableNPCComponent(new Rectangle(startPositionX, startPositionY, 16 * npcScale, 16 * npcScale), npc, new Rectangle(0, yOffset, 16, 16), npcScale);
                NPCComponents.Add(component);
                startPositionX += 16 * npcScale;
                if (startPositionX > xPositionOnScreen + this.width - 16 * npcScale)
                {
                    startPositionX = xPositionOnScreen + 30;
                    startPositionY += 16 * npcScale + 5;
                }

                if (NPCComponents.Count >= 112) //max elements on page
                    break;

            }
        }
        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            hoverText = "";
            foreach (ClickableNPCComponent component in NPCComponents)
            {
                if (component.containsPoint(x, y))
                {
                    hoverText = component.layer.DisplayName;
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

        private void upArrowClick()
        {
            if (currentIndex == 0)
                return;

            Game1.playSound("shiny4");
            currentIndex = Math.Max(0, currentIndex - 14);
            currentScrollIndex = Math.Max(0, --currentScrollIndex);
            UpdateNPCList(currentIndex);
        }

        private void downArrowClick()
        {
           if (currentScrollIndex == maxScrollDownIndex)
             return;

            Game1.playSound("shiny4");
            currentIndex += 14;
            currentIndex = Math.Min(validNPCs.Count - 14, currentIndex);
            currentScrollIndex = Math.Min(maxScrollDownIndex, ++currentScrollIndex);
            UpdateNPCList(currentIndex);
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
                foreach (ClickableNPCComponent component in NPCComponents)
                {
                    if (component.containsPoint(x, y))
                    {
                        customiser.picture.characterLayers.Add(component.layer);
                        customiser.UpdatePreview();
                        NPCModifierMenu.targetLayer = component.layer;
                        Game1.activeClickableMenu = new NPCModifierMenu(this.customiser);
                        return;
                    }
                }
            }
        }
        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
            foreach(ClickableNPCComponent component in NPCComponents)
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
