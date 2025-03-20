using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;

namespace PrismaticQuality
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

        public static Texture2D PrismaticStarTexture;

        public static Mod instance;

        public override void Entry(IModHelper helper)
        {
            instance = this;

            helper.Events.Display.Rendered += this.OnRendered;

            PrismaticStarTexture = Helper.ModContent.Load<Texture2D>("assets/PrismaticStarTexture.png");

            Harmony harmony = new Harmony(this.ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.GetHarvestSpawnedObjectQuality)),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.Postfix_GetHarvestObjectQuality))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Item), nameof(Item.DrawMenuIcons)),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.DrawIconsMenu_Postfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.addItemToInventoryBool)),
                prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.Prefix_addItemToInventory))
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnRendered(object sender, RenderedEventArgs e)
        {
            e.SpriteBatch.Draw(PrismaticStarTexture, new Rectangle(0, 0, 8, 8), Color.White);
        }
    }
}