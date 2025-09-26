using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftMoney
{
    public static class AmountManager
    {

        private static int MoneyEarned = 10000;

        public static int LovedAmount { get => getForLovedGift(); }

        public static int LikedAmount {  get => getForLikedGift(); }

        public static int NeutralAmount { get => getForNeutralGift(); }

        private static bool initEvents = false;

        private static int getForLovedGift()
        {
            if (!ModEntry.Config.scaleLimitsWithProgress)
                return ModEntry.Config.minLovedGiftAmount;

            return (int)(MoneyEarned * 0.06f);

        }

        private static int getForLikedGift()
        {
            if (!ModEntry.Config.scaleLimitsWithProgress)
                return ModEntry.Config.minLikedGiftAmount;

            return (int)(MoneyEarned * 0.03f);

        }

        private static int getForNeutralGift()
        {
            if (!ModEntry.Config.scaleLimitsWithProgress)
                return ModEntry.Config.minLikedGiftAmount;

            return (int)(MoneyEarned * 0.01f);

        }

        public static void InitEvents()
        {
            if (initEvents) return;

            ModEntry.instance.Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            ModEntry.instance.Helper.Events.GameLoop.Saving += OnSaving;

            initEvents = true;
        }

        private static void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            string data = ModEntry.instance.Helper.Data.ReadSaveData<string>($"{ModEntry.instance.ModManifest.UniqueID}_Amount");

            if (data == null)
            {
                UpdateAmount();
                return;
            }

            MoneyEarned = int.Parse(data);
        }

        private static void OnSaving(object sender, SavingEventArgs e)
        {
            ModEntry.instance.Helper.Data.WriteSaveData<string>($"{ModEntry.instance.ModManifest.UniqueID}_Amount", MoneyEarned.ToString());
        }

        public static void UpdateAmount()
        {
            int moneyEarnedATM = (int)Game1.player.totalMoneyEarned;
            if (moneyEarnedATM < 10000)
                moneyEarnedATM = 10000;

            MoneyEarned = moneyEarnedATM;
        }
    }
}
