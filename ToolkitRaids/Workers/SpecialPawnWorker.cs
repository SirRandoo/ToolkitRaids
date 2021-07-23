using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SirRandoo.ToolkitRaids.Interfaces;
using SirRandoo.ToolkitRaids.Workers.Effects;
using Verse;

namespace SirRandoo.ToolkitRaids.Workers
{
    [UsedImplicitly]
    [StaticConstructorOnStartup]
    public static class SpecialPawnWorker
    {
        private static readonly Dictionary<string, IEffectWorker> SpecialEffects;

        static SpecialPawnWorker()
        {
            SpecialEffects = new Dictionary<string, IEffectWorker>
            {
                {"sirrandoo", new SirRandooEffect()}, {"hodlhodl", new DefaultEffect()}
            };

            ToolkitRaids.SpecialNames = SpecialEffects.Keys.ToList();
        }

        private static bool IsSpecialPawn([NotNull] string name)
        {
            return ToolkitRaids.SpecialNames.Contains(name.ToLowerInvariant());
        }

        public static void ApplyEffectOf([NotNull] string name, Pawn pawn)
        {
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
}
