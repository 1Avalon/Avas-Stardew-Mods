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

        private static Dictionary<SocialEntryWrapper, ClickableTextureComponent> progressLinks = new Dictionary<SocialEntryWrapper, ClickableTextureComponent>();
        public static void Postfix_drawNPCSlot(SocialPage __instance, SpriteBatch b, int i)
        {
            SocialEntryWrapper wrapper = null;

            foreach (var kvp in progressLinks)
            {
                SocialEntry entry = __instance.GetSocialEntry(i);
                if (entry == kvp.Key.entry)
                {
                    wrapper = kvp.Key;
                    break;
                }
            }

            if (rectText ==  null)
            {
                rectText = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
                rectText.SetData(new Color[] { new Color(216, 55, 0) });
            }
            //b.Draw(ModEntry.progressBar, new Vector2(__instance.xPositionOnScreen + 320 - 4 + 32 * 5 - 46, __instance.sprites[i].bounds.Y), null, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 5f);

            progressLinks[wrapper].draw(b);

            int progressBarWidth = (int)Math.Floor(wrapper.Completion * 42 * 2);

            b.Draw(rectText, new Rectangle(__instance.xPositionOnScreen + 320 - 4 + 32 * 5 - 46 + 4, __instance.sprites[i].bounds.Y + 4, progressBarWidth, 16), Color.White);
        }

        public static void Postfix_performHoverAction(SocialPage __instance, int x, int y)
        {
            foreach (var kvp in progressLinks)
            {
                if (kvp.Value.containsPoint(x, y))
                {
                    __instance.hoverText = $"Current Points: {kvp.Key.TotalProgressPoints}\nRequired for next heart: {250 - kvp.Key.CurrentProgressPoints}\nCompletion: {kvp.Key.Completion * 100}% ";
                }
            }
        }

        public static void Postfix_updateSlots(SocialPage __instance)
        {
            progressLinks = new Dictionary<SocialEntryWrapper, ClickableTextureComponent> ();
            for (int i = 0; i < __instance.SocialEntries.Count; i++)
            {
                ClickableTextureComponent component = new ClickableTextureComponent(new Rectangle(__instance.xPositionOnScreen + 320 - 4 + 32 * 5 - 46, __instance.sprites[i].bounds.Y, 46 * 2, 12 * 2), ModEntry.progressBar, new Rectangle(0, 0, 46, 12), 2f)
                {
                    myID = i,
                    downNeighborID = i + 1,
                    upNeighborID = i - 1,
                };

                SocialEntryWrapper entry = new SocialEntryWrapper(__instance.GetSocialEntry(i));

                progressLinks.Add(entry, component);
            }
        }
    }
}
