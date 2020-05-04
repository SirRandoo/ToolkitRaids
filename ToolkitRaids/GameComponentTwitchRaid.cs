using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using ToolkitCore.Controllers;
using ToolkitCore.Models;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitRaids
{
    public class Raid
    {
        public Raid(Viewer leader)
        {
            Leader = leader;
        }

        public float Timer { get; set; }
        public Viewer Leader { get; }
        public List<Viewer> Army { get; } = new List<Viewer>();
    }

    public class GameComponentTwitchRaid : GameComponent
    {
        private List<Raid> _raids = new List<Raid>();

        public GameComponentTwitchRaid(Game game)
        {
        }

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
                    Log.Message("ToolkitRaids :: Received an invalid raider.");
                    continue;
                }

                var viewer = !ViewerController.ViewerExists(result)
                    ? ViewerController.CreateViewer(result)
                    : ViewerController.GetViewer(result);

                if (viewer == null)
                {
                    continue;
                }

                if (Settings.MergeRaids)
                {
                    var existing = _raids.FirstOrDefault();

                    if (existing == null)
                    {
                        _raids.Add(new Raid(viewer));
                    }
                    else
                    {
                        existing.Army.Add(viewer);
                    }
                }
                else
                {
                    if (_raids.Any(l => l.Leader.Username.Equals(viewer.Username)))
                    {
                        Log.Message("ToolkitRaids :: Received a duplicate raid.");
                        continue;
                    }

                    _raids.Add(new Raid(viewer));
                }
            }

            for (var index = _raids.Count - 1; index >= 0; index--)
            {
                var raid = _raids[index];

                raid.Timer -= Time.deltaTime;

                if (raid.Timer > 0f)
                {
                    continue;
                }

                var parms = new IncidentParms
                {
                    forced = true,
                    generateFightersOnly = true,
                    pawnCount = raid.Army.Count + 1,
                    raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn
                };

                var worker = new TwitchRaidWorker {RaidData = raid, def = IncidentDefOf.RaidEnemy};

                try
                {
                    worker.TryExecute(parms);
                }
                catch (Exception e)
                {
                    Log.Error(
                        $"ToolkitRaids :: Could not execute raid worker.\n{e.GetType().Name}({e.Message})\n{e.StackTrace}"
                    );
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

        public bool TryJoinRaid(Viewer viewer)
        {
            foreach (var r in _raids)
            {
                if (r.Army.Any(s => s.Username.Equals(viewer.Username)))
                {
                    return false;
                }
            }

            if (!_raids.TryRandomElement(out var raid))
            {
                return false;
            }

            raid.Army.Add(viewer);
            return true;
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref _raids, "pendingRaids", LookMode.Value);
        }
    }
}
