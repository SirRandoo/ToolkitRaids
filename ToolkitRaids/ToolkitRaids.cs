using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitRaids
{
    public class ToolkitRaids : Mod
    {
        public ToolkitRaids(ModContentPack content) : base(content)
        {
            GetSettings<Settings>();
        }

        public override void DoSettingsWindowContents(Rect inRect) => Settings.Draw(inRect);

        public override string SettingsCategory() => nameof(ToolkitRaids);
    }
}
