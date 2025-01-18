using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreSaveInformation
{
    public static class TextureHelper
    {
        public static int FindFirstNonTransparentPixelY(Texture2D texture)
        {
            int width = texture.Width;
            int height = texture.Height;

            // Lade die Pixel-Daten des Textur
            Color[] textureData = new Color[width * height];
            texture.GetData(textureData);

            // Suche den ersten nicht-transparenten Pixel in Y-Richtung
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = textureData[y * width + x];
                    if (pixel.A != 0)
                    {
                        return y;
                    }
                }
            }

            // Wenn kein nicht-transparenter Pixel gefunden wird, gib -1 zurück
            return -1;
        }
    }
}
