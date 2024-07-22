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

        public string textureName;

        public Texture2D backgroundImage { get => _backgroundImage ?? ModEntry.instance.Helper.GameContent.Load<Texture2D>(textureName); }

        private Texture2D _backgroundImage;

        public string displayName;
        private Background(string displayName, int offsetX, int offsetY, Texture2D backgroundImage)
        {
            this.offsetX = offsetX;
            this.offsetY = offsetY;
            this._backgroundImage = backgroundImage;
            this.displayName = displayName;
        }

        public static Background Of(string displayName, int offsetX, int offsetY, Texture2D backgroundImage)
        {
            return new Background(displayName, offsetX, offsetY, backgroundImage);
        }

        public Background(string textureName, string displayName)
        {
            this.textureName = textureName;
            this.displayName = displayName;
        }

        public bool FitsInFrame(Frame frame)
        {
            return frame.spaceWidth <= this.backgroundImage.Width && frame.spaceHeight <= this.backgroundImage.Height;
        }
        public static Background GetDefaultBackground()
        {
            return new Background("Forest", 0, 5, ModEntry.backgroundImages["DefaultImg.Forest"].backgroundImage);
        }
    }
}
