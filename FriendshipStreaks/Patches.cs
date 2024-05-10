using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StardewValley.Menus.SocialPage;

namespace FriendshipStreaks
{
    public static class Patches
    {
        public static void Postfix_drawNPCSlot(SocialPage __instance, SpriteBatch b, int i)
        {
            List<ClickableTextureComponent> sprites = ModEntry.instance.Helper.Reflection.GetField<List<ClickableTextureComponent>>(__instance, "sprites").GetValue();
            ClickableTextureComponent sprite = sprites[i];
            SocialEntry entry = __instance.GetSocialEntry(i);

            FriendshipStreak streak = ModEntry.streaks[(NPC)entry.Character];

            //Talking Streak
            Vector2 textPosition = new Vector2(sprite.bounds.Left + 60, sprite.bounds.Top - 10);
            float speechBubbleScale = 2.5f;
            b.Draw(ModEntry.gameCursors,new Vector2(textPosition.X + 40, textPosition.Y + 5), new Rectangle(66, 4, 14, 12), Color.White, 0f, Vector2.Zero, speechBubbleScale, SpriteEffects.None, 333f);
            b.DrawString(Game1.dialogueFont, streak.CurrentTalkingStreak.ToString(), textPosition, Color.Black, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 1f);

            //Gift Streak
            textPosition = textPosition + new Vector2(100, 0);
            float giftIconScale = 2.5f;
            b.Draw(ModEntry.gameCursors, new Vector2(textPosition.X + 40, textPosition.Y + 5), new Rectangle(229, 410, 14, 14), Color.White, 0f, Vector2.Zero, giftIconScale, SpriteEffects.None, 333f);
            b.DrawString(Game1.dialogueFont, streak.CurrentGiftStreak.ToString(), textPosition, Color.Black, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 1f);
        }

        public static void Postfix_receiveGift(NPC __instance)
        {
            if (!__instance.CanReceiveGifts())
                return;

            ModEntry.streaks[__instance].UpdateGiftStreak();
        }
    }
}
