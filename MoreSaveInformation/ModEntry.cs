using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley;
using HarmonyLib;
using StardewValley.Menus;
using static StardewValley.Menus.LoadGameMenu;

namespace MoreSaveInformation
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

        public static Mod instance;
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            instance = this;

            Harmony harmony = new(this.ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(SaveFileSlot), nameof(SaveFileSlot.Draw)),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.Postfix_DrawSaveFileSlot))
                );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            // print button presses to the console window
            this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
        }
    }
}
