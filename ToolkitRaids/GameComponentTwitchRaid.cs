using System;
using System.CodeDom;
using System.Collections.Generic;
using RimWorld;
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
