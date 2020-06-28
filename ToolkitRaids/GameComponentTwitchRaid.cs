using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using SirRandoo.ToolkitRaids.Workers;
using ToolkitCore.Models;
using UnityEngine;
using Verse;
using Random = UnityEngine.Random;

namespace SirRandoo.ToolkitRaids
{
    public class Raid : IExposable
    {
        public List<string> Army = new List<string>();
        public string Leader;
        public float Timer;

        public Raid(string leader)
        {
            Leader = leader;
        }

        public int TotalTroops => Army.Count + 1;

        public void ExposeData()
        {
            Scribe_Deep.Look(ref Timer, "timer");
            Scribe_Deep.Look(ref Leader, "leader");
            Scribe_Collections.Look(ref Army, "army", LookMode.Value);
        }
    }

    public class GameComponentTwitchRaid : GameComponent
    {
        private int _marker;
        private List<Raid> _raids = new List<Raid>();

        public GameComponentTwitchRaid(Game game)
        {
        }

        public List<Raid> AllRaidsForReading => _raids.ToList();

        public override void GameComponentTick()
        {
            while (!ToolkitRaids.RecentRaids.IsEmpty)
            {
                if (!ToolkitRaids.RecentRaids.TryDequeue(out var result))
                {
                    break;
                }

                if (result.NullOrEmpty())
                {
                    RaidLogger.Warn("Received an invalid raider.");
                    continue;
                }

                if (Settings.MergeRaids)
                {
                    var existing = _raids.FirstOrDefault();

                    if (existing == null)
                    {
                        _raids.Add(new Raid(result) {Timer = Settings.Duration});
                    }
                    else
                    {
                        existing.Army.Add(result);
                    }
                }
                else
                {
                    if (_raids.Any(l => l.Leader.Equals(result)))
                    {
                        RaidLogger.Warn("Received a duplicate raid.");
                        continue;
                    }

                    _raids.Add(new Raid(result) {Timer = Settings.Duration});
                }
            }

            if (_raids.Any() && !Find.WindowStack.IsOpen(typeof(RaidDialog)))
            {
                Find.WindowStack.Add(new RaidDialog());
            }

            if (Mathf.FloorToInt(Time.unscaledTime) <= _marker)
            {
                return;
            }

            _marker = Mathf.FloorToInt(Time.unscaledTime);

            for (var index = _raids.Count - 1; index >= 0; index--)
            {
                var raid = _raids[index];

                raid.Timer -= 1;

                if (raid.Timer > 0f)
                {
                    continue;
                }

                var map = Current.Game.Maps.Where(m => m.IsPlayerHome).RandomElementWithFallback();

                if (map == null)
                {
                    continue;
                }

                var defaultParms =
                    new TwitchRaidParms(StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, map))
                    {
                        TwitchRaid = raid
                    };
                var tellerPoints = defaultParms.points;
                var twitchPoints = Settings.PointsPerPerson * (raid.Army.Count + 1);
                var diff = Mathf.Abs(tellerPoints - twitchPoints);
                var factor = Mathf.Clamp(Mathf.Round((raid.Army.Count + 1f) / 10f), 10f, 100f)
                             + Random.Range(0.75f, 1.5f);
                var finalPoints = Mathf.Clamp(
                    twitchPoints * (diff / tellerPoints * factor),
                    twitchPoints,
                    Settings.MaximumAllowedPoints
                );

                defaultParms.TwitchRaid = raid;
                defaultParms.customLetterLabel = "ToolkitRaids.Letters.Title".Translate(raid.Leader.CapitalizeFirst());
                defaultParms.forced = true;
                defaultParms.raidNeverFleeIndividual = true;
                defaultParms.points = finalPoints;
                defaultParms.pawnCount = raid.Army.Count + 1;
                defaultParms.faction = Find.FactionManager.AllFactionsVisibleInViewOrder
                    .Where(f => !f.IsPlayer)
                    .Where(f => f.PlayerRelationKind == FactionRelationKind.Hostile)
                    .Where(f => f.def.humanlikeFaction)
                    .RandomElementWithFallback(defaultParms.faction);

                var worker = new TwitchRaidWorker {def = IncidentDefOf.RaidEnemy};

                try
                {
                    worker.TryExecute(defaultParms);
                }
                catch (Exception e)
                {
                    RaidLogger.Error("Could not execute raid worker", e);
                }
                finally
                {
                    _raids.RemoveAt(index);
                }
            }
        }

        public void ForceCloseRegistry()
        {
            foreach (var raid in _raids)
            {
                raid.Timer = -10f;
            }
        }

        public bool CanJoinRaid()
        {
            return _raids.Any(r => r.Timer > 0f);
        }

        public void RegisterRaid(Raid raid)
        {
            _raids.Add(raid);
        }

        public bool TryJoinRaid(Viewer viewer)
        {
            if (Enumerable.Any(_raids, r => r.Leader.EqualsIgnoreCase(viewer.Username)))
            {
                return false;
            }

            if (Enumerable.Any(_raids, r => r.Army.Any(s => s.Equals(viewer.Username))))
            {
                return false;
            }

            if (!_raids.TryRandomElement(out var raid))
            {
                return false;
            }

            raid.Army.Add(viewer.Username);
            return true;
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref _raids, "pendingRaids", LookMode.Value);
        }
    }
}
