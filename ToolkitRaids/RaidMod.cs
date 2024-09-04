using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
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
    private DateTime _settingsWindowCooldown;

    internal static readonly ConcurrentQueue<RaidLeader> RecentRaids = new();
    internal static readonly ConcurrentQueue<string> ViewerQueue = new();

    public RaidMod(ModContentPack content) : base(content)
    {
        Instance = this;
        Settings = GetSettings<Settings>();

        new Harmony("com.sirrandoo.tkraids").PatchAll(Assembly.GetExecutingAssembly());
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
