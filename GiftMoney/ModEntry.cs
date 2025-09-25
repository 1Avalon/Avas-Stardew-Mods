#define DEBUG
using System;
using FakeFriends;
using GenericModConfigMenu;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace GiftMoney;

internal sealed class ModEntry : Mod
{
	public static ModConfig Config;

	public static Texture2D icons;

	public static ModEntry instance;

	public static class GiftIds
	{
		public static string Loved = $"{ModEntry.instance.ModManifest.UniqueID}.Loved";

        public static string Liked = $"{ModEntry.instance.ModManifest.UniqueID}.Liked";

        public static string Neutral = $"{ModEntry.instance.ModManifest.UniqueID}.Neutral";
    }

	public override void Entry(IModHelper helper)
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected O, but got Unknown
		Config = ((Mod)this).Helper.ReadConfig<ModConfig>();
		helper.Events.Input.ButtonPressed += OnButtonPressed;
		helper.Events.GameLoop.GameLaunched += OnGameLaunched;
		helper.Events.GameLoop.DayStarted += OnDayStarted;
		helper.Events.GameLoop.UpdateTicking += OnUpdateTicking;
		icons = helper.ModContent.Load<Texture2D>("assets\\icons.png");
		instance = this;

		I18n.Init(Helper.Translation);

		Harmony harmony = new Harmony(this.ModManifest.UniqueID);

        harmony.Patch(
			original: AccessTools.Method(typeof(NPC), nameof(NPC.getGiftTasteForThisItem)),
			postfix: new HarmonyMethod(typeof(MoneyAsGiftPatch), nameof(MoneyAsGiftPatch.Postfix))
			);
    }

	private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
	{
		IGenericModConfigMenuApi configMenu = ((Mod)this).Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
		if (configMenu != null)
		{
			configMenu.Register(((Mod)this).ModManifest, delegate
			{
				Config = new ModConfig();
			}, delegate
			{
				((Mod)this).Helper.WriteConfig<ModConfig>(Config);
			});
			configMenu.AddNumberOption(((Mod)this).ModManifest, () => Config.minLovedGiftAmount, delegate(int value)
			{
				Config.minLovedGiftAmount = value;
			}, () => I18n.Config_MinAmountLoved());
			configMenu.AddNumberOption(((Mod)this).ModManifest, () => Config.minLikedGiftAmount, delegate(int value)
			{
				Config.minLikedGiftAmount = value;
			}, () => I18n.Config_MinAmountLiked());
			configMenu.AddNumberOption(((Mod)this).ModManifest, () => Config.minNeutralGiftAmount, delegate(int value)
			{
				Config.minNeutralGiftAmount = value;
			}, () => I18n.Config_MinAmountNeutral());
			configMenu.AddKeybindList(((Mod)this).ModManifest, () => Config.sendMoneyKey, delegate(KeybindList value)
			{
				Config.sendMoneyKey = value;
			}, () => I18n.Config_ToggleUI());
		}
	}

	private void OnUpdateTicking(object sender, EventArgs e)
	{
		if (Game1.isFestival() && Game1.activeClickableMenu is ItemGrabMenu)
		{
			Game1.activeClickableMenu = new CustomItemGrabMenu(null, reverseGrab: false, showReceivingMenu: false, Utility.highlightSantaObjects, Game1.currentLocation.currentEvent.chooseSecretSantaGift, Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1788", Game1.currentLocation.currentEvent.secretSantaRecipient.displayName));
		}
	}

	private void OnButtonPressed(object sender, ButtonPressedEventArgs ev)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if (!Context.IsWorldReady || Game1.activeClickableMenu != null || !Context.IsPlayerFree || !Config.sendMoneyKey.JustPressed())
		{
			return;
		}
		foreach (NPC npc in Game1.currentLocation.characters)
		{
			Vector2 npcPos = npc.Position;
			Vector2 playerPos = Game1.player.Position;
			float distance = Vector2.Distance(npcPos, playerPos);
			if (distance <= 128f)
			{
				Game1.activeClickableMenu = new SendMoneyUI(npc.Name);
				break;
			}
		}
	}

	private void OnDayStarted(object sender, DayStartedEventArgs e)
	{
		SendMoneyUI.npcNames.Clear();
	}
}
