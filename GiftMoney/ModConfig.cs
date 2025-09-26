using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace GiftMoney;

public sealed class ModConfig
{
	public bool scaleLimitsWithProgress { get; set; } = true;

	public int minLovedGiftAmount { get; set; } = 10000;


	public int minLikedGiftAmount { get; set; } = 7500;


	public int minNeutralGiftAmount { get; set; } = 5000;


	public int minDislikedGiftAmount { get; set; } = 2500;

	public KeybindList sendMoneyKey { get; set; } = new KeybindList((SButton)116);

}
