using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicNPCPaintings.Framework
{
    public class Background
    {
        public int offsetX;

        public int offsetY;

        public Texture2D backgroundImage;

        public Background(int offsetX, int offsetY, Texture2D backgroundImage)
        {
            this.offsetX = offsetX;
            this.offsetY = offsetY;
            this.backgroundImage = backgroundImage;
        }

        public static Background GetDefaultBackground()
        {
            return new Background(0, 5, ModEntry.background);
        }
    }
}
