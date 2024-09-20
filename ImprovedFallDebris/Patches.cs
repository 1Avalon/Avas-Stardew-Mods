using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImprovedFallDebris
{
    public static class Patches
    {
        private static int debrisAmount
        {
            get => Game1.debrisWeather.Count;
        }

        private static Texture2D leafTexture = TextureUtils.CropTexture(Game1.mouseCursors, new Rectangle(352, 1216, 176, 16));

        public static Dictionary<WeatherDebris, Texture2D> customDebrisTextures = new Dictionary<WeatherDebris, Texture2D>();

        public static List<Vector3> Adjustments = new List<Vector3>()
        {
            new Vector3(0.10f, 0, 0), // yellow
            //new Vector3(0.8f, 0, -10),
            new Vector3 (0f, 0, 0), // red / default
            new Vector3(0.3f, -10, -15) // green

        };

        private static Vector3 getRandomShift()
        {
            int index = Game1.random.Next(Adjustments.Count);
            return Adjustments[index];
        }

        private static int debrisCounter = 0;
        public static bool Prefix_draw(WeatherDebris __instance, SpriteBatch b)
        {
            if (Game1.Date.Season != Season.Fall || Game1.debrisWeather.Count == 0)
                return true;

            if (ModEntry.ReloadDebrisTexture)
            {
                leafTexture = TextureUtils.CropTexture(Game1.mouseCursors, new Rectangle(352, 1216, 176, 16));
                ModEntry.ReloadDebrisTexture = false;
            }

            if (debrisCounter == debrisAmount)
                debrisCounter = 0;

            Rectangle alternateSourceRect = __instance.sourceRect;
            alternateSourceRect.X -= 352;
            alternateSourceRect.Y -= 1216;


            if (!customDebrisTextures.ContainsKey(__instance))
                customDebrisTextures.Add(__instance, TextureUtils.ShiftHueSaturationAndBrightness(leafTexture, getRandomShift()));

            b.Draw(customDebrisTextures[__instance], __instance.position, alternateSourceRect, Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 1E-06f);
            debrisCounter++;

            if (ModEntry.Config.AlwaysExecuteOriginalMethod)
                return true;

            return false;
        }
    }
}
