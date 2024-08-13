using DynamicNPCPaintings.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace CustomNPCPaintings.Framework
{
    public class NetworkPictureData
    {
        public Color[] colorData;

        public int width;

        public int height;

        public int tileWidth;

        public int tileHeight;

        public NetworkPictureData(Color[] colorData, int width, int height, int tileWidth, int tileHeight)
        {
            this.colorData = colorData;
            this.width = width;
            this.height = height;
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
        }

        public Texture2D GetTexture()
        {
            Texture2D texture = new Texture2D(Game1.graphics.GraphicsDevice, width, height);
            texture.SetData(colorData);
            return texture;
        }
    }
}
