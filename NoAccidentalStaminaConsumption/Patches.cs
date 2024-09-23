using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoAccidentalStaminaConsumption
{
    public static class Patches
    {
        public static void WateringCan_Prefix_DoFunction(WateringCan __instance, GameLocation location, int x , int y, int power, Farmer who)
        {
            //ModEntry.instance.Helper.Reflection.GetMethod(__instance, "tilesAffected").Invoke(new Vector2(x / 64, y / 64), power, who);
            List<Vector2> tileLocations = ModEntry.instance.Helper.Reflection.GetMethod(__instance, "tilesAffected").Invoke<List<Vector2>>(new Vector2(x / 64, y / 64), power, who);
            Vector2 actionTile = new Vector2(x / 64, y / 64);
            foreach(Vector2 tileLocation in tileLocations)
            {
                if (location.terrainFeatures.TryGetValue(tileLocation, out var terrainFeature))
                {
                    if (terrainFeature is HoeDirt dirt)
                    {
                        if (dirt.state.Value == 1 && tileLocation == actionTile)
                            ModEntry.instance.Monitor.Log("crop already watered", StardewModdingAPI.LogLevel.Info);
                    }
                }
            }
        }
    }
}
