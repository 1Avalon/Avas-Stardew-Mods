using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;

namespace ImprovedFallDebris
{
    public static class TextureUtils
    {
        // Methode zum Verschieben des Farbtons und Anpassen von Sättigung und Helligkeit einer Texture2D
        public static Texture2D ShiftHueSaturationAndBrightness(Texture2D texture, Vector3 adjustments)
        {
            float hueShift = adjustments.X; // Hue zwischen 0 und 1 (normalisiert)
            float saturationPercent = adjustments.Y; // Sättigung (-100 bis 100)
            float brightnessPercent = adjustments.Z; // Helligkeit (-100 bis 100)

            // GraphicsDevice wird für die Erstellung der neuen Textur benötigt
            GraphicsDevice graphicsDevice = Game1.graphics.GraphicsDevice;

            // Lese alle Pixel der Textur in ein Color-Array
            Color[] pixels = new Color[texture.Width * texture.Height];
            texture.GetData(pixels);

            // Verschiebe den Farbton, passe die Sättigung und Helligkeit jedes Pixels an
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = AdjustPixel(pixels[i], hueShift, saturationPercent, brightnessPercent);
            }

            // Erstelle eine neue Texture2D und setze die veränderten Pixel
            Texture2D shiftedTexture = new Texture2D(graphicsDevice, texture.Width, texture.Height);
            shiftedTexture.SetData(pixels);

            return shiftedTexture;
        }

        // Methode zum Verschieben des Farbtons, Anpassen der Sättigung und der Helligkeit eines Pixels
        private static Color AdjustPixel(Color color, float hueShift, float saturationPercent, float brightnessPercent)
        {
            // RGB zu HSV umwandeln
            float h, s, v;
            RGBToHSV(color, out h, out s, out v);

            // Farbton ändern (h ist im Bereich 0 bis 1, hueShift ist ebenfalls normalisiert)
            h += hueShift;
            if (h > 1) h -= 1;
            if (h < 0) h += 1;

            // Sättigung anpassen
            if (saturationPercent < 0)
            {
                // Sättigung reduzieren (in Richtung Grau)
                float reductionFactor = 1 + saturationPercent / 100f;
                s *= reductionFactor;
            }
            else if (saturationPercent > 0)
            {
                // Sättigung erhöhen
                float increaseFactor = 1 + saturationPercent / 100f;
                s *= increaseFactor;
            }

            // Sättigung muss im Bereich von 0 bis 1 bleiben
            s = MathHelper.Clamp(s, 0, 1);

            // Helligkeit (Value) anpassen
            if (brightnessPercent < 0)
            {
                // Helligkeit verringern
                float reductionFactor = 1 + brightnessPercent / 100f;
                v *= reductionFactor;
            }
            else if (brightnessPercent > 0)
            {
                // Helligkeit erhöhen
                float increaseFactor = 1 + brightnessPercent / 100f;
                v *= increaseFactor;
            }

            // Helligkeit (Value) muss im Bereich von 0 bis 1 bleiben
            v = MathHelper.Clamp(v, 0, 1);

            // Zurück zu RGB mit dem neuen Farbton, der angepassten Sättigung und Helligkeit
            Color shiftedColor = HSVToRGB(h, s, v);
            shiftedColor.A = color.A; // Alpha-Wert beibehalten

            return shiftedColor;
        }

        // Methode zur Umwandlung von RGB zu HSV (Hue, Saturation, Value)
        private static void RGBToHSV(Color color, out float h, out float s, out float v)
        {
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;

            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));

            v = max;

            float delta = max - min;

            if (max == 0)
            {
                s = 0;
                h = 0; // undefiniert, achromatisch
            }
            else
            {
                s = delta / max;

                if (r == max)
                    h = (g - b) / delta; // zwischen Gelb & Magenta
                else if (g == max)
                    h = 2 + (b - r) / delta; // zwischen Cyan & Gelb
                else
                    h = 4 + (r - g) / delta; // zwischen Magenta & Cyan

                h /= 6;
                if (h < 0)
                    h += 1;
            }
        }

        // Methode zur Umwandlung von HSV zurück zu RGB
        private static Color HSVToRGB(float h, float s, float v)
        {
            int i = (int)Math.Floor(h * 6);
            float f = h * 6 - i;
            float p = v * (1 - s);
            float q = v * (1 - f * s);
            float t = v * (1 - (1 - f) * s);

            float r, g, b;
            switch (i % 6)
            {
                case 0: r = v; g = t; b = p; break;
                case 1: r = q; g = v; b = p; break;
                case 2: r = p; g = v; b = t; break;
                case 3: r = p; g = q; b = v; break;
                case 4: r = t; g = p; b = v; break;
                case 5: r = v; g = p; b = q; break;
                default: r = 1; g = 1; b = 1; break; // fallback case (sollte nie passieren)
            }

            return new Color(r, g, b);
        }

        // Methode zum Ausschneiden eines Rechtecks aus einer Texture2D
        public static Texture2D CropTexture(Texture2D texture, Rectangle sourceRectangle)
        {
            GraphicsDevice graphicsDevice = Game1.graphics.GraphicsDevice;

            // Sicherstellen, dass das Rechteck innerhalb der Grenzen der Textur liegt
            if (sourceRectangle.X < 0 || sourceRectangle.Y < 0 ||
                sourceRectangle.X + sourceRectangle.Width > texture.Width ||
                sourceRectangle.Y + sourceRectangle.Height > texture.Height)
            {
                throw new ArgumentException("Das angegebene Rechteck liegt außerhalb der Grenzen der Textur.");
            }

            // Erstellt ein Array für die Pixel des ausgeschnittenen Bereichs
            Color[] data = new Color[sourceRectangle.Width * sourceRectangle.Height];

            // Hole die Pixel-Daten aus dem angegebenen Bereich der Textur
            texture.GetData(0, sourceRectangle, data, 0, data.Length);

            // Erstelle eine neue Texture2D für das ausgeschnittene Bild
            Texture2D croppedTexture = new Texture2D(graphicsDevice, sourceRectangle.Width, sourceRectangle.Height);

            // Setze die Pixel-Daten auf die neue Textur
            croppedTexture.SetData(data);

            return croppedTexture;
        }
    }
}
