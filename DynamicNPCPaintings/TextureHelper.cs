using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeddingPhoto
{
    public static class TextureHelper
    {
        public static Texture2D GetCharacterFrame(Texture2D spritesheet, int frame)
        {
            int xOffset = (frame % 4) * 16;
            int yOffset = frame / 4 * 32;

            Rectangle newBounds = new Rectangle(xOffset, yOffset, 16, 32);

            Texture2D croppedTexture = new Texture2D(Game1.graphics.GraphicsDevice, newBounds.Width, newBounds.Height);
            Color[] data = new Color[newBounds.Width * newBounds.Height];
            spritesheet.GetData(0, newBounds, data, 0, newBounds.Width * newBounds.Height);
            croppedTexture.SetData(data);
            Debug.WriteLine(croppedTexture.Bounds.Size);
            return croppedTexture;
        }

        public static Texture2D CropTexture(Texture2D texture, Rectangle cropArea)
        {
            // Erstellen eines Arrays, das die Pixel der zugeschnittenen Textur enthält
            Color[] data = new Color[cropArea.Width * cropArea.Height];

            // Auslesen der Pixel aus der Ursprungs-Textur
            texture.GetData(0, cropArea, data, 0, data.Length);

            // Erstellen einer neuen Textur für das zugeschnittene Bild
            Texture2D croppedTexture = new Texture2D(texture.GraphicsDevice, cropArea.Width, cropArea.Height);

            // Setzen der Pixel auf die neue Textur
            croppedTexture.SetData(data);

            return croppedTexture;
        }
        public static Texture2D DrawCharacterOnBackground(GraphicsDevice graphicsDevice, Texture2D backgroundTexture, Texture2D characterTexture, Vector2 characterPosition, int minimumX, int minimumY, int maximumX, int maximumY)
        {
            // Erstellen des Ziel-Texturen-Speichers mit der Größe des Hintergrunds
            Texture2D resultTexture = new Texture2D(graphicsDevice, backgroundTexture.Width, backgroundTexture.Height);

            // Daten der Texturen lesen
            Color[] backgroundPixels = new Color[backgroundTexture.Width * backgroundTexture.Height];
            backgroundTexture.GetData(backgroundPixels);

            Color[] characterPixels = new Color[characterTexture.Width * characterTexture.Height];
            characterTexture.GetData(characterPixels);

            // Kopieren des Hintergrunds auf das Ziel-Texturen-Objekt
            Color[] resultPixels = new Color[backgroundPixels.Length];
            backgroundPixels.CopyTo(resultPixels, 0);

            // Position und Grenzen der Figur auf dem Hintergrund
            int offsetX = (int)characterPosition.X;
            int offsetY = (int)characterPosition.Y;
            int minX = Math.Max(0, minimumX - offsetX);
            int maxX = Math.Min(characterTexture.Width, maximumX - offsetX);
            int minY = Math.Max(0, minimumY - offsetY);
            int maxY = Math.Min(characterTexture.Height, maximumY - offsetY);

            // Zeichnen der undurchsichtigen Pixel der Figur auf das Ziel-Texturen-Objekt
            for (int y = minY; y < maxY; y++)
            {
                for (int x = minX; x < maxX; x++)
                {
                    // Überprüfen, ob sich der Pixel innerhalb der Grenzen des Hintergrunds befindet
                    int destX = x + offsetX;
                    int destY = y + offsetY;
                    if (destX >= 0 && destX < backgroundTexture.Width && destY >= 0 && destY < backgroundTexture.Height)
                    {
                        // Überprüfen, ob der Pixel undurchsichtig ist
                        int sourceIndex = x + y * characterTexture.Width;
                        if (characterPixels[sourceIndex].A > 0)
                        {
                            // Kopieren des Pixels von der Figur auf das Ziel-Texturen-Objekt
                            int destIndex = destX + destY * backgroundTexture.Width;
                            resultPixels[destIndex] = characterPixels[sourceIndex];
                        }
                    }
                }
            }

            // Daten in das Ziel-Texturen-Objekt schreiben
            resultTexture.SetData(resultPixels);

            return resultTexture;
        }
        public static Texture2D BackgroundWithFrame(Texture2D frameTexture, Texture2D backgroundTexture, int startX, int startY, int endX, int endY, int offsetX, int offsetY, GraphicsDevice graphicsDevice)
        {
            // Berechne die maximal zulässigen Offsets
            if (offsetX > backgroundTexture.Width - (endX - startX))
            {
                offsetX = backgroundTexture.Width - (endX - startX);
            }

            if (offsetY > backgroundTexture.Height - (endY - startY))
            {
                offsetY = backgroundTexture.Height - (endY - startY);
            }

            // Erstelle eine neue Texture2D mit der gleichen Größe wie der Rahmen
            Texture2D resultTexture = new Texture2D(graphicsDevice, frameTexture.Width, frameTexture.Height);

            // Lade die Pixel-Daten des Rahmens
            Color[] frameData = new Color[frameTexture.Width * frameTexture.Height];
            frameTexture.GetData(frameData);

            // Lade die Pixel-Daten des Hintergrunds
            Color[] backgroundData = new Color[backgroundTexture.Width * backgroundTexture.Height];
            backgroundTexture.GetData(backgroundData);

            // Erstelle ein Array für die Pixel-Daten des Ergebnisses
            Color[] resultData = new Color[frameTexture.Width * frameTexture.Height];

            // Kopiere den Rahmen in das Ergebnis
            for (int y = 0; y < frameTexture.Height; y++)
            {
                for (int x = 0; x < frameTexture.Width; x++)
                {
                    resultData[y * frameTexture.Width + x] = frameData[y * frameTexture.Width + x];
                }
            }

            // Zeichne den Hintergrund innerhalb des Rahmens
            for (int y = startY; y < endY && y < frameTexture.Height; y++)
            {
                for (int x = startX; x < endX && x < frameTexture.Width; x++)
                {
                    // Berechne die Position im Hintergrundbild unter Berücksichtigung des Offsets
                    int backgroundX = x - startX + offsetX;
                    int backgroundY = y - startY + offsetY;

                    // Nur zeichnen, wenn die Hintergrundkoordinaten im gültigen Bereich liegen
                    if (backgroundX >= 0 && backgroundX < backgroundTexture.Width && backgroundY >= 0 && backgroundY < backgroundTexture.Height)
                    {
                        // Setze das Pixel in den Ergebnisdaten
                        resultData[y * frameTexture.Width + x] = backgroundData[backgroundY * backgroundTexture.Width + backgroundX];
                    }
                }
            }

            // Setze die Pixel-Daten in die resultierende Textur
            resultTexture.SetData(resultData);

            return resultTexture;
        }
        public static Texture2D DrawTextureOnTexture(GraphicsDevice graphicsDevice, Texture2D backgroundTexture, Texture2D overlayTexture, Vector2 position)
        {
            // Erstellen eines neuen Texture2D-Objekts mit der Größe des Hintergrundbildes
            Texture2D resultTexture = new Texture2D(graphicsDevice, backgroundTexture.Width, backgroundTexture.Height);

            // Daten der Texturen lesen
            Color[] backgroundPixels = new Color[backgroundTexture.Width * backgroundTexture.Height];
            backgroundTexture.GetData(backgroundPixels);

            Color[] overlayPixels = new Color[overlayTexture.Width * overlayTexture.Height];
            overlayTexture.GetData(overlayPixels);

            // Kopieren der Hintergrundtextur in das Ergebnis
            Color[] resultPixels = new Color[backgroundPixels.Length];
            backgroundPixels.CopyTo(resultPixels, 0);

            // Berechnen der Position, an der die Overlay-Textur gezeichnet werden soll
            int offsetX = (int)position.X;
            int offsetY = (int)position.Y;

            // Zeichnen der Overlay-Textur über die Hintergrundtextur
            for (int y = 0; y < overlayTexture.Height; y++)
            {
                for (int x = 0; x < overlayTexture.Width; x++)
                {
                    int index = x + y * overlayTexture.Width;
                    int destX = x + offsetX;
                    int destY = y + offsetY;

                    // Überprüfen, ob sich der Ziel-Pixel innerhalb der Grenzen des Hintergrunds befindet
                    if (destX >= 0 && destX < backgroundTexture.Width && destY >= 0 && destY < backgroundTexture.Height)
                    {
                        // Überprüfen, ob der Pixel der Overlay-Textur undurchsichtig ist
                        if (overlayPixels[index].A > 0)
                        {
                            // Zeichnen des Pixels der Overlay-Textur auf das Ergebnis
                            int destIndex = destX + destY * backgroundTexture.Width;
                            resultPixels[destIndex] = overlayPixels[index];
                        }
                    }
                }
            }

            // Daten in das Ergebnis-Texturen-Objekt schreiben
            resultTexture.SetData(resultPixels);

            return resultTexture;
        }
    }
}