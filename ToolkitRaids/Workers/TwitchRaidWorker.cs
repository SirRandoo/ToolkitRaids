using System;
using System.Collections.Generic;
using System.Linq;
using NVorbis.Ogg;
using RimWorld;
using UnityEngine;
using Verse;
using Random = System.Random;

namespace SirRandoo.ToolkitRaids.Workers
{
    public class TwitchRaidWorker : IncidentWorker_RaidEnemy
    {
        protected override string GetLetterText(IncidentParms parms, List<Pawn> pawns)
        {
            if (parms.faction == Faction.OfMechanoids || parms.faction == Faction.OfInsects)
            {
                RaidLogger.Warn("Generated a raid with a non-human faction!");
                return base.GetLetterText(parms, pawns);
            }

            if (!(parms is TwitchRaidParms tParms))
            {
                RaidLogger.Warn("Generated a raid without Twitch parms!");
                return base.GetLetterText(parms, pawns);
            }

            if (pawns.Count < tParms.TwitchRaid.TotalTroops)
            {
                var faction = pawns.FirstOrDefault()?.Faction;
                var pawnKind = pawns.FirstOrDefault()?.kindDef;
                var missingPawns = new List<Pawn>();

                try
                {
                    missingPawns = GenerateMissingPawns(pawnKind, faction, Mathf.Abs(pawns.Count - tParms.TwitchRaid.TotalTroops));
                }
                catch (Exception e)
                {
                    RaidLogger.Error("Could not generate enough missing pawns!", e);
                }

                if (missingPawns.Count > 0)
                {
                    pawns.AddRange(missingPawns);
                    pawns.Shuffle();
                    tParms.raidArrivalMode.Worker.Arrive(missingPawns, parms);
                }
            }

            var leader = pawns.FirstOrDefault(p => p.Faction.leader == p);
            var army = tParms.TwitchRaid.Army.ToList();
            var armyComplete = pawns.Count >= tParms.TwitchRaid.TotalTroops;

            if (leader != null)
            {
                var name = leader.Name as NameTriple;
                leader.Name = new NameTriple(name?.First, tParms.TwitchRaid.Leader, name?.Last);
            }
            else
            {
                army.Insert(0, tParms.TwitchRaid.Leader);
            }

            var limit = Mathf.Min(pawns.Count, army.Count);

            RenamePawns(pawns, limit, leader, army);

            var text = "ToolkitRaids.Letters.Text".Translate(army.Count.ToString("N0"), tParms.TwitchRaid.Leader);

            if (armyComplete)
            {
                return text;
            }

            text += "\n\n";
            text += "ToolkitRaids.Letters.FailedSubtext".Translate(Mathf.Abs(pawns.Count - army.Count));

            return text;
        }

        private static void RenamePawns(IReadOnlyList<Pawn> pawns, int limit, Pawn leader, IReadOnlyList<string> army)
        {
            for (var index = 0; index < limit; index++)
            {
                var pawn = pawns[index];

                if (pawn == leader)
                {
                    continue;
                }

                string viewer;

                try
                {
                    viewer = army[index];
                }
                catch (IndexOutOfRangeException)
                {
                    break;
                }

                var pName = pawn.Name as NameTriple;
                pawn.Name = new NameTriple(pName?.First, viewer, pName?.Last);
            }
        }

        private static List<Pawn> GenerateMissingPawns(PawnKindDef kind, Faction faction, int count)
        {
            kind ??= PawnKindDefOf.Colonist;
            faction ??= Find.FactionManager.RandomEnemyFaction(allowNonHumanlike: false);

            var container = new List<Pawn>();

            for (var index = 0; index < count; ++index)
            {
                var pawn = PawnGenerator.GeneratePawn(
                    new PawnGenerationRequest(
                        kind,
                        faction,
                        allowDead: false,
                        allowDowned: false,
                        mustBeCapableOfViolence: true,
                        biocodeWeaponChance: 1f
                    ) {BiocodeApparelChance = 1f}
                );

                if (pawn == null)
                {
                    continue;
                }

                container.Add(pawn);
            }

            return container;
        }
    }
}
