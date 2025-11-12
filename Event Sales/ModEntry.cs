using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Event_Sales
{
    internal sealed class ModEntry : Mod
    {

        public static ModEntry Instance;

        public static string SALE_KEY;

        public override void Entry(IModHelper helper)
        {
            Helper.Events.Content.AssetRequested += OnAssetRequested;
            Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            Helper.Events.GameLoop.DayStarted += OnDayStarted;
            SALE_KEY = $"{this.ModManifest.UniqueID}/Sales";
            Instance = this;
        }

        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo(SALE_KEY))
            {
                e.LoadFrom(() => new Dictionary<string, Sale>(), AssetLoadPriority.Exclusive);
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Mail"))
            {
                e.Edit((IAssetData asset) =>
                {
                    var data = asset.AsDictionary<string, string>().Data;

                    foreach (var kvp in SaleManager.MailData)
                    {
                        if (data.ContainsKey(kvp.Key))
                            data.Remove(kvp.Key);

                        data.Add(kvp.Key, kvp.Value);
                    }

                });
            }
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            SaleManager.InitSales();
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            foreach (Sale sale in SaleManager.Sales)
            {
                if (sale.isSaleDay())
                    sale.SendLetter();
            }
        }
    }
}
