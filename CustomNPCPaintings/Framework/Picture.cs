using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using Microsoft.Xna.Framework;
using CustomNPCPaintings.Framework;

namespace DynamicNPCPaintings.Framework
{
    public class Picture
    {
        public Frame frame;

        public Background background;

        public List<CharacterLayer> characterLayers;

        public Color backgroundColor;

        public int tileWidth { get => frame.frameTexture.Width / 16; }

        public int tileHeight {  get => frame.frameTexture.Height / 16; }

        public Picture(Frame frame, Background background, List<CharacterLayer> characterLayers)
        {
            this.frame = frame;
            this.background = background;
            this.characterLayers = characterLayers;
        }

        public static Picture GetDefaultPicture()
        {
            List<CharacterLayer> layers = new List<CharacterLayer>()
            {
                new CharacterLayer(Game1.getCharacterFromName("Haley"), Background.GetDefaultBackground(), 0)
            };
            return new Picture(Frame.GetDefaultFrame(), Background.GetDefaultBackground(), layers);
        }
        public Texture2D GetTexture()
        {
            Texture2D frameAndBackground = backgroundColor.A == 0 ? TextureHelper.BackgroundWithFrame(frame, background) : TextureHelper.BackgroundWithFrame(frame, backgroundColor);

            List<CharacterLayer> orderedLayerList = characterLayers.OrderBy(o => o.layer).ToList();
            foreach (CharacterLayer characterLayer in orderedLayerList)
            {
                Texture2D characterTexture = TextureHelper.GetCharacterFrame(characterLayer, characterLayer.npcFrame, characterLayer.isFarmer ? 22 : 4, characterLayer.npcFlipped);
                frameAndBackground = TextureHelper.DrawCharacterOnBackground(frameAndBackground, characterTexture, new Vector2(characterLayer.npcOffsetX, characterLayer.npcOffsetY), frame.startX, frame.startY, frame.endX, frame.endY);
            }
            return frameAndBackground;
            //Texture2D characterTexture = TextureHelper.GetCharacterFrame(target, npcFrame, npcFlipped);
            //return TextureHelper.DrawCharacterOnBackground(frameAndBackground, characterTexture, new Vector2(npcOffsetX, npcOffsetY), frame.startX, frame.startY, frame.endX, frame.endY);
        }
    }
}
