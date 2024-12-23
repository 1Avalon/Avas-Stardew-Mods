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

        private Button RemoveNPCButton;

        public Button exportButton;

        private List<ClickableNPCComponent> npcComponents;

        private int npcComponentsStartX;

        private int npcComponentsStartY;

        private int npcScale = 4;

        public static CharacterLayer targetLayer;

        private OffsetWheel npcOffsetWheel;

        private OffsetWheel backgroundOffsetWheel;

        public FrameSwitcher switcher;

        public FrameSwitcher layerSwitcher;

        public Checkbox flipCheckbox;

        public ClickableTextureComponent test;

        public NPCModifierMenu(Customiser customiser) 
        {
            int width = 960;
            int height = 720;
            this.customiser = customiser;
            npcComponentsStartX = customiser.preview.bounds.X + customiser.preview.bounds.Width * 5 + 64 + 30;
            npcComponentsStartY = customiser.preview.bounds.Y;

            if (customiser.picture.characterLayers.Count == 1)
                targetLayer = customiser.picture.characterLayers[0];
            base.initialize(Game1.viewport.Width / 2 - width / 2, Game1.viewport.Height / 2 - height / 2, width, height);
            npcComponents = new List<ClickableNPCComponent>();
            AddNPCButton = new Button("Add NPC", delegate
            {
                Game1.activeClickableMenu = new SelectNPCMenu(this.customiser);
            });
            AddNPCButton.setPosition(xPositionOnScreen + width - AddNPCButton.width - 64, yPositionOnScreen + 150);

            RemoveNPCButton = new Button("Remove NPC", delegate
            {
                this.customiser.picture.characterLayers.Remove(targetLayer);
                int layerAmount = customiser.picture.characterLayers.Count;
                if (layerAmount > 0)
                    targetLayer = customiser.picture.characterLayers[layerAmount - 1];
                UpdateCharacterLayers();
                
            });
            RemoveNPCButton.setPosition(AddNPCButton.bounds.X - (RemoveNPCButton.width - AddNPCButton.width), AddNPCButton.bounds.Y + 100);

            exportButton = new Button(I18n.Menu_Export(), delegate
            {
                Game1.playSound("dwop");
                TextureHelper.ExportToPainting(this.customiser.picture);
            });
            exportButton.setPosition(AddNPCButton.bounds.X - (exportButton.width - AddNPCButton.width), RemoveNPCButton.bounds.Y + 100);

            UpdateCharacterLayers();

            upperRightCloseButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 50, yPositionOnScreen + 69, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);

            npcOffsetWheel = new OffsetWheel(xPositionOnScreen + 100, yPositionOnScreen + 550, I18n.Menu_NPC(), 20, 3);
            backgroundOffsetWheel = new OffsetWheel(npcOffsetWheel.positionX + 200, npcOffsetWheel.positionY, I18n.Menu_Background(), 20, 3);
            switcher = new FrameSwitcher(I18n.Menu_NPCFrame(), xPositionOnScreen + 50 + 20, yPositionOnScreen + 350, 20, 4);
            layerSwitcher = new FrameSwitcher("Layer", switcher.positionX + 550, switcher.positionY + 20 + exportButton.height, 20, 4);
            flipCheckbox = new Checkbox("Flip", new Rectangle(switcher.positionX + 8, switcher.positionY + 80, 36, 36), I18n.Menu_FlipNPC());

            exitFunction = () => Game1.activeClickableMenu = this.customiser;
        }

        private void UpdateCharacterLayers()
        {
            npcComponents.Clear();
            npcComponentsStartX = customiser.preview.bounds.X + customiser.preview.bounds.Width * 5 + 64 + 30;
            foreach (CharacterLayer layer in customiser.picture.characterLayers)
            {
                int yOffset = TextureHelper.FindFirstNonTransparentPixelY(layer.Texture);
                npcComponents.Add(new ClickableNPCComponent(new Rectangle(npcComponentsStartX, npcComponentsStartY, npcScale * 16, npcScale * 16), layer, new Rectangle(0, yOffset, 16, 16), npcScale));
                npcComponentsStartX += 16 * npcScale;

                if (npcComponentsStartX > AddNPCButton.bounds.X - 16 * npcScale - 30)
                {
                    npcComponentsStartY += 16 * npcScale;
                    npcComponentsStartX = customiser.preview.bounds.X + customiser.preview.bounds.Width * 5 + 64 + 30;
                }
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (upperRightCloseButton.containsPoint(x, y))
                Game1.activeClickableMenu = this.customiser;

            foreach (ClickableNPCComponent component in  npcComponents)
            {
                if (component.containsPoint(x, y))
                { 
                    foreach(CharacterLayer layer in customiser.picture.characterLayers)
                        if (layer == component.layer)
                            targetLayer = layer;
                }
            }

            if (AddNPCButton.containsPoint(x, y))
                AddNPCButton.CallEvent();

            if (RemoveNPCButton.containsPoint(x, y))
                RemoveNPCButton.CallEvent();

            if(exportButton.containsPoint(x, y))
                exportButton.CallEvent();

            switcher.click(x, y, ref targetLayer);
            npcOffsetWheel.click(x, y, ref targetLayer.npcOffsetX, ref targetLayer.npcOffsetY);
            backgroundOffsetWheel.click(x, y, ref this.customiser.picture.background.offsetX, ref this.customiser.picture.background.offsetY);
            flipCheckbox.click(x, y, ref targetLayer.npcFlipped);
            layerSwitcher.click(x, y, ref targetLayer.layer, 0, customiser.picture.characterLayers.Count - 1);
            customiser.UpdatePreview();
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            AddNPCButton.PerformHover(x, y);
            RemoveNPCButton.PerformHover(x, y);
            exportButton.PerformHover(x, y);
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
            base.draw(b);
            AddNPCButton.draw(b);
            foreach (ClickableNPCComponent component in npcComponents)
            {
                float opacity = 0.3f;
                if (component.layer == targetLayer)
                    opacity = 1f;

                component.draw(b, Color.White * opacity, 1f);
            }
            customiser.drawTileSizeText(b);
            customiser.preview.draw(b);
            npcOffsetWheel.draw(b);
            backgroundOffsetWheel.draw(b);
            switcher.draw(b);
            flipCheckbox.draw(b);
            layerSwitcher.draw(b);
            RemoveNPCButton.draw(b);
            exportButton.draw(b);
            string frameText = $"{targetLayer.npcFrame + 1}/{targetLayer.npcFrameAmount}";
            string layerText = (targetLayer.layer + 1).ToString();
            Utility.drawTextWithShadow(b, layerText, Game1.smallFont, new Vector2((layerSwitcher.positionX + layerSwitcher.width / 2 - Game1.smallFont.MeasureString(layerText).X), layerSwitcher.positionY + 40), Game1.textColor, 0.8f);
            Utility.drawTextWithShadow(b, frameText, Game1.smallFont, new Vector2((switcher.positionX + switcher.width / 2 - Game1.smallFont.MeasureString(frameText).X), switcher.positionY + 40), Game1.textColor, 0.8f);
            drawMouse(b);
        }
    }
}
