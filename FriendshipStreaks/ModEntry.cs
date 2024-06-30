using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using HarmonyLib;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Graphics;
using System.Linq.Expressions;
using System.IO;

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

            Helper.Events.GameLoop.Saving += OnSaving;
            Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            Helper.Events.GameLoop.DayStarted += OnDayStarted;

            Helper.ConsoleCommands.Add("set_streak", "Sets the streak for an NPC to the given value.\nUsage: set_streak <type> <NPC name> <value>\n- type - must be either 'gift' or 'talking'\n- NPC name - the name of your target\n- value - amount of the desired streak.", this.SetStreak);

            instance = this;
            gameCursors = Helper.GameContent.Load<Texture2D>("LooseSprites/Cursors");
            Harmony harmony = new(this.ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(SocialPage), "drawNPCSlot"),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.Postfix_drawNPCSlot)),
                transpiler: new HarmonyMethod(typeof(Patches), nameof(Patches.Transpiler))
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

            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.changeFriendship)),
                prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.Prefix_changeFriendship))
                );
        }
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            foreach (KeyValuePair<string, FriendshipStreak> kvp in streaks)
            {
                kvp.Value.ResetStreaksIfMissed();
            }
        }
        private void OnSaving(object sender, SavingEventArgs e)
        {
            foreach (KeyValuePair<string, FriendshipStreak> kvp in streaks)
            {
                Helper.Data.WriteSaveData(kvp.Key, kvp.Value);
                Monitor.Log($"Saved Streak data for {kvp.Key}");
            }
        }

        private void SetStreak(string command, string[] args)
        {

            if (args.Length < 3)
                return;

            string type = args[0].ToLower();
            string npcName = args[1];
            int value = int.Parse(args[2]);
            
            switch(type)
            {
                case "gift":
                    streaks[npcName].CurrentGiftStreak = value;
                    Monitor.Log("Success", LogLevel.Info);
                    break;

                case "talking":
                    streaks[npcName].CurrentTalkingStreak = value;
                    Monitor.Log("Success", LogLevel.Info);
                    break;

                default:
                    Monitor.Log("First argument must be 'gift' or 'talking'", LogLevel.Error);
                    break;
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