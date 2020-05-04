using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using ToolkitCore.Models;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitRaids
{
    public class TwitchRaidWorker : IncidentWorker_RaidEnemy
    {
        public Raid RaidData { get; set; }
        
        protected override string GetLetterText(IncidentParms parms, List<Pawn> pawns)
        {
            var leader = pawns.FirstOrDefault(p => p.Faction.leader == p);

            if (leader != null)
            {
                var name = leader.Name as NameTriple;
                leader.Name = new NameTriple(name?.First, RaidData.Leader.Username, name?.Last);
            }

            var limit = Mathf.Min(pawns.Count, RaidData.Army.Count);

            for (var index = 0; index < limit; index++)
            {
                var pawn = pawns[index];
                
                if (pawn == leader)
                {
                    continue;
                }
                
                Viewer viewer;

                try
                {
                    viewer = RaidData.Army[index];
                }
                catch(IndexOutOfRangeException)
                {
                    break;
                }

                var pName = pawn.Name as NameTriple;
                pawn.Name = new NameTriple(pName?.First, viewer.Username, pName?.Last);
            }

            return base.GetLetterText(parms, pawns);
        }
    }
}
