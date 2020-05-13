using System.Collections.Concurrent;
using ToolkitCore;
using TwitchLib.Client.Events;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitRaids
{
    public class ToolkitRaids : Mod
    {
        public static ConcurrentQueue<string> RecentRaids = new ConcurrentQueue<string>();

        public ToolkitRaids(ModContentPack content) : base(content)
        {
            GetSettings<Settings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.Draw(inRect);
        }

        public override string SettingsCategory()
        {
            return nameof(ToolkitRaids);
        }
    }

    [StaticConstructorOnStartup]
    public static class ToolkitRaidsStatic
    {
        static ToolkitRaidsStatic()
        {
            TwitchWrapper.Client.OnRaidNotification += OnRaidNotification;
        }

        private static void OnRaidNotification(object sender, OnRaidNotificationArgs args)
        {
            ToolkitRaids.RecentRaids.Enqueue(args.RaidNotification.Login);
        }
    }
}
