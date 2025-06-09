using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using static StardewValley.Menus.SocialPage;

namespace FriendshipBars
{
    public static class Patches
    {
        private static Texture2D rectText;
        public static void Postfix_drawNPCSlot(SocialPage __instance, SpriteBatch b, int i)
        {
            SocialEntry entry = __instance.GetSocialEntry(i);

            if (entry.Friendship == null)
                return;

            float currentPoints = entry.Friendship.Points % 250;
            float requiredPoints = 250;

            if (rectText ==  null)
            {
                rectText = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
                rectText.SetData(new Color[] { new Color(216, 55, 0) });
            }
            b.Draw(ModEntry.progressBar, new Vector2(__instance.xPositionOnScreen + 320 - 4 + 32 * 5 - 46, __instance.sprites[i].bounds.Y), null, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 5f);

            float completion = currentPoints / requiredPoints;

            int progressBarWidth = (int)Math.Floor(completion * 42 * 2);

            b.Draw(rectText, new Rectangle(__instance.xPositionOnScreen + 320 - 4 + 32 * 5 - 46 + 4, __instance.sprites[i].bounds.Y + 4, progressBarWidth, 16), Color.White);
        }
    }
}
