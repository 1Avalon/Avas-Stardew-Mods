using DynamicNPCPaintings.Framework;
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
using CustomNPCPaintings;
using CustomNPCPaintings.Framework;
using xTile.Layers;

namespace DynamicNPCPaintings
{
    public static class TextureHelper
    {

        private static readonly Dictionary<string, string> CUTE_SEASONAL_CHARACTERS_FESTIVAL = new Dictionary<string, string>()
        {
            ["festival_winter25"] = "Winter_WinterStar",
            ["festival_spring24"] = "FlowerDance",
            ["festival_summer28"] = "Jellies",
            ["festival_summer11"] = "Luau",
            ["festival_fall27"] = "SpiritsEve",
            ["festival_winter8"] = "Winter_IceF",
            ["festival_spring13"] = "EggF",
            ["festival_fall16"] = "Fair",

        };
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

        public static Texture2D ShiftHSV_Range(Texture2D input, HSVPair hsv,
            float maxHueShiftDegrees = 180f,
            float maxSaturationShift = 1f,
            float maxValueShift = 1f)
        {
            // Parameter 0–99 → -1 bis +1 → auf Max Shift
            float hueShift = ConvertSlider(hsv.hue) * maxHueShiftDegrees;
            float saturationShift = ConvertSlider(hsv.saturation) * maxSaturationShift;
            float valueShift = ConvertSlider(hsv.value) * maxValueShift;

            return ShiftHSV(input, hueShift, saturationShift, valueShift);
        }
        private static float ConvertSlider(int v)
        {
            return ((v / 99f) * 2f) - 1f;
        }

        public static Texture2D ShiftHSV(Texture2D input, float hueShift, float saturationShift, float valueShift)
        {
            // Pixelarray auslesen
            Color[] data = new Color[input.Width * input.Height];
            input.GetData(data);

            for (int i = 0; i < data.Length; i++)
            {
                Color c = data[i];

                // Transparente Pixel ignorieren
                if (c.A == 0)
                    continue;

                // RGB → HSV
                float h, s, v;
                ColorToHSV(c, out h, out s, out v);

                // Shifts anwenden
                h = (h + hueShift) % 360f;
                if (h < 0) h += 360f;

                s = Math.Clamp(s + saturationShift, 0f, 1f);
                v = Math.Clamp(v + valueShift, 0f, 1f);

                // HSV → RGB
                Color newColor = ColorFromHSV(h, s, v);
                newColor.A = c.A;

                data[i] = newColor;
            }

            // Neue Textur mit verschobenen Pixeln erzeugen
            Texture2D result = new Texture2D(Game1.graphics.GraphicsDevice, input.Width, input.Height);
            result.SetData(data);
            return result;
        }

        public static void ColorToHSV(Color color, out float hue, out float saturation, out float value)
        {
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;

            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));
            float delta = max - min;

            // Hue
            if (delta == 0)
                hue = 0;
            else if (max == r)
                hue = 60f * (((g - b) / delta) % 6f);
            else if (max == g)
                hue = 60f * (((b - r) / delta) + 2f);
            else
                hue = 60f * (((r - g) / delta) + 4f);

            if (hue < 0) hue += 360f;

            // Saturation
            saturation = (max == 0) ? 0 : delta / max;

            // Value
            value = max;
        }

        public static Color ColorFromHSV(float hue, float saturation, float value)
        {
            float c = value * saturation;
            float x = c * (1 - Math.Abs(((hue / 60f) % 2) - 1));
            float m = value - c;

            float r, g, b;

            if (hue < 60) { r = c; g = x; b = 0; }
            else if (hue < 120) { r = x; g = c; b = 0; }
            else if (hue < 180) { r = 0; g = c; b = x; }
            else if (hue < 240) { r = 0; g = x; b = c; }
            else if (hue < 300) { r = x; g = 0; b = c; }
            else { r = c; g = 0; b = x; }

            return new Color(
                (byte)((r + m) * 255),
                (byte)((g + m) * 255),
                (byte)((b + m) * 255)
            );
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

        // Layout muss zu CharacterLayer/GetCharacterFrame passen: 22 Sprites pro Zeile, Zellgröße 32x48 im Ergebnis
        // (16x32 Charakter mittig platziert + 8px Puffer links/rechts + 16px Kopfraum oben, damit Hüte/Frisuren/
        // breite Werkzeug-Animationen nicht ins Nachbar-Frame bluten)
        private const int FarmerSpritesPerRow = 22;
        private const int FarmerFrameCount = 126;
        private const int FarmerWidthHeadroom = 64; // Puffer links+rechts vom Charakter (pre-scale), 64px -> 16px im Endergebnis
        private const int FarmerCellWidth = 64 + FarmerWidthHeadroom;
        private const int FarmerHeadroom = 64; // Puffer oberhalb des Charakters (pre-scale), 64px -> 16px im Endergebnis
        private const int FarmerCellHeight = 128 + FarmerHeadroom;
        private const float FarmerOutputScale = 0.25f; // pre-scale Zelle -> Sprite im Endergebnis
        private const int FarmerCaptureMargin = 32; // Puffer, falls ein Frame über seine Zelle hinausragt (z.B. Werkzeug-Schwung)

        public static void InitFarmerSpriteSheet()
        {
            ModEntry.farmerSpriteSheet = CaptureFarmerSpriteSheet(Game1.player);
        }

        /// <summary>
        /// Rendert alle Farmer-Frames in ein eigenes, transparent geleertes RenderTarget2D und liest es direkt aus.
        /// Kein Greenscreen/Chroma-Keying nötig: Bei einem transparenten Ziel liefert SpriteBatchs Standard-AlphaBlend
        /// (ColorSourceBlend=One, ColorDestinationBlend=InverseSourceAlpha) exakt die Quellfarbe und den Quell-Alphawert
        /// zurück, auch für halbtransparente Randpixel. Die Größe ist zudem unabhängig von Bildschirm/Fenstergröße.
        /// </summary>
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

            Texture2D cropped = CropTexture(captured, new Rectangle(FarmerCaptureMargin, FarmerCaptureMargin, gridWidth, gridHeight));
            return ScaleTexture(cropped, FarmerOutputScale);
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
        public static Texture2D GetCharacterFrame(CharacterLayer layer, int frame, int spritesPerRow, bool flipped = false)
        {
            Texture2D tex;
            try
            {
                string eventId = Game1.CurrentEvent?.id;
                string sheetName = CUTE_SEASONAL_CHARACTERS_FESTIVAL[eventId];
                tex = ModEntry.instance.Helper.GameContent.Load<Texture2D>($"Characters/{layer.Name}_{sheetName}");
            }
            catch
            {
                tex = layer.Texture;
            }
            int xOffset = (frame % spritesPerRow) * layer.SpriteWidth;
            int yOffset = frame / spritesPerRow * layer.SpriteHeight;
            Rectangle newBounds = new Rectangle(xOffset, yOffset, layer.SpriteWidth, layer.SpriteHeight);

            Texture2D croppedTexture = new Texture2D(Game1.graphics.GraphicsDevice, newBounds.Width, newBounds.Height);
            Color[] data = new Color[newBounds.Width * newBounds.Height];
            try
            {
                tex.GetData(0, newBounds, data, 0, newBounds.Width * newBounds.Height);
            }
            catch 
            {
                newBounds = new Rectangle(0, 0, layer.SpriteWidth, layer.SpriteHeight);
                tex.GetData(0, newBounds, data, 0, newBounds.Width * newBounds.Height);
            }
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


        public static Texture2D BackgroundWithFrame(Frame frame, Color backgroundColor)
        {
            GraphicsDevice graphicsDevice = Game1.graphics.GraphicsDevice;

            // Erstelle eine neue Texture2D mit der gleichen Größe wie der Rahmen
            Texture2D resultTexture = new Texture2D(graphicsDevice, frame.frameTexture.Width, frame.frameTexture.Height);

            // Lade die Pixel-Daten des Rahmens
            Color[] frameData = new Color[frame.frameTexture.Width * frame.frameTexture.Height];
            frame.frameTexture.GetData(frameData);

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
                    // Setze das Pixel in den Ergebnisdaten auf die Hintergrundfarbe
                    resultData[y * frame.frameTexture.Width + x] = backgroundColor;
                }
            }

            // Setze die Pixel-Daten in die resultierende Textur
            resultTexture.SetData(resultData);

            return resultTexture;
        }
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
            background.Texture.GetData(backgroundData);

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

        public static string ExportToPainting(Picture picture, bool addToInventory = true)
        {

            if (Context.IsMultiplayer && !Context.IsMainPlayer)
            {
                Texture2D tex = picture.GetTexture();
                Color[] color = new Color[tex.Width  * tex.Height];
                tex.GetData<Color>(color);

                NetworkPictureData networkPictureData = new NetworkPictureData(color, tex.Width, tex.Height, picture.tileWidth, picture.tileHeight);

                ModEntry.instance.Helper.Multiplayer.SendMessage<NetworkPictureData>(networkPictureData, "CustomPaintingData", new string[] { ModEntry.instance.ModManifest.UniqueID });
                return "";
            }

            int number = ModEntry.dataManager.PictureData.Count + ModEntry.PaintingIncrementOffset + 1;


            string uniqueID = $"AvalonMFX.CustomNPCPaintings.Picture_{number}";
            string uniqueTextureName = $"{Constants.SaveFolderName}.{uniqueID}_IMG";

            if (ModEntry.Config.exportPaintingsLocally)
            {
                if (!Directory.Exists(Path.Combine(ModEntry.instance.Helper.DirectoryPath, "pictures", Constants.SaveFolderName)))
                    Directory.CreateDirectory(Path.Combine(ModEntry.instance.Helper.DirectoryPath, "pictures", Constants.SaveFolderName));

                int file_number = 1;
                var file = Path.Combine(ModEntry.instance.Helper.DirectoryPath, "pictures", Constants.SaveFolderName, $"{Constants.SaveFolderName}_Picture_{file_number}.png");
                while (File.Exists(file))
                {
                    file = Path.Combine(ModEntry.instance.Helper.DirectoryPath, "pictures", Constants.SaveFolderName, $"{Constants.SaveFolderName}_Picture_{++file_number}.png");
                }


                Stream stream = File.Create(file);

                picture.GetTexture().SaveAsPng(stream, picture.frame.frameTexture.Width, picture.frame.frameTexture.Height);
                stream.Close();
                ModEntry.dataManager.TextureData.Add(uniqueTextureName, file);
                Game1.addHUDMessage(new HUDMessage("Successfully saved painting locally", 1));
            }

            if (!ModEntry.dataManager.FurnitureData.ContainsKey(uniqueID))
            {
                ModEntry.dataManager.FurnitureData.Add(uniqueID, $"AvalonMFX.Picture_{number}/painting/{picture.tileWidth} {picture.tileHeight}/{picture.tileWidth} {picture.tileHeight}/1/0/-1/{I18n.Menu_Export_Painting()}/0/{uniqueTextureName}");

                Texture2D tex = picture.GetTexture();
                Color[] color = new Color[tex.Width * tex.Height];
                tex.GetData<Color>(color);

                NetworkPictureData networkPictureData = new NetworkPictureData(color, tex.Width, tex.Height, picture.tileWidth, picture.tileHeight);

                ModEntry.dataManager.PictureData.Add(uniqueTextureName, networkPictureData);
                ModEntry.modHelper.Multiplayer.SendMessage<SavedDataManager>(ModEntry.dataManager, "DataManager", new string[] { ModEntry.instance.ModManifest.UniqueID });
            }
            ModEntry.instance.Helper.GameContent.InvalidateCache("Data/Furniture");
            if (addToInventory)
            {
                Furniture furniture = new Furniture(uniqueID, Vector2.Zero);
                List<Item> items = new List<Item>()
                {
                    furniture
                };
                Game1.activeClickableMenu = new ItemGrabMenu(items);
            }

            return uniqueID;
        }

        public static string ExportToPainting(NetworkPictureData picture, bool addToInventory = true)
        {
            if (!Directory.Exists(Path.Combine(ModEntry.instance.Helper.DirectoryPath, "pictures", Constants.SaveFolderName)))
                Directory.CreateDirectory(Path.Combine(ModEntry.instance.Helper.DirectoryPath, "pictures", Constants.SaveFolderName));

            int number = 1;
            var file = Path.Combine(ModEntry.instance.Helper.DirectoryPath, "pictures", Constants.SaveFolderName, $"{Constants.SaveFolderName}_Picture_{number}.png");
            while (File.Exists(file))
            {
                file = Path.Combine(ModEntry.instance.Helper.DirectoryPath, "pictures", Constants.SaveFolderName, $"{Constants.SaveFolderName}_Picture_{++number}.png");
            }


            Stream stream = File.Create(file);

            picture.GetTexture().SaveAsPng(stream, picture.width, picture.height);
            stream.Close();

            Game1.addHUDMessage(new HUDMessage("Successfully saved the painting", 1));


            string uniqueID = $"AvalonMFX.CustomNPCPaintings.Picture_{number}";

            if (!ModEntry.dataManager.FurnitureData.ContainsKey(uniqueID))
            {
                string uniqueTextureName = $"{Constants.SaveFolderName}.{uniqueID}_IMG";
                ModEntry.dataManager.FurnitureData.Add(uniqueID, $"AvalonMFX.Picture_{number}/painting/{picture.tileWidth} {picture.tileHeight}/{picture.tileWidth} {picture.tileHeight}/1/0/-1/{I18n.Menu_Export_Painting()}/0/{uniqueTextureName}");
                ModEntry.dataManager.TextureData.Add(uniqueTextureName, file);
                ModEntry.dataManager.PictureData.Add(uniqueTextureName, picture);
            }
            ModEntry.instance.Helper.GameContent.InvalidateCache("Data/Furniture");
            if (addToInventory)
            {
                Furniture furniture = new Furniture(uniqueID, Vector2.Zero);
                List<Item> items = new List<Item>()
                {
                    furniture
                };
                Game1.activeClickableMenu = new ItemGrabMenu(items);
            }

            return uniqueID;
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