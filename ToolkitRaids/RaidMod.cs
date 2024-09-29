using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitRaids.Models;
using SirRandoo.ToolkitRaids.Windows;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitRaids;

[PublicAPI]
[UsedImplicitly]
public class RaidMod : Mod
{
    internal static List<string> SpecialNames = null!;

    internal static readonly ConcurrentQueue<RaidLeader> RecentRaids = new();
    internal static readonly ConcurrentQueue<string> ViewerQueue = new();
    private DateTime _settingsWindowCooldown;

    public RaidMod(ModContentPack content) : base(content)
    {
        Instance = this;
        Settings = GetSettings<Settings>();
    }

    internal Settings Settings { get; }
    internal static RaidMod Instance { get; private set; } = null!;
    internal SettingsWindow SettingsWindow => new(this);

    public override void DoSettingsWindowContents(Rect inRect)
    {
        if (DateTime.UtcNow - _settingsWindowCooldown < TimeSpan.FromMilliseconds(500))
        {
            return;
        }

        ProxySettingsWindow.Open(SettingsWindow);

        _settingsWindowCooldown = DateTime.UtcNow + TimeSpan.FromMilliseconds(500);
    }

    public override string SettingsCategory() => Content.Name;

    internal static string GenerateNameForRaid() => UnityData.IsInMainThread && Rand.Chance(0.05f)
        ? SpecialNames.RandomElement()
        : Path.GetRandomFileName().Replace(".", "")[..8];
}

[UsedImplicitly]
[StaticConstructorOnStartup]
internal static class PatchRunner
{
    private static readonly Harmony Harmony = new("com.sirrandoo.tkraids");

    static PatchRunner()
    {
        Harmony.PatchAll();
    }
}
