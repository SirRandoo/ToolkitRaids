using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SirRandoo.ToolkitRaids.Models;
using ToolkitCore;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitRaids;

[UsedImplicitly]
public class GameComponentTwitchRaid : GameComponent
{
    private Raid? _lastRaid;
    private float _marker;
    private List<Raid> _raids = [];

    public GameComponentTwitchRaid(Game game)
    {
    }

    internal IEnumerable<Raid> GetActiveRaids()
    {
        for (var i = 0; i < _raids.Count; i++)
        {
            Raid raid = _raids[i];

            if (raid.Timer <= 0)
            {
                yield return raid;
            }
        }
    }

    public override void GameComponentUpdate()
    {
        ProcessRaidQueue();
        ProcessViewerQueue();

        if (GetActiveRaids().Any() && !Find.WindowStack.IsOpen(typeof(RaidDialog)))
        {
            Find.WindowStack.Add(new RaidDialog());
        }

        if (_marker <= 0)
        {
            _marker = Time.unscaledTime;
        }

        foreach (Raid raid in _raids)
        {
            raid.Timer -= Time.unscaledTime - _marker;

            if (raid.Timer > 0)
            {
                continue;
            }

            try
            {
                _lastRaid = raid;
                raid.Spawn();
            }
            catch (Exception e)
            {
                RaidLogger.Error("Could not spawn raid", e);
            }
        }

        _marker = Time.unscaledTime;
        _raids.RemoveAll(static r => r.Timer <= 0);
    }

    private void ProcessViewerQueue()
    {
        if (RaidMod.ViewerQueue.IsEmpty || !RaidMod.ViewerQueue.TryDequeue(out string viewer) || string.IsNullOrEmpty(viewer))
        {
            return;
        }

        RecruitViewer(viewer);
    }

    private void ProcessRaidQueue()
    {
        if (RaidMod.RecentRaids.IsEmpty || !RaidMod.RecentRaids.TryDequeue(out RaidLeader leader))
        {
            return;
        }

        if (leader.ViewerCount < RaidMod.Instance.Settings.MinimumRaiders && !leader.Generated)
        {
            return;
        }

        ProcessRaid(leader);
    }

    private void RecruitViewer(string viewer)
    {
        if (!CanJoinRaid(viewer))
        {
            return;
        }

        var chance = 1f;

        for (var i = 0; i < _raids.Count; i++)
        {
            if (!Rand.Chance(chance))
            {
                continue;
            }

            Raid raid = _raids[i];

            if (raid.Timer <= 0 || string.Equals(raid.Leader, viewer, StringComparison.OrdinalIgnoreCase) || raid.Army.Contains(viewer))
            {
                continue;
            }

            raid.Recruit(viewer);
            chance -= Mathf.Clamp(Rand.Range(0.05f, 0.5f), 0f, 1f);
        }
    }

    private void ProcessRaid(RaidLeader result)
    {
        if (RaidMod.Instance.Settings.MergeRaids)
        {
            Raid? existing = _raids.FirstOrDefault();

            if (existing == null)
            {
                _raids.Add(new Raid { Leader = result.Username, Timer = RaidMod.Instance.Settings.Duration });
            }
            else
            {
                existing.Recruit(result.Username);
            }
        }
        else
        {
            if (_raids.Any(l => l.Leader.Equals(result.Username)))
            {
                RaidLogger.Warn("Received a duplicate raid.");

                return;
            }

            _raids.Add(new Raid { Leader = result.Username, Timer = RaidMod.Instance.Settings.Duration });
        }

        if (RaidMod.Instance.Settings.SendMessage && !RaidMod.Instance.Settings.MessageToSend.NullOrEmpty() && !result.Generated)
        {
            TwitchWrapper.SendChatMessage(
                RaidMod.Instance.Settings.MessageToSend.Replace("%raider%", result.Username).Replace("%viewers%", result.ViewerCount.ToString("N0"))
            );
        }
    }

    internal void ForceCloseRegistry()
    {
        foreach (Raid raid in _raids)
        {
            raid.Timer = -10f;
        }
    }

    private bool CanJoinRaid(string viewer)
    {
        for (var i = 0; i < _raids.Count; i++)
        {
            Raid raid = _raids[i];

            if (raid.Timer <= 0 || string.Equals(raid.Leader, viewer, StringComparison.OrdinalIgnoreCase) || raid.Army.Contains(viewer))
            {
                continue;
            }

            return true;
        }

        return false;
    }

    internal void RegisterRaid(Raid raid)
    {
        _raids.Add(raid);
    }

    internal void RunLastRaid()
    {
        if (_lastRaid == null)
        {
            return;
        }

        RegisterRaid(_lastRaid);
    }

    public override void ExposeData()
    {
        Scribe_Deep.Look(ref _lastRaid, "lastRaid");
        Scribe_Collections.Look(ref _raids, "pendingRaids", LookMode.Deep);
    }
}
