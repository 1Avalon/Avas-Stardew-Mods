using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event_Sales
{
    public class Sale
    {
        public Season Season;

        public int Day;

        public float Multiplier;

        public int TargetCategory;

        public string targetShopId;

        public string LetterContent;

        public string LetterId;

        private Dictionary<string, Season> seasons = new Dictionary<string, Season>()
        {
            ["summer"] = Season.Summer,
            ["winter"] = Season.Winter,
            ["fall"] = Season.Fall,
            ["spring"] = Season.Spring
        };

        public Sale(string Season, float Multiplier, string TargetShopId, string LetterContent, string LetterId, int targetCategory = 0)
        {

            if (seasons.TryGetValue(Season.ToLower(), out Season targetSeason))
                this.Season = targetSeason;

            else
                ModEntry.Instance.Monitor.Log($"Season '{Season}' couldn't be parsed for targetshop '{TargetShopId}.", StardewModdingAPI.LogLevel.Error);

            this.Multiplier = Multiplier;
            this.TargetCategory = targetCategory;
            TargetCategory = targetCategory;
            targetShopId = TargetShopId;
            this.LetterContent = LetterContent;
            this.LetterId = LetterId;
        }

        public bool isSaleDay()
        {
            return Game1.season == Season && Game1.dayOfMonth == Day;
        }

        public void SendLetter()
        {
            Game1.mailbox.Add(LetterId);
        }
    }
}
