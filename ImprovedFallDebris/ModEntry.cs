using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using HarmonyLib;
using StardewValley;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics.Tracing;
using System.Runtime.InteropServices;

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
        public static bool ReloadDebrisTexture = false;
        public override void Entry(IModHelper helper)
        {
            Harmony harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(WeatherDebris), nameof(WeatherDebris.draw)),
                prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.Prefix_draw))
                );

            Helper.ConsoleCommands.Add("clear_debris_array", "Clears the colored fall leaves debris. Not an interesting command for a player", this.ClearDebrisArray);
            Helper.ConsoleCommands.Add("add_debris_color", "Adds a new color choice to the array. Not an interesting command for a player", this.AddColorToDebrisArray);

            Helper.Events.GameLoop.DayStarted += OnDayStarted;
            Helper.Events.Content.AssetRequested += OnAssetRequested;
        }

        /*********
** Private methods
*********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        ///
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            Patches.customDebrisTextures.Clear();
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("LooseSprites\\Cursors"))
                ReloadDebrisTexture = true;
        }
        private void ClearDebrisArray(string command, string[] args)
        {
            Patches.customDebrisTextures.Clear();
            Monitor.Log("Successfully cleared the custom debris", LogLevel.Info);
        }
        private void AddColorToDebrisArray(string command, string[] args)
        {
            float hueShift = float.Parse(args[0]);
            int saturation = int.Parse(args[1]);
            int brightness = int.Parse(args[2]);

            Vector3 values = new Vector3(hueShift, saturation, brightness);

            int index;
            if (args.Length >= 4)
            {
                index = int.Parse(args[3]);
                Patches.Adjustments[index] = values;
                Monitor.Log($"Successfully replaced the values at index {index}.", LogLevel.Info);
            }
            else
            {
                Patches.Adjustments.Add(values);
                Monitor.Log("Successfully added a new color. Please clear the array using the corresponding command", LogLevel.Info);
            }
            Patches.customDebrisTextures.Clear();
        }
    }
}