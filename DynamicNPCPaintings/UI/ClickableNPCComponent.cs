using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using Microsoft.Xna.Framework.Graphics;
using WeddingPhoto;

namespace DynamicNPCPaintings.UI
{
    public class ClickableNPCComponent : ClickableTextureComponent
    {
        public NPC npc;

        public OptionsButton npcListButton;
        public ClickableNPCComponent(Rectangle bounds, NPC npc, Rectangle sourceRect, float scale) : base(bounds, null, sourceRect, scale)
        {
            this.npc = npc;
            texture = TextureHelper.GetCharacterFrame(npc.Sprite.Texture, 1);
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
