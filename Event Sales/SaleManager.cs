using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event_Sales
{
    public static class SaleManager
    {
        public static List<Sale> Sales = new List<Sale>();

        public static Dictionary<string, string> MailData = new Dictionary<string, string>();

        public static void InitSales()
        {
            Sales.Clear();
            foreach (var kvp in ModEntry.Instance.Helper.GameContent.Load<Dictionary<string, Sale>>(ModEntry.SALE_KEY))
            {
                Sales.Add(kvp.Value);
                MailData.Add(kvp.Value.LetterId, kvp.Value.LetterContent);
            }
        }
    }
}
