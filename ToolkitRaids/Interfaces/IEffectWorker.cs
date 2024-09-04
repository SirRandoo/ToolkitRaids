using Verse;

namespace SirRandoo.ToolkitRaids.Interfaces;

public interface IEffectWorker
{
    public float Chance { get; }
    public void Apply(Pawn pawn);
}
