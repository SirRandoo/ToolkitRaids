using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitRaids.Workers;
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

        public string ArmyCountLabel { get; private set; }

        public int TotalTroops => Army.Count + 1;

        public void ExposeData()
        {
            Scribe_Deep.Look(ref Timer, "timer");
            Scribe_Deep.Look(ref Leader, "leader");
            Scribe_Collections.Look(ref Army, "army", LookMode.Value);
        }

        public void Recruit(string viewer)
        {
            Army.Add(viewer);
            ArmyCountLabel = Army.Count.ToString("N0");
        }

        public void Unrecruit(string viewer)
        {
            Army.RemoveAll(v => v.EqualsIgnoreCase(viewer));
            ArmyCountLabel = Army.Count.ToString("N0");
        }

        internal void Tick()
        {
            Timer -= 1;

            if (Timer <= 0)
            {
                Spawn();
            }
        }

        private void Spawn()
        {
            Map map = Current.Game.Maps.Where(m => m.IsPlayerHome).RandomElementWithFallback();

            if (map == null)
            {
                return;
            }

            TwitchRaidParms defaultParms = TwitchRaidParms.ForRaid(this, map);

            if (!Settings.UseStoryteller)
            {
                float tellerPoints = defaultParms.points;
                float twitchPoints = Settings.PointsPerPerson * (Army.Count + 1);
                float diff = tellerPoints - twitchPoints;
                float finalPoints = twitchPoints;

                if (diff > tellerPoints * 0.95f)
                {
                    RaidLogger.Debug("Point differential too high!");

                    float factor = Mathf.Clamp(Mathf.Round((Army.Count + 1f) / 10f), 10f, 100f)
                                   + Random.Range(0.75f, 1.5f);
                    finalPoints = Mathf.Clamp(
                        twitchPoints * (diff / tellerPoints * factor),
                        twitchPoints,
                        Settings.MaximumAllowedPoints
                    );

                    RaidLogger.Warn(
                        $"Adjusted the raid's points from {twitchPoints:N2} to {finalPoints:N2} (Storyteller points: {tellerPoints:N2})"
                    );
                }

                RaidLogger.Debug($"Teller points: {tellerPoints:N4}");
                RaidLogger.Debug($"ToolkitRaid points: {twitchPoints:N4}");
                RaidLogger.Debug($"Differential: {diff:N4}");
                RaidLogger.Debug($"Final points: {finalPoints:N4}");
                defaultParms.points = finalPoints;
            }

            defaultParms.TwitchRaid = this;
            defaultParms.customLetterLabel = "ToolkitRaids.Letters.Title".Translate(Leader.CapitalizeFirst());
            defaultParms.forced = true;
            defaultParms.raidNeverFleeIndividual = true;
            defaultParms.pawnCount = Army.Count + 1;
            defaultParms.faction = Find.FactionManager.AllFactionsVisibleInViewOrder.Where(f => !f.IsPlayer)
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
        }
    }

    [UsedImplicitly]
    public class GameComponentTwitchRaid : GameComponent
    {
        private int _marker;
        private List<Raid> _raids = new List<Raid>();

        public GameComponentTwitchRaid(Game game) { }

        public List<Raid> AllRaidsForReading => _raids.ToList();

        public override void GameComponentTick()
        {
            ProcessRaidQueue();
            ProcessViewerQueue();

            if (_raids.Count > 0 && !Find.WindowStack.IsOpen(typeof(RaidDialog)))
            {
                Find.WindowStack.Add(new RaidDialog());
            }

            int currentTime = Mathf.FloorToInt(Time.unscaledTime);
            if (currentTime <= _marker)
            {
                return;
            }

            _marker = currentTime;

            foreach (Raid raid in _raids)
            {
                raid.Tick();
            }

            _raids.RemoveAll(r => r.Timer <= 0);
        }

        private void ProcessViewerQueue()
        {
            while (!ToolkitRaids.ViewerQueue.IsEmpty)
            {
                if (!ToolkitRaids.ViewerQueue.TryDequeue(out string result) || result.NullOrEmpty())
                {
                    break;
                }

                RecruitViewer(result);
            }
        }

        private void ProcessRaidQueue()
        {
            while (!ToolkitRaids.RecentRaids.IsEmpty)
            {
                if (!ToolkitRaids.RecentRaids.TryDequeue(out string result) || result.NullOrEmpty())
                {
                    break;
                }

                ProcessRaid(result);
            }
        }

        private void RecruitViewer(string viewer)
        {
            if (!CanJoinRaid(viewer))
            {
                return;
            }

            var chance = 1f;
            foreach (Raid raid in _raids.Where(
                r => r.Timer > 0 && !r.Leader.EqualsIgnoreCase(viewer) && !r.Army.Contains(viewer)
            ))
            {
                if (!Rand.Chance(chance))
                {
                    continue;
                }

                raid.Recruit(viewer);
                chance -= Mathf.Clamp(Rand.Range(0.05f, 0.5f), 0f, 1f);
            }
        }

        private void ProcessRaid(string result)
        {
            if (Settings.MergeRaids)
            {
                Raid existing = _raids.FirstOrDefault();

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
                    return;
                }

                _raids.Add(new Raid(result) {Timer = Settings.Duration});
            }
        }

        public void ForceCloseRegistry()
        {
            foreach (Raid raid in _raids)
            {
                raid.Timer = -10f;
            }
        }

        public bool CanJoinRaid(string viewer)
        {
            return _raids.Where(r => r.Timer > 0f)
               .Any(r => !r.Leader.EqualsIgnoreCase(viewer) && !r.Army.Contains(viewer.ToLowerInvariant()));
        }

        public void RegisterRaid(Raid raid)
        {
            _raids.Add(raid);
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref _raids, "pendingRaids", LookMode.Value);
        }
    }
}
