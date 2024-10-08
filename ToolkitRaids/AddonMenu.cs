﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SirRandoo.ToolkitRaids.Models;
using SirRandoo.ToolkitRaids.Windows;
using ToolkitCore;
using ToolkitCore.Interfaces;
using ToolkitCore.Windows;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Extensions;
using Verse;

namespace SirRandoo.ToolkitRaids;

[UsedImplicitly]
internal class AddonMenu : IAddonMenu
{
    private static readonly List<FloatMenuOption> Options =
    [
        new FloatMenuOption("ToolkitRaids.AddonMenu.Settings".TranslateSimple(), OpenSettingsWindow),
        new FloatMenuOption("ToolkitRaids.AddonMenu.ForceNoRegister".TranslateSimple(), CloseRegistration),
        new FloatMenuOption("ToolkitRaids.AddonMenu.ForceNewRaid".TranslateSimple(), ForceNewRaid),
        new FloatMenuOption("ToolkitRaids.AddonMenu.ForceNewRaidLarge".TranslateSimple(), ForceNewLargeRaid),
        new FloatMenuOption("ToolkitRaids.AddonMenu.ExecuteLastRaid".TranslateSimple(), ReplayLastRaid),
        new FloatMenuOption("ToolkitRaids.AddonMenu.QueueDebugRaid".TranslateSimple(), QueueDebugRaid),
        new FloatMenuOption("ToolkitRaids.AddonMenu.BuildRaid".TranslateSimple(), BuildRaid)
    ];

    private static void BuildRaid()
    {
        Find.WindowStack.Add(new RaidBuilderWindow());
    }

    public List<FloatMenuOption> MenuOptions() => Options;

    private static void ReplayLastRaid()
    {
        RaidLogger.Info("Reviving last raid...");
        Current.Game.GetComponent<GameComponentTwitchRaid>()?.RunLastRaid();
    }

    private static void ForceNewLargeRaid()
    {
        RaidLogger.Info("Forcibly starting a new large raid...");

        var component = Current.Game.GetComponent<GameComponentTwitchRaid>();

        if (component == null)
        {
            return;
        }

        var raid = new Raid { Leader = RaidMod.GenerateNameForRaid() };

        for (var index = 0; index < Rand.Range(20, 50); index++)
        {
            raid.Recruit(RaidMod.GenerateNameForRaid());
        }

        component.RegisterRaid(raid);
    }

    private static void ForceNewRaid()
    {
        string result = RaidMod.GenerateNameForRaid();

        RaidMod.RecentRaids.Enqueue(new RaidLeader { Username = result, ViewerCount = RaidMod.Instance.Settings.MinimumRaiders + 1, Generated = true });
        RaidLogger.Info($"""Scheduled a new raid with leader "{result}".""");
    }

    private static void OpenSettingsWindow()
    {
        ProxySettingsWindow.Open(RaidMod.Instance.SettingsWindow);
    }

    private static void CloseRegistration()
    {
        RaidLogger.Warn("Forcibly closing registration for all pending raids...");
        Current.Game?.GetComponent<GameComponentTwitchRaid>()?.ForceCloseRegistry();
    }

    private static void QueueDebugRaid()
    {
        RaidLogger.Info("Queued debug raid...");

        TwitchWrapper.Client.InvokeRaidNotification(
            ToolkitCoreSettings.channel_username,
            [],
            [],
            "#FFFFFF",
            ToolkitCoreSettings.bot_username,
            "",
            Guid.NewGuid().ToString(),
            ToolkitCoreSettings.bot_username,
            true,
            Guid.NewGuid().ToString(),
            ToolkitCoreSettings.bot_username,
            ToolkitCoreSettings.bot_username,
            RaidMod.Instance.Settings.MinimumRaiders.ToString(),
            Guid.NewGuid().ToString(),
            false,
            "",
            "",
            "",
            false,
            UserType.Moderator,
            Guid.NewGuid().ToString()
        );
    }
}
