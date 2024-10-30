using CustomNPCPaintings.Framework;
using DynamicNPCPaintings;
using DynamicNPCPaintings.Framework;
using DynamicNPCPaintings.UI;
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

namespace CustomNPCPaintings.UI
{
    public class NPCModifierMenu : IClickableMenu
    {

        private Customiser customiser;

        private Button AddNPCButton;

        private List<ClickableNPCComponent> npcComponents;

        private int npcComponentsStartX;

        private int npcComponentsStartY;

        private int npcScale = 4;

        private CharacterLayer targetLayer;

        private OffsetWheel npcOffsetWheel;

        public FrameSwitcher switcher;

        public FrameSwitcher layerSwitcher;

        public Checkbox flipCheckbox;

        public NPCModifierMenu(Customiser customiser) 
        {
            int width = 960;
            int height = 720;
            this.customiser = customiser;
            npcComponentsStartX = customiser.preview.bounds.X + customiser.preview.bounds.Width * 5 + 64 + 30;
            npcComponentsStartY = customiser.preview.bounds.Y;
            targetLayer = customiser.picture.characterLayers[0];
            base.initialize(Game1.viewport.Width / 2 - width / 2, Game1.viewport.Height / 2 - height / 2, width, height);
            npcComponents = new List<ClickableNPCComponent>();
            AddNPCButton = new Button("Add NPC", delegate
            {
                Game1.activeClickableMenu = new SelectNPCMenu(this.customiser);
            });
            AddNPCButton.setPosition(customiser.npcListButton.bounds.X, customiser.npcListButton.bounds.Y);

            foreach(CharacterLayer layer in customiser.picture.characterLayers)
            {
                int yOffset = TextureHelper.FindFirstNonTransparentPixelY(layer.target.Sprite.Texture);
                npcComponents.Add(new ClickableNPCComponent(new Rectangle(npcComponentsStartX, npcComponentsStartY, npcScale * 16, npcScale * 16), layer.target, new Rectangle(0, yOffset, 16, 16), npcScale));
                npcComponentsStartX += 16 * npcScale;
            }

            npcOffsetWheel = new OffsetWheel(xPositionOnScreen + 100, yPositionOnScreen + 550, I18n.Menu_NPC(), 20, 3);
            switcher = new FrameSwitcher(I18n.Menu_NPCFrame(), xPositionOnScreen + 50 + 20, yPositionOnScreen + 350, 20, 4);
            layerSwitcher = new FrameSwitcher("Layer", switcher.positionX + 550, switcher.positionY, 20, 4);
            flipCheckbox = new Checkbox("Flip", new Rectangle(switcher.positionX + 8, switcher.positionY + 80, 36, 36), I18n.Menu_FlipNPC());

            exitFunction = () => Game1.activeClickableMenu = this.customiser;
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            foreach (ClickableNPCComponent component in  npcComponents)
            {
                if (component.containsPoint(x, y))
                { 
                    foreach(CharacterLayer layer in customiser.picture.characterLayers)
                        if (layer.target == component.npc)
                            targetLayer = layer;
                }
            }

            if (AddNPCButton.containsPoint(x, y))
                AddNPCButton.CallEvent();

            switcher.click(x, y, ref targetLayer);
            npcOffsetWheel.click(x, y, ref targetLayer.npcOffsetX, ref targetLayer.npcOffsetY);
            flipCheckbox.click(x, y, ref targetLayer.npcFlipped);
            layerSwitcher.click(x, y, ref targetLayer.layer, 0, customiser.picture.characterLayers.Count - 1);
            customiser.UpdatePreview();
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            if (AddNPCButton.containsPoint(x, y))
                AddNPCButton.textColor = Color.White;
            else
                AddNPCButton.textColor = Game1.textColor;
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
            AddNPCButton.draw(b);
            foreach (ClickableNPCComponent component in npcComponents)
            {
                float opacity = 0.3f;
                if (component.npc == targetLayer.target)
                    opacity = 1f;

                component.draw(b, Color.White * opacity, 1f);
            }
            customiser.drawTileSizeText(b);
            customiser.preview.draw(b);
            npcOffsetWheel.draw(b);
            switcher.draw(b);
            flipCheckbox.draw(b);
            layerSwitcher.draw(b);
            string frameText = $"{targetLayer.npcFrame + 1}/{targetLayer.npcFrameAmount}";
            string layerText = (targetLayer.layer + 1).ToString();
            Utility.drawTextWithShadow(b, layerText, Game1.smallFont, new Vector2((layerSwitcher.positionX + layerSwitcher.width / 2 - Game1.smallFont.MeasureString(layerText).X), layerSwitcher.positionY + 40), Game1.textColor, 0.8f);
            Utility.drawTextWithShadow(b, frameText, Game1.smallFont, new Vector2((switcher.positionX + switcher.width / 2 - Game1.smallFont.MeasureString(frameText).X), switcher.positionY + 40), Game1.textColor, 0.8f);
            drawMouse(b);
        }
    }
}
