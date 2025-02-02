using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;
using StardewValley.GameData.Pets;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StardewValley.Menus.LoadGameMenu;

namespace MoreSaveInformation
{
    public static class Patches
    {
        private static void DrawSpouse(Farmer farmer, LoadGameMenu menu, SpriteBatch b, int i)
        {
            string spouseName = farmer.spouse;

            if (spouseName == null)
                return;

            Texture2D spouseTex;
            try
            {
                spouseTex = ModEntry.instance.Helper.GameContent.Load<Texture2D>($"Characters\\{spouseName}");
            }
            catch
            {
                return;
            }

            int yOffset = TextureHelper.FindFirstNonTransparentPixelY( spouseTex );

            int nameWidth = SpriteText.getWidthOfString(farmer.Name);

            Vector2 npcHeadPosition = new Vector2(menu.slotButtons[i].bounds.X + 128 + 36 + nameWidth + 20, menu.slotButtons[i].bounds.Y + 36);

            b.Draw(spouseTex, npcHeadPosition, new Rectangle(0, yOffset, 16, 16), Color.White, 0, Vector2.Zero, 3f, SpriteEffects.None, 6f);
            b.Draw(Game1.objectSpriteSheet, npcHeadPosition - new Vector2(5, 10), new Rectangle(64, 304, 16, 16), Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 8f);
        }

        public static void DrawPet(Farmer farmer, LoadGameMenu menu, SpriteBatch b, int i)
        {
            int petType = Int32.Parse(farmer.whichPetBreed);
            string petBreed = farmer.whichPetType;

            string actualPetType = petType == 0 ? "" : petType.ToString();

            string petDataName = petBreed + actualPetType;

            if (!Pet.TryGetData(farmer.whichPetType, out var data))
                return;

            Texture2D petTexture = null;
            Rectangle sourceRect = Rectangle.Empty;
            foreach (PetBreed breed in data.Breeds)
            {
                if (breed.Id == farmer.whichPetBreed)
                {
                    petTexture = Game1.content.Load<Texture2D>(breed.IconTexture);
                    sourceRect = breed.IconSourceRect;
                    break;
                }
            }

            int nameWidth = SpriteText.getWidthOfString(farmer.Name);

            Vector2 npcHeadPosition = new Vector2(menu.slotButtons[i].bounds.X + 128 + 36 + nameWidth + 20, menu.slotButtons[i].bounds.Y + 36);

            b.Draw(petTexture, npcHeadPosition + new Vector2 (3 * 16, 0), sourceRect, Color.White, 0, Vector2.Zero, 3f, SpriteEffects.None, 6f);

        }

        public static void DrawAchievementCompletion(Farmer farmer, LoadGameMenu menu, SpriteBatch b, int i)
        {
            int completedAchievemnts = farmer.achievements.Count;

            int nameWidth = SpriteText.getWidthOfString(farmer.Name);

            Vector2 pos = new Vector2(menu.slotButtons[i].bounds.X + 128 + 36 + nameWidth + 20, menu.slotButtons[i].bounds.Y + 36) + new Vector2(3 * 16, 0) + new Vector2(3 * 16, 5);
            b.Draw(Game1.mouseCursors, pos, new Rectangle(294, 392, 16, 16), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 6f);

            string text = $"{completedAchievemnts}/{Game1.achievements.Count}";
            float textCenter = Game1.smallFont.MeasureString(text).X / 2f * 0.7f;
            b.DrawString(Game1.smallFont, $"{completedAchievemnts}/{Game1.achievements.Count}", new Vector2((int)pos.X + 16 - textCenter, (int)pos.Y + 30), Color.Black, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 6f);
        }
        public static void Postfix_DrawSaveFileSlot(SaveFileSlot __instance, SpriteBatch b, int i)
        {
            LoadGameMenu menu = ModEntry.instance.Helper.Reflection.GetField<LoadGameMenu>(__instance, "menu").GetValue();

            DrawSpouse(__instance.Farmer, menu, b, i);
            DrawPet(__instance.Farmer, menu, b, i);
            DrawAchievementCompletion(__instance.Farmer, menu, b, i);
        }
    }
}
