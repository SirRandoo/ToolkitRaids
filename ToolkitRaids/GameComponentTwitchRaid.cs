using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SirRandoo.ToolkitRaids.Models;
using ToolkitCore;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitRaids
{
    [UsedImplicitly]
    public class GameComponentTwitchRaid : GameComponent
    {
        private Raid _lastRaid;
        private float _marker;
        private List<Raid> _raids = new List<Raid>();

        public GameComponentTwitchRaid(Game game) { }

        [NotNull] public List<Raid> AllRaidsForReading => _raids.ToList();
        [NotNull] public IEnumerable<Raid> AllActiveRaids => _raids.Where(r => r.Timer > 0);

        public override void GameComponentUpdate()
        {
            ProcessRaidQueue();
            ProcessViewerQueue();

            if (AllActiveRaids.Any() && !Find.WindowStack.IsOpen(typeof(RaidDialog)))
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
                if (!ToolkitRaids.RecentRaids.TryDequeue(out RaidLeader result))
                {
                    break;
                }

                if (result.ViewerCount < Settings.MinimumRaiders && !result.Generated)
                {
                    continue;
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

        private void ProcessRaid(RaidLeader result)
        {
            if (Settings.MergeRaids)
            {
                Raid existing = _raids.FirstOrDefault();

                if (existing == null)
                {
                    _raids.Add(new Raid {Leader = result.Username, Timer = Settings.Duration});
                }
                else
                {
                    existing.Army.Add(result.Username);
                }
            }
            else
            {
                if (_raids.Any(l => l.Leader.Equals(result.Username)))
                {
                    RaidLogger.Warn("Received a duplicate raid.");
                    return;
                }

                _raids.Add(new Raid {Leader = result.Username, Timer = Settings.Duration});
            }

            if (Settings.SendMessage && !Settings.MessageToSend.NullOrEmpty() && !result.Generated)
            {
                TwitchWrapper.SendChatMessage(
                    Settings.MessageToSend.Replace("%raider%", result.Username)
                       .Replace("%viewers%", result.ViewerCount.ToString("N0"))
                );
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

        public void RunLastRaid()
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
}
