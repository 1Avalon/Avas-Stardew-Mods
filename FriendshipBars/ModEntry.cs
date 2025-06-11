using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using GenericModConfigMenu;

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

        public static ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            Harmony harmony = new Harmony(this.ModManifest.UniqueID);

            Config = Helper.ReadConfig<ModConfig>();

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

            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;


            progressBar = Helper.ModContent.Load<Texture2D>("assets\\progress_bar.png");
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(Config)
            );

            // add some config options
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Enabled",
                tooltip: () => "Enable or disable the bars",
                getValue: () => Config.Enabled,
                setValue: value => Config.Enabled = value
            );

            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Hover Settings"
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Show Current Points",
                tooltip: () => "Shows your current Points",
                getValue: () => Config.CurrentPointsHover,
                setValue: value => Config.CurrentPointsHover = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Show Required Points",
                tooltip: () => "Shows Points required for the next Heart",
                getValue: () => Config.RequiredPointsHover,
                setValue: value => Config.RequiredPointsHover = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Show Completion",
                tooltip: () => "Shows the Progress for the next heart in %",
                getValue: () => Config.CompletionHover,
                setValue: value => Config.CompletionHover = value
            );
        }
    }
}