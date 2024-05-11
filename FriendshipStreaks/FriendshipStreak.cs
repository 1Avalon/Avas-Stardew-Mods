using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendshipStreaks
{
    public class FriendshipStreak
    {
        public string NpcName;

        public int CurrentTalkingStreak;

        public int CurrentGiftStreak;

        public int HighestTalkingStreak;

        public int HighestGiftStreak;

        public float Multiplier = 1f;

        public int LastDayTalked = 0;

        public int LastWeekGiftGiven = 0;

        public FriendshipStreak(string npcName, int currentTalkingStreak, int currentGiftStreak, int highestTalkingStreak, int highestGiftStreak)
        {
            NpcName = npcName;
            CurrentTalkingStreak = currentTalkingStreak;
            CurrentGiftStreak = currentGiftStreak;
            HighestTalkingStreak = highestTalkingStreak;
            HighestGiftStreak = highestGiftStreak;
        }

        public void ResetStreaksIfMissed()
        {
            int day = Game1.Date.DayOfMonth;
            int week = day / 7 + 1;
            if (day - 1 > LastDayTalked)
            {
                CurrentTalkingStreak = 0;
            }

            if (week - 1 > LastWeekGiftGiven)
            {
                CurrentGiftStreak = 0;
            }
        }

        public void UpdateGiftStreak()
        {
            CurrentGiftStreak++;
            int day = Game1.Date.DayOfMonth;
            int week = day / 7 + 1;
            LastDayTalked = week;
            if (CurrentGiftStreak > HighestGiftStreak)
                HighestGiftStreak = CurrentGiftStreak;
        }
        public void UpdateTalkingStreak()
        {
            CurrentTalkingStreak++;
            LastDayTalked = Game1.dayOfMonth;
            if (CurrentTalkingStreak > HighestTalkingStreak)
            {
                HighestTalkingStreak = CurrentTalkingStreak;
            }
        }
    }
}
