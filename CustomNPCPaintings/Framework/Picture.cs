using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using Microsoft.Xna.Framework;

namespace DynamicNPCPaintings.Framework
{
    public class Picture
    {
        public Frame frame;

        public Background background;

        public NPC target;

        public int npcFrame;

        public int npcOffsetX;

        public int npcOffsetY;

        public bool npcFlipped = false;

        public int tileWidth { get => frame.frameTexture.Width / 16; }

        public int tileHeight {  get => frame.frameTexture.Height / 16; }

        public int npcFrameAmount { get => (target.Sprite.Texture.Width / target.Sprite.SpriteWidth) * (target.Sprite.Texture.Height / target.Sprite.SpriteHeight); }

        public Picture(Frame frame, Background background, NPC target, int npcFrame)
        {
            this.frame = frame;
            this.background = background;
            this.target = target;
            this.npcFrame = npcFrame;
            this.npcOffsetX = background.backgroundImage.Width / 2;
            this.npcOffsetY = 0;
        }

        public static Picture GetDefaultPicture()
        {
            return new Picture(Frame.GetDefaultFrame(), Background.GetDefaultBackground(), Game1.getCharacterFromName("Haley"), 0);
        }
        public Texture2D GetTexture()
        {
            Texture2D frameAndBackground = TextureHelper.BackgroundWithFrame(frame, background);
            Texture2D characterTexture = TextureHelper.GetCharacterFrame(target, npcFrame, npcFlipped);
            return TextureHelper.DrawCharacterOnBackground(frameAndBackground, characterTexture, new Vector2(npcOffsetX, npcOffsetY), frame.startX, frame.startY, frame.endX, frame.endY);
        }
    }
}
