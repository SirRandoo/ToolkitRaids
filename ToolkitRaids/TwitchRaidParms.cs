using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitRaids.Models;
using Verse;

namespace SirRandoo.ToolkitRaids
{
    public class TwitchRaidParms : IncidentParms
    {
        public TwitchRaidParms([NotNull] IncidentParms parms)
        {
            target = parms.target;
            points = parms.points;
            faction = parms.faction;
            forced = parms.forced;
            customLetterLabel = parms.customLetterLabel;
            customLetterText = parms.customLetterText;
            customLetterDef = parms.customLetterDef;
            letterHyperlinkThingDefs = parms.letterHyperlinkThingDefs;
            inSignalEnd = parms.inSignalEnd;
            spawnCenter = parms.spawnCenter;
            spawnRotation = parms.spawnRotation;
            generateFightersOnly = parms.generateFightersOnly;
            dontUseSingleUseRocketLaunchers = parms.dontUseSingleUseRocketLaunchers;
            raidStrategy = parms.raidStrategy;
            raidArrivalMode = parms.raidArrivalMode;
            raidForceOneIncap = parms.raidForceOneIncap;
            raidNeverFleeIndividual = parms.raidNeverFleeIndividual;
            raidArrivalModeForQuickMilitaryAid = parms.raidArrivalModeForQuickMilitaryAid;
            biocodeApparelChance = parms.biocodeApparelChance;
            biocodeWeaponsChance = parms.biocodeWeaponsChance;
            pawnGroups = parms.pawnGroups;
            pawnGroupMakerSeed = parms.pawnGroupMakerSeed;
            pawnKind = parms.pawnKind;
            pawnCount = parms.pawnCount;
            traderKind = parms.traderKind;
            podOpenDelay = parms.podOpenDelay;
            quest = parms.quest;
            questScriptDef = parms.questScriptDef;
            questTag = parms.questTag;
            mechClusterSketch = parms.mechClusterSketch;
        }

        public Raid TwitchRaid { get; set; }

        [NotNull]
        public static TwitchRaidParms ForRaid(Raid raid, Map map)
        {
            return new TwitchRaidParms(StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, map))
            {
                TwitchRaid = raid
            };
        }
    }
}
