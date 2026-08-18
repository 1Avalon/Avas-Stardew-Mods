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
using StardewValley.Menus;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Objects;
using xTile.Layers;

namespace NPCSpriteCreator
{
    public static class TextureHelper
    {
        public static void RenderFarmer(SpriteBatch b, Farmer farmer, int frame, Vector2 position)
        {
            int xOffset = (frame % 6) * 16;
            int yOffset = frame / 6 * 32;

            int facingDirection = (frame / 6 + 2) % 4;

            if (facingDirection == 3)
                facingDirection = 1;

            //Sheet is inconsistent and I have to do it manually... kms

            switch (frame)
            {
                case 10:
                case 16:
                case 18:
                case 19:
                case 42:
                case 74:
                case 75:
                case 78:
                case 79:
                case 107:
                    facingDirection = 2;
                    break;
                case 11:
                case 17:
                case 20:
                case 21:
                case 60:
                case 61:
                case 72:
                case 73:
                case 89:
                case 97:
                case 101:
                    facingDirection = 1;
                    break;
                case 22:
                case 23:
                case 44:
                case 46:
                case 53:
                case 71:
                case 76:
                case 77:
                case 82:
                case 83:
                    facingDirection = 0;
                    break;
            }

            if (frame >= 24 && frame <= 29)
                facingDirection = 2;

            else if (frame >= 25 && frame <= 30) //zwischen 25 und 30
                facingDirection = 1;
            else if (frame >= 48 && frame <= 52) //zwischen 48 und 52
                facingDirection = 1;
            else if (frame >= 54 && frame <= 57) //zwischen 54 und 56
                facingDirection = 2;
            else if (frame >= 66 && frame <= 70)
                facingDirection = 2;
            else if (frame >= 84 && frame <= 88)
                facingDirection = 2;
            else if (frame >= 90 && frame <= 96)
                facingDirection = 2;
            else if (frame >= 102 && frame <= 105)
                facingDirection = 2;

            Rectangle sourceRect = new Rectangle(xOffset, yOffset, 16, 32);

            farmer.FarmerRenderer.draw(b, new FarmerSprite.AnimationFrame(0, 0, secondaryArm: false, flip: false), frame, sourceRect, position, Vector2.Zero, 0.8f, facingDirection, Color.White, 0f, 1f, farmer);
        }

        public static void ExportSheet()
        {
            if (!Directory.Exists(Path.Combine(ModEntry.instance.Helper.DirectoryPath, "sheets", Constants.SaveFolderName)))
                Directory.CreateDirectory(Path.Combine(ModEntry.instance.Helper.DirectoryPath, "sheets", Constants.SaveFolderName));

            int file_number = 1;
            var file = Path.Combine(ModEntry.instance.Helper.DirectoryPath, "sheets", Constants.SaveFolderName, $"{Constants.SaveFolderName}_Picture_{file_number}.png");
            while (File.Exists(file))
            {
                file = Path.Combine(ModEntry.instance.Helper.DirectoryPath, "sheets", Constants.SaveFolderName, $"{Constants.SaveFolderName}_Picture_{++file_number}.png");
            }


            Stream stream = File.Create(file);

            ModEntry.resultTexture.SaveAsPng(stream, ModEntry.resultTexture.Width, ModEntry.resultTexture.Height);
            stream.Close();
            Game1.addHUDMessage(new HUDMessage("Successfully saved painting locally", 1));
        }

        public static void CopyFrameToNewSpritesheet(
             Texture2D sourceSheet,
             Texture2D destinationSheet,
             int sourceFrameIndex,
             int targetFrameIndex,
             bool flipHorizontally = false)  // optionaler Parameter für horizontalen Flip
        {
            const int frameWidth = 16;
            const int frameHeight = 32;
            const int sourceColumns = 22;
            const int destinationColumns = 4;

            // Quelle: Position im alten Sheet
            int sourceX = (sourceFrameIndex % sourceColumns) * frameWidth;
            int sourceY = (sourceFrameIndex / sourceColumns) * frameHeight;
            Rectangle sourceRect = new Rectangle(sourceX, sourceY, frameWidth, frameHeight);

            // Ziel: Position im neuen Sheet
            int targetX = (targetFrameIndex % destinationColumns) * frameWidth;
            int targetY = (targetFrameIndex / destinationColumns) * frameHeight;
            Rectangle targetRect = new Rectangle(targetX, targetY, frameWidth, frameHeight);

            // Pixeldaten vom SourceFrame holen
            Color[] frameData = new Color[frameWidth * frameHeight];
            sourceSheet.GetData(0, sourceRect, frameData, 0, frameData.Length);

            // Horizontal flippen falls gewünscht
            if (flipHorizontally)
            {
                Color[] flippedData = new Color[frameWidth * frameHeight];
                for (int y = 0; y < frameHeight; y++)
                {
                    for (int x = 0; x < frameWidth; x++)
                    {
                        // Pixel aus frameData mit horizontal umgedrehter X-Koordinate übernehmen
                        flippedData[y * frameWidth + x] = frameData[y * frameWidth + (frameWidth - 1 - x)];
                    }
                }
                frameData = flippedData;
            }

            // Bestehende Daten im Ziel holen, ändern und zurückschreiben
            Color[] destinationData = new Color[destinationSheet.Width * destinationSheet.Height];
            destinationSheet.GetData(destinationData);

            // Einzelne Pixel im Zielbild ersetzen
            for (int y = 0; y < frameHeight; y++)
            {
                for (int x = 0; x < frameWidth; x++)
                {
                    int destIndex = (targetY + y) * destinationSheet.Width + (targetX + x);
                    int srcIndex = y * frameWidth + x;

                    destinationData[destIndex] = frameData[srcIndex];
                }
            }

            // Neue Daten zurück ins Zielbild schreiben
            destinationSheet.SetData(destinationData);
        }


        private const int FarmerSpritesPerRow = 22;
        private const int FarmerFrameCount = 126;
        private const int FarmerDrawWidth = 64; // tatsächliche Zeichengröße des Farmers (16 * scaledPixelZoom 4), pre-scale
        private const int FarmerDrawHeight = 128; // tatsächliche Zeichengröße des Farmers (32 * scaledPixelZoom 4), pre-scale
        private const int FarmerWidthHeadroom = 64; // Puffer links+rechts vom Charakter beim Zeichnen (z.B. Werkzeug-Schwung), fliegt vor dem finalen Packen wieder raus
        private const int FarmerCellWidth = FarmerDrawWidth + FarmerWidthHeadroom;
        private const int FarmerHeadroom = 64; // Puffer oberhalb des Charakters beim Zeichnen (z.B. Hut/Frisur), fliegt vor dem finalen Packen wieder raus
        private const int FarmerCellHeight = FarmerDrawHeight + FarmerHeadroom;
        private const float FarmerOutputScale = 0.25f; // 64x128 (nativ gezeichnet) -> 16x32 im Endergebnis
        private const int FarmerCaptureMargin = 32; // Puffer, falls ein Frame über seine Zelle hinausragt (z.B. Werkzeug-Schwung)

        public static Texture2D CaptureFarmerSpriteSheet(Farmer farmer)
        {
            GraphicsDevice graphicsDevice = Game1.graphics.GraphicsDevice;

            int rows = (int)Math.Ceiling(FarmerFrameCount / (float)FarmerSpritesPerRow);
            int gridWidth = FarmerSpritesPerRow * FarmerCellWidth;
            int gridHeight = rows * FarmerCellHeight;
            int targetWidth = gridWidth + FarmerCaptureMargin * 2;
            int targetHeight = gridHeight + FarmerCaptureMargin * 2;

            RenderTargetBinding[] previousTargets = graphicsDevice.GetRenderTargets();

            using RenderTarget2D renderTarget = new RenderTarget2D(graphicsDevice, targetWidth, targetHeight);
            graphicsDevice.SetRenderTarget(renderTarget);
            graphicsDevice.Clear(Color.Transparent);

            using (SpriteBatch spriteBatch = new SpriteBatch(graphicsDevice))
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
                for (int i = 0; i < FarmerFrameCount; i++)
                {
                    int column = i % FarmerSpritesPerRow;
                    int row = i / FarmerSpritesPerRow;
                    int xOffset = i == 101 ? -15 : 0; // Sheet ist an dieser Stelle inkonsistent, siehe RenderFarmer
                    Vector2 position = new Vector2(FarmerCaptureMargin + column * FarmerCellWidth + FarmerWidthHeadroom / 2 + xOffset, FarmerCaptureMargin + row * FarmerCellHeight + FarmerHeadroom);
                    RenderFarmer(spriteBatch, farmer, i, position);
                }
                spriteBatch.End();
            }

            graphicsDevice.SetRenderTargets(previousTargets);

            Color[] pixels = new Color[targetWidth * targetHeight];
            renderTarget.GetData(pixels);
            Texture2D captured = new Texture2D(graphicsDevice, targetWidth, targetHeight);
            captured.SetData(pixels);

            // Statt das ganze gepolsterte Raster in einem Rutsch zu croppen+skalieren (wodurch der
            // Puffer mitskaliert wird und der Charakter nicht mehr bei (0,0) jeder Zelle landet),
            // wird jeder Frame einzeln eng um den Charakter (FarmerDrawWidth x FarmerDrawHeight, ohne
            // Puffer) zugeschnitten, auf 16x32 runterskaliert und danach lückenlos in ein neues
            // 22-Spalten-Raster gepackt. So bleibt es konsistent mit den Annahmen von
            // CopyFrameToNewSpritesheet (16x32-Zellen, 22 Spalten, Charakter bei (0,0) jeder Zelle).
            int outputFrameWidth = (int)(FarmerDrawWidth * FarmerOutputScale);
            int outputFrameHeight = (int)(FarmerDrawHeight * FarmerOutputScale);
            int outputGridWidth = FarmerSpritesPerRow * outputFrameWidth;
            int outputGridHeight = rows * outputFrameHeight;

            Color[] packedData = new Color[outputGridWidth * outputGridHeight];

            for (int i = 0; i < FarmerFrameCount; i++)
            {
                int column = i % FarmerSpritesPerRow;
                int row = i / FarmerSpritesPerRow;
                int xOffset = i == 101 ? -15 : 0;

                Rectangle tightRect = new Rectangle(
                    FarmerCaptureMargin + column * FarmerCellWidth + FarmerWidthHeadroom / 2 + xOffset,
                    FarmerCaptureMargin + row * FarmerCellHeight + FarmerHeadroom,
                    FarmerDrawWidth,
                    FarmerDrawHeight);

                using Texture2D frameTex = CropTexture(captured, tightRect);
                using Texture2D scaledFrameTex = ScaleTexture(frameTex, FarmerOutputScale);

                Color[] frameData = new Color[outputFrameWidth * outputFrameHeight];
                scaledFrameTex.GetData(frameData);

                int targetX = column * outputFrameWidth;
                int targetY = row * outputFrameHeight;
                for (int y = 0; y < outputFrameHeight; y++)
                {
                    for (int x = 0; x < outputFrameWidth; x++)
                    {
                        packedData[(targetY + y) * outputGridWidth + (targetX + x)] = frameData[y * outputFrameWidth + x];
                    }
                }
            }

            Texture2D packed = new Texture2D(graphicsDevice, outputGridWidth, outputGridHeight);
            packed.SetData(packedData);
            return packed;
        }


        public static Texture2D ScaleTexture(Texture2D originalTexture, float scale)
        {
            // Berechne die neue Breite und Höhe basierend auf dem Skalierungsfaktor
            int newWidth = (int)(originalTexture.Width * scale);
            int newHeight = (int)(originalTexture.Height * scale);

            // Hole die Pixel-Daten der Original-Textur
            Color[] originalData = new Color[originalTexture.Width * originalTexture.Height];
            originalTexture.GetData(originalData);

            // Erstelle ein neues Pixel-Array für die skalierten Pixel
            Color[] newData = new Color[newWidth * newHeight];

            // Schleife über jedes Pixel der neuen Textur
            for (int y = 0; y < newHeight; y++)
            {
                for (int x = 0; x < newWidth; x++)
                {
                    // Berechne die Position im Originalbild
                    int originalX = (int)(x / scale);
                    int originalY = (int)(y / scale);

                    // Hole den Farbwert vom Original und weise ihn der neuen Position zu
                    newData[y * newWidth + x] = originalData[originalY * originalTexture.Width + originalX];
                }
            }

            // Erstelle eine neue Texture2D mit den neuen Abmessungen und setze die skalierten Pixel-Daten
            Texture2D scaledTexture = new Texture2D(Game1.graphics.GraphicsDevice, newWidth, newHeight);
            scaledTexture.SetData(newData);

            return scaledTexture;
        }

        public static void InitFarmerSpriteSheet()
        {
            ModEntry.farmerSpriteSheet = CaptureFarmerSpriteSheet(Game1.player);

            // Ausgewählte Frames aus dem vollen Farmer-Sheet (22 Spalten) in das
            // resultTexture (4 Spalten, NPC-Layout) übertragen, das im Menü als
            // farmerPreview angezeigt und beim Export gespeichert wird.

            //Walking Front
            CopyFrameToNewSpritesheet(ModEntry.farmerSpriteSheet, ModEntry.resultTexture, 0, 0);
            CopyFrameToNewSpritesheet(ModEntry.farmerSpriteSheet, ModEntry.resultTexture, 2, 1);
            CopyFrameToNewSpritesheet(ModEntry.farmerSpriteSheet, ModEntry.resultTexture, 0, 2);
            CopyFrameToNewSpritesheet(ModEntry.farmerSpriteSheet, ModEntry.resultTexture, 1, 3);

            //Walking Right
            CopyFrameToNewSpritesheet(ModEntry.farmerSpriteSheet, ModEntry.resultTexture, 6, 4);
            CopyFrameToNewSpritesheet(ModEntry.farmerSpriteSheet, ModEntry.resultTexture, 7, 5);
            CopyFrameToNewSpritesheet(ModEntry.farmerSpriteSheet, ModEntry.resultTexture, 6, 6);
            CopyFrameToNewSpritesheet(ModEntry.farmerSpriteSheet, ModEntry.resultTexture, 8, 7);

            //Walking Back
            CopyFrameToNewSpritesheet(ModEntry.farmerSpriteSheet, ModEntry.resultTexture, 12, 8);
            CopyFrameToNewSpritesheet(ModEntry.farmerSpriteSheet, ModEntry.resultTexture, 14, 9);
            CopyFrameToNewSpritesheet(ModEntry.farmerSpriteSheet, ModEntry.resultTexture, 12, 10);
            CopyFrameToNewSpritesheet(ModEntry.farmerSpriteSheet, ModEntry.resultTexture, 13, 11);

            //Walking Left
            CopyFrameToNewSpritesheet(ModEntry.farmerSpriteSheet, ModEntry.resultTexture, 6, 12, true);
            CopyFrameToNewSpritesheet(ModEntry.farmerSpriteSheet, ModEntry.resultTexture, 7, 13, true);
            CopyFrameToNewSpritesheet(ModEntry.farmerSpriteSheet, ModEntry.resultTexture, 6, 14, true);
            CopyFrameToNewSpritesheet(ModEntry.farmerSpriteSheet, ModEntry.resultTexture, 8, 15, true);

            //Kiss
            CopyFrameToNewSpritesheet(ModEntry.farmerSpriteSheet, ModEntry.resultTexture, 101, 28);
        }


        public static Texture2D MakeColorTransparent(Texture2D originalTexture, Color targetColor, int tolerance)
        {
            GraphicsDevice graphicsDevice = Game1.graphics.GraphicsDevice;
            Color[] pixelData = new Color[originalTexture.Width * originalTexture.Height];
            originalTexture.GetData(pixelData);

            for (int i = 0; i < pixelData.Length; i++)
            {
                Color pixel = pixelData[i];

                // Prüfe, ob der Pixel innerhalb der Toleranz liegt
                if (IsColorWithinTolerance(pixel, targetColor, tolerance))
                {
                    pixelData[i] = Color.Transparent;
                }
            }

            Texture2D newTexture = new Texture2D(graphicsDevice, originalTexture.Width, originalTexture.Height);
            newTexture.SetData(pixelData);
            return newTexture;
        }
        

        public static Texture2D RemoveGreenScreen(Texture2D input, float tolerance = 0.1f)
        {
            Color[] pixels = new Color[input.Width * input.Height];
            input.GetData(pixels);

            Color chromaKey = new Color(127, 127, 127); // z. B. Greenscreen

            float chromaR = chromaKey.R / 255f;
            float chromaG = chromaKey.G / 255f;
            float chromaB = chromaKey.B / 255f;

            for (int i = 0; i < pixels.Length; i++)
            {
                Color px = pixels[i];

                float a = px.A / 255f;
                float r = px.R / 255f;
                float g = px.G / 255f;
                float b = px.B / 255f;

                float diff = Math.Abs(r - chromaR) + Math.Abs(g - chromaG) + Math.Abs(b - chromaB);

                // 1. Harte Entfernung des Chroma-Key-Hintergrunds
                if (diff < tolerance)
                {
                    pixels[i] = Color.Transparent;
                }
                // 2. Korrektur halbtransparenter Pixel
                else if (a < 1.0f && a > 0f)
                {
                    float r0 = (r - (1 - a) * chromaR) / a;
                    float g0 = (g - (1 - a) * chromaG) / a;
                    float b0 = (b - (1 - a) * chromaB) / a;

                    r0 = Math.Clamp(r0, 0f, 1f);
                    g0 = Math.Clamp(g0, 0f, 1f);
                    b0 = Math.Clamp(b0, 0f, 1f);

                    pixels[i] = new Color(r0, g0, b0, a);
                }
            }

            Texture2D result = new Texture2D(Game1.graphics.GraphicsDevice, input.Width, input.Height);
            result.SetData(pixels);
            return result;
        }

        private static bool IsColorWithinTolerance(Color c1, Color c2, int tolerance)
        {
            return Math.Abs(c1.R - c2.R) <= tolerance &&
                   Math.Abs(c1.G - c2.G) <= tolerance &&
                   Math.Abs(c1.B - c2.B) <= tolerance;
        }
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
                        // Überprüfen, ob der Pixel nicht vollständig transparent ist
                        int sourceIndex = x + y * characterTexture.Width;
                        if (characterPixels[sourceIndex].A > 0)
                        {
                            // Kopieren des Pixels von der Figur auf das Ziel-Texturen-Objekt mit Alpha-Blending
                            int destIndex = destX + destY * backgroundTexture.Width;
                            resultPixels[destIndex] = AlphaBlend(characterPixels[sourceIndex], backgroundPixels[destIndex]);
                        }
                    }
                }
            }

            // Daten in das Ziel-Texturen-Objekt schreiben
            resultTexture.SetData(resultPixels);

            return resultTexture;
        }

        private static Color AlphaBlend(Color source, Color destination)
        {
            float alpha = source.A / 255f;
            float inverseAlpha = 1f - alpha;

            byte r = (byte)((source.R * alpha) + (destination.R * inverseAlpha));
            byte g = (byte)((source.G * alpha) + (destination.G * inverseAlpha));
            byte b = (byte)((source.B * alpha) + (destination.B * inverseAlpha));
            byte a = (byte)(source.A + destination.A * inverseAlpha);

            return new Color(r, g, b, a);
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
        public static Texture2D CaptureArea(Rectangle area)
        {
            // Beschränke den Bereich auf den Backbuffer, falls er außerhalb liegt
            GraphicsDevice graphicsDevice = Game1.graphics.GraphicsDevice;
            area = Rectangle.Intersect(area, new Rectangle(0, 0,
                                   graphicsDevice.PresentationParameters.BackBufferWidth,
                                   graphicsDevice.PresentationParameters.BackBufferHeight));

            // Erstelle ein neues RenderTarget in der Größe des Bereichs
            using (RenderTarget2D renderTarget = new RenderTarget2D(graphicsDevice, area.Width, area.Height))
            {
                // Setze das RenderTarget als Ziel für das Rendering
                graphicsDevice.SetRenderTarget(renderTarget);
                graphicsDevice.Clear(Color.Transparent);

                // Erstelle einen SpriteBatch für das Rendering
                using (SpriteBatch spriteBatch = new SpriteBatch(graphicsDevice))
                {
                    spriteBatch.Begin();

                    // Kopiere den Bereich aus dem Backbuffer ins RenderTarget
                    spriteBatch.Draw(
                        GetBackBufferAsTexture(), // Texturauszug des Backbuffers
                        new Rectangle(0, 0, area.Width, area.Height), // Zielgröße (RenderTarget)
                        area, // Quelle aus dem Backbuffer
                        Color.White);

                    spriteBatch.End();
                }

                // Setze das RenderTarget zurück
                graphicsDevice.SetRenderTarget(null);

                // Erstelle die Texture2D aus den RenderTarget-Daten
                Texture2D screenshotTexture = new Texture2D(graphicsDevice, area.Width, area.Height);
                Color[] data = new Color[area.Width * area.Height];
                renderTarget.GetData(data);
                screenshotTexture.SetData(data);

                return screenshotTexture; // Rückgabe der aufgenommenen Texture2D
            }
        }

        private static Texture2D GetBackBufferAsTexture()
        {
            // Erstelle Texture2D mit der Größe des Backbuffers
            GraphicsDevice graphicsDevice = Game1.graphics.GraphicsDevice;
            Texture2D backBufferTexture = new Texture2D(graphicsDevice,
                graphicsDevice.PresentationParameters.BackBufferWidth,
                graphicsDevice.PresentationParameters.BackBufferHeight);

            // Pixel-Daten vom Backbuffer abrufen
            Color[] backBufferData = new Color[graphicsDevice.PresentationParameters.BackBufferWidth *
                                               graphicsDevice.PresentationParameters.BackBufferHeight];
            graphicsDevice.GetBackBufferData(backBufferData);

            // Pixel-Daten in das Texture2D-Objekt setzen
            backBufferTexture.SetData(backBufferData);

            return backBufferTexture;
        }
    }
}