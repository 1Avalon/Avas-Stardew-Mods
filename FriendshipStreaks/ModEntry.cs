using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using HarmonyLib;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Graphics;

namespace FriendshipStreaks
{
    /// <summary>The mod entry point.</summary>
    public sealed class ModEntry : Mod
    {
        public static Mod instance;

        public static Texture2D gameCursors;

        public static Dictionary<string, FriendshipStreak> streaks = new Dictionary<string, FriendshipStreak>();
        public override void Entry(IModHelper helper)
        {

            Helper.Events.GameLoop.Saved += OnSaved;
            Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            Helper.Events.GameLoop.DayStarted += OnDayStarted;

            instance = this;
            gameCursors = Helper.GameContent.Load<Texture2D>("LooseSprites/Cursors");
            Harmony harmony = new(this.ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(SocialPage), "drawNPCSlot"),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.Postfix_drawNPCSlot))
                );

            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.receiveGift)),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.Postfix_receiveGift))
                );

            harmony.Patch(
                original: AccessTools.Method(typeof(ProfileMenu), nameof(ProfileMenu.draw), new Type[] { typeof(SpriteBatch)}),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.Postfix_drawProfileMenu))
                );

            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.grantConversationFriendship)),
                prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.Prefix_grantConversationFriendship))
                );
        }
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            foreach (KeyValuePair<string, FriendshipStreak> kvp in streaks)
            {
                kvp.Value.ResetStreaksIfMissed();
            }
        }
        private void OnSaved(object sender, SavedEventArgs e)
        {
            foreach (KeyValuePair<string, FriendshipStreak> kvp in streaks)
            {
                Helper.Data.WriteSaveData(kvp.Key, kvp.Value);
                Monitor.Log($"Saved Streak data for {kvp.Key}");
            }
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            streaks.Clear();
            List<string> npcNames = new List<string>();
            foreach (NPC npc in Utility.getAllVillagers())
            {
                if (!npcNames.Contains(npc.Name))
                {
                    if (!npc.CanSocialize)
                        continue;

                    npcNames.Add(npc.Name);
                    FriendshipStreak streak = Helper.Data.ReadSaveData<FriendshipStreak>(npc.Name);
                    if (streak == null)
                    {
                        Monitor.Log($"No streak found for {npc.Name}. Initialising new one...");
                        streak = new FriendshipStreak(npc.Name, 0, 0, 0, 0);
                    }
                    streaks.Add(npc.Name, streak);
                }
            }
        }
    }
}