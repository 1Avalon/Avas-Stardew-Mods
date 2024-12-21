using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicNPCPaintings;
using DynamicNPCPaintings.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace CustomNPCPaintings.Framework
{
    public class CharacterLayer
    {

        public int layer;

        private NPC target;

        private Farmer farmer;
        public int npcFrameAmount { get
            {
                if (target != null)
                    return (target.Sprite.Texture.Width / target.Sprite.SpriteWidth) * (target.Sprite.Texture.Height / target.Sprite.SpriteHeight);

                return 126;
            } }

        public int npcFrame;

        public int npcOffsetX;

        public int npcOffsetY;

        public bool npcFlipped = false;

        public bool isFarmer { get => farmer != null; }

        public string Name { get => target == null ? farmer.Name : target.Name; }

        public string DisplayName { get => target == null ? farmer.displayName : target.displayName; }

        public int SpriteWidth { get => target == null ? 16 : target.Sprite.SpriteWidth; }

        public int SpriteHeight { get => target == null ? 32 : target.Sprite.SpriteHeight; }
        public Texture2D Texture { get => target == null ? FarmerTexture : target.Sprite.Texture; }

        private Texture2D FarmerTexture { get
            {
                return ModEntry.farmerSpriteSheet;
            } }

        public CharacterLayer(Farmer farmer, DynamicNPCPaintings.Framework.Background background, int npcFrame) 
        {
            this.farmer = farmer;
            this.npcFrame = npcFrame;
            this.npcOffsetX = background.backgroundImage.Width / 2; ;
            this.npcOffsetY = 0;
            layer = 0;
        }
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
