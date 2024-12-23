using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using Microsoft.Xna.Framework.Graphics;
using CustomNPCPaintings.Framework;

namespace DynamicNPCPaintings.UI.UIElements
{
    public class ClickableNPCComponent : ClickableTextureComponent
    {
        public CharacterLayer layer;

        public OptionsButton npcListButton;

    public ClickableNPCComponent(Rectangle bounds, CharacterLayer layer, Rectangle sourceRect, float scale) : base(bounds, null, sourceRect, scale)
        {
            this.layer = layer;
            texture = TextureHelper.GetCharacterFrame(layer, 1, 4);
        }

        public void CropTexture(Rectangle cropArea)
        {
            TextureHelper.CropTexture(texture, cropArea);
        }
        public override void draw(SpriteBatch b)
        {
            base.draw(b);
        }
    }
}
