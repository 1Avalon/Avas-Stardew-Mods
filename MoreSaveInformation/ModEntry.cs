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
            instance = this;

            Harmony harmony = new(this.ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(SaveFileSlot), nameof(SaveFileSlot.Draw)),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.Postfix_DrawSaveFileSlot))
                );
        }
    }
}
