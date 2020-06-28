using System;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitRaids
{
    public class Settings : ModSettings
    {
        public static bool MergeRaids;
        public static int Duration = 60;

        public static void Draw(Rect canvas)
        {
            var listing = new Listing_Standard {maxOneColumn = true};

            listing.Begin(canvas);

            listing.CheckboxLabeled(
                "ToolkitRaids.MergeRaids.Label".TranslateSimple(),
                ref MergeRaids,
                "ToolkitRaids.MergeRaids.Tooltip".TranslateSimple()
            );

            var (timeLabel, timeField) = ToForm(listing.GetRect(Text.LineHeight));
            var durationBuffer = Duration.ToString();

            Widgets.Label(timeLabel, "ToolkitRaids.Duration.Label".TranslateSimple());
            Widgets.TextFieldNumeric(timeField, ref Duration, ref durationBuffer, 30, 600);

            if (Mouse.IsOver(timeLabel))
            {
                Widgets.DrawHighlightIfMouseover(timeLabel);
                TooltipHandler.TipRegion(timeLabel, "ToolkitRaids.Duration.Tooltip".TranslateSimple());
            }

            listing.End();
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref MergeRaids, "mergeRaids");
            Scribe_Values.Look(ref Duration, "duration", 60);
        }

        public static Tuple<Rect, Rect> ToForm(Rect region)
        {
            var left = new Rect(region.x, region.y, region.width * 0.85f - 2f, region.height);

            return new Tuple<Rect, Rect>(
                left,
                new Rect(left.x + left.width + 2f, left.y, region.width - left.width - 2f, left.height)
            );
        }
    }
}
