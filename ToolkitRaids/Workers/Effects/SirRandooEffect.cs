﻿using RimWorld;
using SirRandoo.ToolkitRaids.Interfaces;
using Verse;

namespace SirRandoo.ToolkitRaids.Workers.Effects
{
    public class SirRandooEffect : IEffectWorker
    {
        public float Chance
        {
            get
            {
            #if DEBUG
                return 1f;
            #else
                return 0.3333f;
            #endif
            }
        }

        public void Apply(Pawn pawn)
        {
            HediffGiverUtility.TryApply(pawn, DrugHediffs.LuciferiumHigh, null);
            
            if(!Rand.Chance(Settings.AddictionlessLuciferium))
            {
                HediffGiverUtility.TryApply(pawn, DrugHediffs.LuciferiumAddiction, null);
            }
        }
    }
}
