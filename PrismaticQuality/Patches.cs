using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace PrismaticQuality
{
    public static class Patches
    {

        public static void Prefix_addItemToInventory(ref Item item, bool makeActiveObject = false)
        {
            if (item is null) 
                return;

            if (item.HasBeenInInventory)
                return;

            if (item.Quality == 4 && Game1.random.NextDouble() >= 0.5f)
                item.Quality = 5;
        }

        public static void DrawIconsMenu_Postfix(Item __instance, SpriteBatch sb, Vector2 location, float scale_size, float transparency, float layer_depth, StackDrawType drawStackNumber, Color color)
        {
            if (__instance.quality.Value == 5)
            {
                var qualityRect = new Rectangle(0, 0, 8, 8);
                var qualitySheet = ModEntry.PrismaticStarTexture;
                sb.Draw(qualitySheet, location + new Vector2(12f, 52f), qualityRect, color * transparency, 0f, new Vector2(4f, 4f), 3f * scale_size, SpriteEffects.None, layer_depth);
            }
        }
    }
}
