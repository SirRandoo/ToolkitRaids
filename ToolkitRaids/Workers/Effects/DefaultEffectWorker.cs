using SirRandoo.ToolkitRaids.Interfaces;
using Verse;

namespace SirRandoo.ToolkitRaids.Workers.Effects;

internal sealed class DefaultEffectWorker : IEffectWorker
{
    public const float AddictionlessLuciferium = 0.05f;

    private static readonly HediffDef[] Effects =
        [DrugHediffs.FlakeHigh, DrugHediffs.GoJuiceHigh, DrugHediffs.LuciferiumHigh, DrugHediffs.WakeUpHigh, DrugHediffs.YayoHigh];

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

        if (drugHediff == DrugHediffs.LuciferiumHigh && !Rand.Chance(AddictionlessLuciferium))
        {
            HediffGiverUtility.TryApply(pawn, DrugHediffs.LuciferiumAddiction, null);
        }
    }
}
