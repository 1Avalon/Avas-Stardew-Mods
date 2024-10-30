using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicNPCPaintings.Framework;
using StardewValley;

namespace CustomNPCPaintings.Framework
{
    public class CharacterLayer
    {

        public int layer;

        public NPC target;
        public int npcFrameAmount { get => (target.Sprite.Texture.Width / target.Sprite.SpriteWidth) * (target.Sprite.Texture.Height / target.Sprite.SpriteHeight); }

        public int npcFrame;

        public int npcOffsetX;

        public int npcOffsetY;

        public bool npcFlipped = false;

        public CharacterLayer(NPC target, DynamicNPCPaintings.Framework.Background background, int npcFrame) 
        { 
            this.target = target;
            this.npcFrame = npcFrame;
            this.npcOffsetX = background.backgroundImage.Width / 2; ;
            this.npcOffsetY = 0;
            layer = 0;
        }
    }
}
