﻿using DynamicNPCPaintings.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using Background = DynamicNPCPaintings.Framework.Background;
using StardewValley.Menus;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Objects;

namespace DynamicNPCPaintings
{
    public static class TextureHelper
    {
        public static Texture2D FlipTextureHorizontally(Texture2D originalTexture)
        {
            int width = originalTexture.Width;
            int height = originalTexture.Height;

            // Lade die Pixel-Daten des Originals
            Color[] originalData = new Color[width * height];
            originalTexture.GetData(originalData);

            // Erstelle ein Array für die Pixel-Daten der geflippten Textur
            Color[] flippedData = new Color[width * height];

            // Horizontales Flippen der Pixel
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    flippedData[y * width + (width - 1 - x)] = originalData[y * width + x];
                }
            }

            // Erstelle eine neue Texture2D und setze die geflippten Pixel-Daten
            Texture2D flippedTexture = new Texture2D(Game1.graphics.GraphicsDevice, width, height);
            flippedTexture.SetData(flippedData);

            return flippedTexture;
        }

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
        public static Texture2D GetCharacterFrame(Texture2D spritesheet, int frame, bool flipped = false)
        {
            int xOffset = (frame % 4) * 16;
            int yOffset = frame / 4 * 32;

            Rectangle newBounds = new Rectangle(xOffset, yOffset, 16, 32);

            Texture2D croppedTexture = new Texture2D(Game1.graphics.GraphicsDevice, newBounds.Width, newBounds.Height);
            Color[] data = new Color[newBounds.Width * newBounds.Height];
            spritesheet.GetData(0, newBounds, data, 0, newBounds.Width * newBounds.Height);
            croppedTexture.SetData(data);
            Debug.WriteLine(croppedTexture.Bounds.Size);
            return flipped ? FlipTextureHorizontally(croppedTexture) : croppedTexture;
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
        public static Texture2D DrawCharacterOnBackground(Texture2D backgroundTexture, Texture2D characterTexture, Vector2 characterPosition, int minimumX, int minimumY, int maximumX, int maximumY)
        {
            GraphicsDevice graphicsDevice = Game1.graphics.GraphicsDevice;
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
        }// Texture2D frameTexture, Texture2D backgroundTexture, int startX, int startY, int endX, int endY, int offsetX, int offsetY
        public static Texture2D BackgroundWithFrame(Frame frame, Background background)
        {
            GraphicsDevice graphicsDevice = Game1.graphics.GraphicsDevice;

            // Berechne die maximal zulässigen Offsets und stelle sicher, dass sie nicht kleiner als null sind
            background.offsetX = Math.Max(0, Math.Min(background.offsetX, background.backgroundImage.Width - (frame.endX - frame.startX)));
            background.offsetY = Math.Max(0, Math.Min(background.offsetY, background.backgroundImage.Height - (frame.endY - frame.startY)));

            // Erstelle eine neue Texture2D mit der gleichen Größe wie der Rahmen
            Texture2D resultTexture = new Texture2D(graphicsDevice, frame.frameTexture.Width, frame.frameTexture.Height);

            // Lade die Pixel-Daten des Rahmens
            Color[] frameData = new Color[frame.frameTexture.Width * frame.frameTexture.Height];
            frame.frameTexture.GetData(frameData);

            // Lade die Pixel-Daten des Hintergrunds
            Color[] backgroundData = new Color[background.backgroundImage.Width * background.backgroundImage.Height];
            background.backgroundImage.GetData(backgroundData);

            // Erstelle ein Array für die Pixel-Daten des Ergebnisses
            Color[] resultData = new Color[frame.frameTexture.Width * frame.frameTexture.Height];

            // Kopiere den Rahmen in das Ergebnis
            for (int y = 0; y < frame.frameTexture.Height; y++)
            {
                for (int x = 0; x < frame.frameTexture.Width; x++)
                {
                    resultData[y * frame.frameTexture.Width + x] = frameData[y * frame.frameTexture.Width + x];
                }
            }

            // Zeichne den Hintergrund innerhalb des Rahmens
            for (int y = frame.startY; y < frame.endY && y < frame.frameTexture.Height; y++)
            {
                for (int x = frame.startX; x < frame.endX && x < frame.frameTexture.Width; x++)
                {
                    // Berechne die Position im Hintergrundbild unter Berücksichtigung des Offsets
                    int backgroundX = x - frame.startX + background.offsetX;
                    int backgroundY = y - frame.startY + background.offsetY;

                    // Nur zeichnen, wenn die Hintergrundkoordinaten im gültigen Bereich liegen
                    if (backgroundX >= 0 && backgroundX < background.backgroundImage.Width && backgroundY >= 0 && backgroundY < background.backgroundImage.Height)
                    {
                        // Setze das Pixel in den Ergebnisdaten
                        resultData[y * frame.frameTexture.Width + x] = backgroundData[backgroundY * background.backgroundImage.Width + backgroundX];
                    }
                }
            }

            // Setze die Pixel-Daten in die resultierende Textur
            resultTexture.SetData(resultData);

            return resultTexture;
        }

        public static void ExportToPainting(Picture picture)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            if (!Directory.Exists(Path.Combine(ModEntry.instance.Helper.DirectoryPath, "pictures", Constants.SaveFolderName)))
                Directory.CreateDirectory(Path.Combine(ModEntry.instance.Helper.DirectoryPath, "pictures", Constants.SaveFolderName));

            int number = 1;
            var file = Path.Combine(ModEntry.instance.Helper.DirectoryPath, "pictures", Constants.SaveFolderName, $"Picture_{number}.png");
            while (File.Exists(file))
            {
                file = Path.Combine(ModEntry.instance.Helper.DirectoryPath, "pictures", Constants.SaveFolderName, $"Picture_{++number}.png");
            }


            Stream stream = File.Create(file);

            picture.GetTexture().SaveAsPng(stream, picture.frame.frameTexture.Width, picture.frame.frameTexture.Height);
            stream.Close();

            Game1.addHUDMessage(new HUDMessage("Successfully saved the painting", 1));


            string uniqueID = $"AvalonMFX.CustomNPCPaintings.Picture_{number}";

            if (!ModEntry.dataManager.FurnitureData.ContainsKey(uniqueID))
            {
                string uniqueTextureName = $"{uniqueID}_IMG";
                ModEntry.dataManager.FurnitureData.Add(uniqueID, $"AvalonMFX.Picture_{number}/painting/{picture.tileWidth} {picture.tileHeight}/{picture.tileWidth} {picture.tileHeight}/1/0/-1/Picture/0/{uniqueTextureName}");
                ModEntry.dataManager.TextureData.Add(uniqueTextureName, file);
            }
            ModEntry.instance.Helper.GameContent.InvalidateCache("Data/Furniture");
            Item obj = new StardewValley.Object(uniqueID, 1);
            Furniture furniture = new Furniture(uniqueID, Vector2.Zero);
            List<Item> items = new List<Item>()
            {
                furniture
            };
            Game1.activeClickableMenu = new ItemGrabMenu(items);
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