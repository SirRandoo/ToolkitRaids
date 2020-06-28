using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;
using Verse;

namespace SirRandoo.ToolkitRaids
{
    public class Settings : ModSettings
    {
        public static float PointsPerPerson = 50f;
        public static float MaximumAllowedPoints = 20000f;
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

            listing.Gap(8f);
            var line = listing.GetRect(Text.LineHeight).ContractedBy(12f);
            Widgets.DrawLineHorizontal(line.x, line.y, line.width);
            listing.Gap();

            var (timeLabel, timeField) = ToForm(listing.GetRect(Text.LineHeight));
            var durationBuffer = Duration.ToString();

            Widgets.Label(timeLabel, "ToolkitRaids.Duration.Label".TranslateSimple());
            Widgets.TextFieldNumeric(timeField, ref Duration, ref durationBuffer, 30, 600);
            Widgets.DrawHighlightIfMouseover(timeLabel);
            TooltipHandler.TipRegion(timeLabel, "ToolkitRaids.Duration.Tooltip".TranslateSimple());

            var (personLabel, personField) = ToForm(listing.GetRect(Text.LineHeight));
            var personBuffer = PointsPerPerson.ToString(CultureInfo.InvariantCulture);

            Widgets.Label(personLabel, "ToolkitRaids.PersonPoints.Label".TranslateSimple());
            Widgets.TextFieldNumeric(personField, ref PointsPerPerson, ref personBuffer, 1f);
            Widgets.DrawHighlightIfMouseover(personLabel);
            TooltipHandler.TipRegion(personLabel, "ToolkitRaids.PersonPoints.Tooltip".TranslateSimple());

            var (maxLabel, maxField) = ToForm(listing.GetRect(Text.LineHeight));
            var maxBuffer = MaximumAllowedPoints.ToString(CultureInfo.InvariantCulture);

            Widgets.Label(maxLabel, "ToolkitRaids.MaxPoints.Label".TranslateSimple());
            Widgets.TextFieldNumeric(maxField, ref MaximumAllowedPoints, ref maxBuffer, PointsPerPerson);
            Widgets.DrawHighlightIfMouseover(maxLabel);
            TooltipHandler.TipRegion(maxLabel, "ToolkitRaids.MaxPoints.Tooltip".TranslateSimple());

            listing.End();
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref MergeRaids, "mergeRaids");
            Scribe_Values.Look(ref Duration, "duration", 60);
            Scribe_Values.Look(ref MaximumAllowedPoints, "maxPoints", 20000f);
            Scribe_Values.Look(ref PointsPerPerson, "pointsPerPerson", 50f);
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
