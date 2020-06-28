using System.Collections.Concurrent;
using System.Reflection;
using HarmonyLib;
using ToolkitCore;
using TwitchLib.Client.Events;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitRaids
{
    public class ToolkitRaids : Mod
    {
        public static readonly ConcurrentQueue<string> RecentRaids = new ConcurrentQueue<string>();
        private static Harmony _harmony;

        public ToolkitRaids(ModContentPack content) : base(content)
        {
            GetSettings<Settings>();

            _harmony = new Harmony("com.sirrandoo.tkraids");
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.Draw(inRect);
        }

        public override string SettingsCategory()
        {
            return nameof(ToolkitRaids);
        }

        internal static void OnRaidNotification(object sender, OnRaidNotificationArgs args)
        {
            RecentRaids.Enqueue(args.RaidNotification.Login);
        }
    }

    [HarmonyPatch(typeof(TwitchWrapper), "InitializeClient")]
    public static class TwitchClientPatch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (TwitchWrapper.Client == null)
            {
                return;
            }

            TwitchWrapper.Client.OnRaidNotification += ToolkitRaids.OnRaidNotification;
        }
    }
}
