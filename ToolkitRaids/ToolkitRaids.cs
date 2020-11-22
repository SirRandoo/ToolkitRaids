﻿using System.Collections.Concurrent;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using ToolkitCore;
using TwitchLib.Client.Events;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitRaids
{
    [UsedImplicitly]
    public class ToolkitRaids : Mod
    {
        public static readonly ConcurrentQueue<string> RecentRaids = new ConcurrentQueue<string>();
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

        public override string SettingsCategory()
        {
            return nameof(ToolkitRaids);
        }

        internal static void OnRaidNotification(object sender, OnRaidNotificationArgs args)
        {
            RecentRaids.Enqueue(args.RaidNotification.Login);
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
