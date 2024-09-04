using RimWorld;
using SirRandoo.ToolkitRaids.Models;
using Verse;

namespace SirRandoo.ToolkitRaids;

public sealed class TwitchRaidParams : IncidentParms
{
    private TwitchRaidParams(IncidentParms @params)
    {
        target = @params.target;
        points = @params.points;
        faction = @params.faction;
        forced = @params.forced;
        customLetterLabel = @params.customLetterLabel;
        customLetterText = @params.customLetterText;
        customLetterDef = @params.customLetterDef;
        letterHyperlinkThingDefs = @params.letterHyperlinkThingDefs;
        inSignalEnd = @params.inSignalEnd;
        spawnCenter = @params.spawnCenter;
        spawnRotation = @params.spawnRotation;
        generateFightersOnly = @params.generateFightersOnly;
        dontUseSingleUseRocketLaunchers = @params.dontUseSingleUseRocketLaunchers;
        raidStrategy = @params.raidStrategy;
        raidArrivalMode = @params.raidArrivalMode;
        raidForceOneDowned = @params.raidForceOneDowned;
        raidNeverFleeIndividual = @params.raidNeverFleeIndividual;
        raidArrivalModeForQuickMilitaryAid = @params.raidArrivalModeForQuickMilitaryAid;
        biocodeApparelChance = @params.biocodeApparelChance;
        biocodeWeaponsChance = @params.biocodeWeaponsChance;
        pawnGroups = @params.pawnGroups;
        pawnGroupMakerSeed = @params.pawnGroupMakerSeed;
        pawnKind = @params.pawnKind;
        pawnCount = @params.pawnCount;
        traderKind = @params.traderKind;
        podOpenDelay = @params.podOpenDelay;
        quest = @params.quest;
        questScriptDef = @params.questScriptDef;
        questTag = @params.questTag;
        mechClusterSketch = @params.mechClusterSketch;
    }

    public required Raid TwitchRaid { get; init; }

    internal static TwitchRaidParams ForRaid(Raid raid, Map map) => new(StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, map)) { TwitchRaid = raid };
}
