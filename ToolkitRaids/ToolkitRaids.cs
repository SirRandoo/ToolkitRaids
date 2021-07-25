using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitRaids.Models;
using ToolkitCore;
using TwitchLib.Client.Events;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitRaids
{
    [UsedImplicitly]
    public class ToolkitRaids : Mod
    {
        internal static List<string> SpecialNames;

        public static readonly ConcurrentQueue<RaidLeader> RecentRaids = new ConcurrentQueue<RaidLeader>();
        public static readonly ConcurrentQueue<string> ViewerQueue = new ConcurrentQueue<string>();

        public ToolkitRaids(ModContentPack content) : base(content)
        {
            GetSettings<Settings>();

            new Harmony("com.sirrandoo.tkraids").PatchAll(Assembly.GetExecutingAssembly());
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.Draw(inRect);
        }

        [NotNull]
        public override string SettingsCategory()
        {
            return nameof(ToolkitRaids);
        }

        internal static void OnRaidNotification(object sender, [NotNull] OnRaidNotificationArgs args)
        {
            var leader = new RaidLeader {Username = args.RaidNotification.Login};

            if (!int.TryParse(args.RaidNotification.MsgParamViewerCount, out int count))
            {
                RaidLogger.Warn(
                    $"Could not parse viewer count of {args.RaidNotification.MsgParamViewerCount}. Defaulted to 1"
                );
                count = 1;
            }

            leader.ViewerCount = count;

            RecentRaids.Enqueue(leader);
        }

        internal static string GenerateNameForRaid()
        {
            return UnityData.IsInMainThread && Rand.Chance(0.05f)
                ? SpecialNames.RandomElement()
                : Path.GetRandomFileName().Replace(".", "").Substring(0, 8);
        }
    }

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [HarmonyPatch(typeof(TwitchWrapper), "InitializeClient")]
    public static class TwitchClientPatch
    {
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
