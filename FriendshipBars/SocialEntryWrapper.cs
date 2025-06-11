using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StardewValley.Menus.SocialPage;
using static System.Net.Mime.MediaTypeNames;

namespace FriendshipBars
{
    public class SocialEntryWrapper
    {
        public SocialEntry entry;

        public int CurrentProgressPoints;

        public int TotalProgressPoints;

        public int CurrentHearts;

        public const int RequiredForNextHeart = 250;

        public float Completion;

        public string HoverText;
        public SocialEntryWrapper(SocialEntry e) 
        {
            entry = e;

            CurrentProgressPoints = 0;
            CurrentHearts = 0;
            TotalProgressPoints = 0;

            if (e.Friendship != null)
            {
                CurrentProgressPoints = e.Friendship.Points % 250;
                CurrentHearts = e.Friendship.Points / 250;
                TotalProgressPoints = CurrentProgressPoints + 250 * CurrentHearts;
            }

            Completion = (float) CurrentProgressPoints / (float) RequiredForNextHeart;
            HoverText = getHoverText();
        }

        private string getHoverText()
        {
            ModConfig c = ModEntry.Config;

            string s = "";

            if (c.CurrentPointsHover)
                s += $"{I18n.Display_CurrentPoints(TotalProgressPoints)}\n";

            if (c.RequiredPointsHover)
                s += $"{I18n.Display_ForNextHeart(RequiredForNextHeart - CurrentProgressPoints)}\n";

            if (c.CompletionHover)
                s += $"{I18n.Display_Completion((Completion * 100).ToString("0.0"))}%\n";

            return s.Substring(0, s.Length - 1);
        }
    }
}
