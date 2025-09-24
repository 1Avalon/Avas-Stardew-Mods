using System.Collections.Generic;
using GiftMoney;
using HarmonyLib;
using StardewValley;

namespace FakeFriends;

public static class MoneyAsGiftPatch
{
	public static void Postfix(ref int __result, Item item)
	{
		IDictionary<string, int> giftTastes = new Dictionary<string, int>
		{
			{ ModEntry.GiftIds.Neutral, 8 },
			{ ModEntry.GiftIds.Liked, 2 },
			{ ModEntry.GiftIds.Loved, 0 }
		};
		if (giftTastes.ContainsKey(item.ItemId))
		{
			__result = giftTastes[item.ItemId];
		}
	}
}
