using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event_Sales
{
    public static class Patches
    {
        public static void Prefix_PlayOpenSound(ShopMenu __instance) // patching this method because it only gets called once when opening the menu, just injecting the code here
        {
            Sale appliedSale = null;
            foreach (Sale sale in ModEntry.TodaySales)
            {
                if (sale.targetShopId == __instance.ShopId)
                {
                    appliedSale = sale;
                    break;
                }
            }

            if (appliedSale == null)
                return;

            foreach (var kvp in __instance.itemPriceAndStock)
            {
                int category = ItemRegistry.Create(kvp.Key.QualifiedItemId).Category;

                if (category == appliedSale.TargetCategory)
                    kvp.Value.Price = appliedSale.CalculateNewPrice(kvp.Value.Price);
            }
        }
    }
}
