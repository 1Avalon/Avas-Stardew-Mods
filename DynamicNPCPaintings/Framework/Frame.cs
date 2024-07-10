using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicNPCPaintings.Framework
{
    public class Frame
    {
        public int startX;

        public int startY;

        public int endX;

        public int endY;

        public Texture2D frameTexture;

        public Frame(int startX, int startY, int endX, int endY, Texture2D frameTexture)
        {
            this.startX = startX;
            this.startY = startY;
            this.endX = endX;
            this.endY = endY;
            this.frameTexture = frameTexture;
        }

        public static Frame GetDefaultFrame()
        {
            return new Frame(4, 5, 45, 25, ModEntry.frame);
        }
    }
}
