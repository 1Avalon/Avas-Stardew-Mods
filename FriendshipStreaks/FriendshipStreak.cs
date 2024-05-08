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

        public int lastDayTalked = 0;

        public int lastWeekTalked = 0;

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
        }

        public void UpdateGiftStreak()
        {
            CurrentGiftStreak++;
            if (CurrentGiftStreak > HighestGiftStreak)
                HighestGiftStreak = CurrentGiftStreak;
        }
        public void UpdateTalkingStreak()
        {
            CurrentTalkingStreak++;
            if (CurrentTalkingStreak > HighestTalkingStreak)
            {
                HighestTalkingStreak = CurrentTalkingStreak;
            }
        }
    }
}
