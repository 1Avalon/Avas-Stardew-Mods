using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StardewValley.Menus.SocialPage;

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
        }
    }
}
