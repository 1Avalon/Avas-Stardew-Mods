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

        List<NPC> validNPCs = new List<NPC>();

        private string hoverText;

        private Customiser customiser;
        public SelectNPCMenu(Customiser customiser)
        {
            validNPCs = Utility.getAllCharacters().Where(npc => npc.CanSocialize).ToList();
            int width = 960;
            int height = 720;
            base.initialize(Game1.viewport.Width / 2 - width / 2, Game1.viewport.Height / 2 - height / 2, width, height);

            int startPositionX = xPositionOnScreen + 30;
            int startPositionY = yPositionOnScreen + 110;
            int npcScale = 4;

            foreach (var npc in validNPCs)
            {
                ClickableNPCComponent component = new ClickableNPCComponent(new Rectangle(startPositionX, startPositionY, 16 * npcScale, 16 * npcScale), npc, new Rectangle(0, 5, 16, 16), npcScale);
                NPCComponents.Add(component);
                startPositionX += 16 * npcScale;
                if (startPositionX > xPositionOnScreen + this.width - 16 * npcScale)
                {
                    startPositionX = xPositionOnScreen + 30;
                    startPositionY += 16 * npcScale + 5;
                }
            }

            this.customiser = customiser;
            exitFunction = () => { Game1.activeClickableMenu = this.customiser; };
        }
        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            hoverText = "";
            foreach (ClickableNPCComponent component in NPCComponents)
            {
                if (component.containsPoint(x, y))
                {
                    hoverText = component.npc.displayName;
                }
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            foreach(ClickableNPCComponent component in NPCComponents)
            {
                if (component.containsPoint(x, y))
                {
                    customiser.picture.target = component.npc;
                    customiser.picture.npcFrame = 0;
                    customiser.UpdatePreview();
                    Game1.activeClickableMenu = customiser;
                    return;
                }
            }
        }
        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
            foreach(ClickableNPCComponent component in NPCComponents)
                component.draw(b);

            drawHoverText(b, hoverText, Game1.smallFont);
            drawMouse(b);
        }
    }
}
