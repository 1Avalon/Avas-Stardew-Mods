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

        public static List<Item> flaggedIridiumItems;

        public static ModConfig Config;
        public override void Entry(IModHelper helper)
        {
            instance = this;

            PrismaticStarTexture = Helper.ModContent.Load<Texture2D>("assets/PrismaticStarTexture.png");

            Helper.Events.World.DebrisListChanged += OnDebrisListChanged;

            Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;

            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            Config = Helper.ReadConfig<ModConfig>();

            Harmony harmony = new Harmony(this.ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(Item), nameof(Item.DrawMenuIcons)),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.DrawIconsMenu_Postfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.addItemToInventoryBool)),
                prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.Prefix_addItemToInventory))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.GetHarvestSpawnedObjectQuality)),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.Postfix_GetHarvestObjectQuality))
            );
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            flaggedIridiumItems = new List<Item>();
        }
        private void OnDebrisListChanged(object sender, DebrisListChangedEventArgs e)
        {
            if (!Context.IsMultiplayer)
                return;

            foreach (Debris debris in e.Added)
            {
                if (debris.DroppedByPlayerID.Value == 0)
                    continue;

                long authorId = debris.DroppedByPlayerID.Value;

                if (authorId == Game1.player.UniqueMultiplayerID)
                    continue;

                Item item = debris.item;
                flaggedIridiumItems.Add(item);
            }
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
                name: () => "Prismatic for Botanist",
                tooltip: () => "Makes every item affected by the Botanist profession prismatic",
                getValue: () => Config.AlwaysPrismaticForage,
                setValue: value => Config.AlwaysPrismaticForage = value
            );
        }
    }
}