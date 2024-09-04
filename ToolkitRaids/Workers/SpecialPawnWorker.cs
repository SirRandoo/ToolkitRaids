using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using SirRandoo.ToolkitRaids.Interfaces;
using SirRandoo.ToolkitRaids.Workers.Effects;
using Verse;

namespace SirRandoo.ToolkitRaids.Workers;

[UsedImplicitly]
[StaticConstructorOnStartup]
internal static class SpecialPawnWorker
{
    private static readonly Dictionary<string, IEffectWorker> SpecialEffects;

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    static SpecialPawnWorker()
    {
        SpecialEffects = new Dictionary<string, IEffectWorker> { { "sirrandoo", new SirRandooEffectWorker() }, { "hodlhodl", new DefaultEffectWorker() } };

        RaidMod.SpecialNames = SpecialEffects.Keys.ToList();
    }

    private static bool IsSpecialPawn(string name) => RaidMod.SpecialNames.Contains(name.ToLowerInvariant());

    public static void ApplyEffectOf(string name, Pawn pawn)
    {
        if (pawn.RaceProps.IsMechanoid || pawn.RaceProps.IsAnomalyEntity)
        {
            return;
        }

        if (!IsSpecialPawn(name))
        {
            return;
        }

        if (!SpecialEffects.TryGetValue(name.ToLowerInvariant(), out IEffectWorker effectWorker))
        {
            return;
        }

        if (!Rand.Chance(effectWorker.Chance))
        {
            return;
        }

        effectWorker.Apply(pawn);
    }
}
