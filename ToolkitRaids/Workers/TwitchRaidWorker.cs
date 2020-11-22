using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

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
                Faction faction = pawns.FirstOrDefault()?.Faction;
                PawnKindDef pawnKind = pawns.FirstOrDefault()?.kindDef;
                var missingPawns = new List<Pawn>();

                try
                {
                    missingPawns = GenerateMissingPawns(
                        pawnKind,
                        faction,
                        Mathf.Abs(pawns.Count - tParms.TwitchRaid.TotalTroops)
                    );
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

            Pawn leader = pawns.FirstOrDefault(p => p.Faction.leader == p);
            List<string> army = tParms.TwitchRaid.Army.ToList();
            bool armyComplete = pawns.Count >= tParms.TwitchRaid.TotalTroops;

            if (leader != null)
            {
                var name = leader.Name as NameTriple;
                leader.Name = new NameTriple(name?.First, tParms.TwitchRaid.Leader, name?.Last);
                SpecialPawnWorker.ApplyEffectOf(tParms.TwitchRaid.Leader, leader);
            }
            else
            {
                army.Insert(0, tParms.TwitchRaid.Leader);
            }

            int limit = Mathf.Min(pawns.Count, army.Count);

            RenamePawns(pawns, limit, leader, army);

            string text = "ToolkitRaids.Letters.Text".Translate(tParms.TwitchRaid.ArmyCountLabel, tParms.TwitchRaid.Leader);
            string strategy = GetKeyForStrategy(tParms.raidStrategy);

            if (!strategy.NullOrEmpty())
            {
                text += " " + strategy.TranslateSimple();
            }

            if (armyComplete)
            {
                return text;
            }

            text += "\n\n";
            text += "ToolkitRaids.Letters.FailedSubtext".Translate(Mathf.Abs(pawns.Count - army.Count));

            return text;
        }

        private static string GetKeyForStrategy(Def strategy)
        {
            switch (strategy.defName)
            {
                case "ImmediateAttack":
                    return "ToolkitRaids.Letters.Strategy.Immediate";
                case "ImmediateAttackSmart":
                    return "ToolkitRaids.Letters.Strategy.ImmediateSmart";
                case "StageThenAttack":
                    return "ToolkitRaids.Letters.Strategy.Wait";
                case "ImmediateAttackSappers":
                    return "ToolkitRaids.Letters.Strategy.Sapper";
                case "Siege":
                case "SiegeMechanoid":
                    return "ToolkitRaids.Letters.Strategy.Siege";
                default:
                    return "ToolkitRaids.Letters.Strategy.Unknown";
            }
        }

        private static void RenamePawns(IReadOnlyList<Pawn> pawns, int limit, Pawn leader, IReadOnlyList<string> army)
        {
            for (var index = 0; index < limit; index++)
            {
                Pawn pawn = pawns[index];

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
                SpecialPawnWorker.ApplyEffectOf(viewer, pawn);
            }
        }

        private static List<Pawn> GenerateMissingPawns(PawnKindDef kind, Faction faction, int count)
        {
            kind ??= PawnKindDefOf.Colonist;
            faction ??= Find.FactionManager.RandomEnemyFaction(allowNonHumanlike: false);

            var container = new List<Pawn>();

            for (var index = 0; index < count; ++index)
            {
                Pawn pawn = PawnGenerator.GeneratePawn(
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
