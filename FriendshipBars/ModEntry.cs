using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace FriendshipBars
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

        public static Texture2D progressBar;

        public override void Entry(IModHelper helper)
        {
            Harmony harmony = new Harmony(this.ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(SocialPage), nameof(SocialPage.drawNPCSlot)),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.Postfix_drawNPCSlot))
                );

            harmony.Patch(
                original: AccessTools.Method(typeof(SocialPage), nameof(SocialPage.updateSlots)),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.Postfix_updateSlots))
                );

            harmony.Patch(
                original: AccessTools.Method(typeof(SocialPage), nameof(SocialPage.performHoverAction)),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.Postfix_performHoverAction))
                );


            progressBar = Helper.ModContent.Load<Texture2D>("assets\\progress_bar.png");
        }
    }
}