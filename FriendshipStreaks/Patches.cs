using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Network;
using System.IO;
using static StardewValley.Menus.SocialPage;

namespace FriendshipStreaks
{
    public static class Patches
    {
        private static float counter = 0;
        public static void Postfix_drawNPCSlot(SocialPage __instance, SpriteBatch b, int i)
        {
            List<ClickableTextureComponent> sprites = ModEntry.instance.Helper.Reflection.GetField<List<ClickableTextureComponent>>(__instance, "sprites").GetValue();
            ClickableTextureComponent sprite = sprites[i];
            SocialEntry entry = __instance.GetSocialEntry(i);


            NPC npc = entry.Character as NPC;
            FriendshipStreak streak = ModEntry.streaks[npc.Name];

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

            ModEntry.instance.Monitor.Log($"Adding +1 to {__instance.Name}'s gift streak");
            ModEntry.streaks[__instance.Name].UpdateGiftStreak();
        }

        public static bool Prefix_grantConversationFriendship(NPC __instance, Farmer who,  int amount)
        {
            if (!who.hasPlayerTalkedToNPC(__instance.Name))
                ModEntry.streaks[__instance.Name].UpdateTalkingStreak();

            return true;
        }

        public static void Postfix_drawProfileMenu(ProfileMenu __instance, SpriteBatch b)
        {
            //Gift
            Vector2 positionMaxGiftStreak = __instance.nextCharacterButton.getVector2() + new Vector2(15, -90);
            float giftIconScale = 2.5f;
            string characterName = __instance.Current.Character.Name;
            FriendshipStreak streak = ModEntry.streaks[characterName];
            string highestStreak = "110";
            string current = "Current";

            counter += 0.01f;
            b.DrawString(Game1.dialogueFont, "Max", positionMaxGiftStreak, Color.Black, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 1f);
            var offset = Math.Abs(Math.Sin(counter));
            b.DrawString(Game1.dialogueFont, current, positionMaxGiftStreak + new Vector2(40 - SpriteText.getWidthOfString(current) / 2, 150), Color.Lerp(Color.Red, Color.Yellow, (float)offset), 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 1f);
            b.DrawString(Game1.dialogueFont, highestStreak, positionMaxGiftStreak + new Vector2(20 - SpriteText.getWidthOfString(highestStreak) / 2, 40), Color.Black, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 1f);
            b.Draw(ModEntry.gameCursors, new Vector2(positionMaxGiftStreak.X - 40, positionMaxGiftStreak.Y), new Rectangle(229, 410, 14, 14), Color.White, 0f, Vector2.Zero, giftIconScale, SpriteEffects.None, 333f);

            //Bubble
            Vector2 positionMaxTalkingStreak = __instance.previousCharacterButton.getVector2() + new Vector2(15, -90);
            float speechBubbleScale = 2.5f;
            characterName = __instance.Current.Character.Name;
            streak = ModEntry.streaks[characterName];
            b.DrawString(Game1.dialogueFont, "Max", positionMaxTalkingStreak, Color.Black, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 1f);
            b.DrawString(Game1.dialogueFont, streak.HighestTalkingStreak.ToString(), positionMaxTalkingStreak + new Vector2(0, 40), Color.Black, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 1f);
            b.Draw(ModEntry.gameCursors, new Vector2(positionMaxTalkingStreak.X - 40, positionMaxTalkingStreak.Y), new Rectangle(66, 4, 14, 12), Color.White, 0f, Vector2.Zero, speechBubbleScale, SpriteEffects.None, 333f);
        }
    }
}
