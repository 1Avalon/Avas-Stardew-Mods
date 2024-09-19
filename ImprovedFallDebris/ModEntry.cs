using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using HarmonyLib;
using StardewValley;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics.Tracing;

namespace ImprovedFallDebris
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        ///

        public override void Entry(IModHelper helper)
        {
            Harmony harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(WeatherDebris), nameof(WeatherDebris.draw)),
                prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.Prefix_draw))
                );
        }
        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
    }
}