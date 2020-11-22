using System.Collections.Generic;
using RimWorld;
using SirRandoo.ToolkitRaids.Interfaces;
using Verse;

namespace SirRandoo.ToolkitRaids.Workers.Effects
{
    public class DefaultEffect : IEffectWorker
    {
        private static readonly List<HediffDef> Effects = new List<HediffDef>
        {
            DrugHediffs.FlakeHigh,
            DrugHediffs.GoJuiceHigh,
            DrugHediffs.LuciferiumHigh,
            DrugHediffs.WakeUpHigh,
            DrugHediffs.YayoHigh
        };

        public float Chance
        {
            get
            {
            #if DEBUG
                return 1f;
            #else
                return 0.1f;
            #endif
            }
        }

        public void Apply(Pawn pawn)
        {
            HediffDef drugHediff = Effects.RandomElement();
            HediffGiverUtility.TryApply(pawn, drugHediff, null);

            if (drugHediff == DrugHediffs.LuciferiumHigh && !Rand.Chance(Settings.AddictionlessLuciferium))
            {
                HediffGiverUtility.TryApply(pawn, DrugHediffs.LuciferiumAddiction, null);
            }
        }
    }
}
