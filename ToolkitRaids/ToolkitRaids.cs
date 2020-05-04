using System.Collections.Concurrent;
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

        public override void DoSettingsWindowContents(Rect inRect) => Settings.Draw(inRect);

        public override string SettingsCategory() => nameof(ToolkitRaids);
    }

    public static class ToolkitRaidsStatic
    {
        static ToolkitRaidsStatic()
        {
            ToolkitCore.TwitchWrapper.Client.OnRaidNotification += OnRaidNotification;
        }

        private static void OnRaidNotification(object sender, OnRaidNotificationArgs args)
        {
            ToolkitRaids.RecentRaids.Enqueue(args.RaidNotification.Login);
        }
    }
}
