using System;
using System.Globalization;
using SirRandoo.ToolkitRaids.Helpers;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitRaids
{
    public class Settings : ModSettings
    {
        public static float PointsPerPerson = 50f;
        public static float MaximumAllowedPoints = 20000f;
        public static bool MergeRaids;
        public static bool UseStoryteller;
        public static int Duration = 60;

        public static void Draw(Rect canvas)
        {
            var listing = new Listing_Standard(GameFont.Small) {maxOneColumn = true};

            listing.Begin(canvas.ContractedBy(16f));

            listing.CheckboxLabeled("ToolkitRaids.MergeRaids.Label".TranslateSimple(), ref MergeRaids);
            listing.DrawDescription("ToolkitRaids.MergeRaids.Description".TranslateSimple());
            listing.CheckboxLabeled("ToolkitRaids.StorytellerRaid.Label".TranslateSimple(), ref UseStoryteller);
            listing.DrawDescription("ToolkitRaids.StorytellerRaid.Description".TranslateSimple());
            
            (Rect timeLabel, Rect timeField) = listing.GetRectAsForm();
            var durationBuffer = Duration.ToString();

            Widgets.Label(timeLabel, "ToolkitRaids.Duration.Label".TranslateSimple());
            Widgets.TextFieldNumeric(timeField, ref Duration, ref durationBuffer, 30, 600);
            Widgets.DrawHighlightIfMouseover(timeLabel);
            listing.DrawDescription("ToolkitRaids.Duration.Description".TranslateSimple());

            if (UseStoryteller)
            {
                listing.End();
                return;
            }

            listing.Gap(8f);
            Rect line = listing.GetRect(Text.LineHeight).ContractedBy(12f);
            Widgets.DrawLineHorizontal(line.x, line.y, line.width);
            listing.Gap();

            (Rect personLabel, Rect personField) = listing.GetRectAsForm();
            var personBuffer = PointsPerPerson.ToString(CultureInfo.InvariantCulture);

            Widgets.Label(personLabel, "ToolkitRaids.PersonPoints.Label".TranslateSimple());
            Widgets.TextFieldNumeric(personField, ref PointsPerPerson, ref personBuffer, 1f);
            Widgets.DrawHighlightIfMouseover(personLabel);
            listing.DrawDescription("ToolkitRaids.PersonPoints.Description".TranslateSimple());

            (Rect maxLabel, Rect maxField) = listing.GetRectAsForm();
            var maxBuffer = MaximumAllowedPoints.ToString(CultureInfo.InvariantCulture);

            Widgets.Label(maxLabel, "ToolkitRaids.MaxPoints.Label".TranslateSimple());
            Widgets.TextFieldNumeric(maxField, ref MaximumAllowedPoints, ref maxBuffer, PointsPerPerson);
            Widgets.DrawHighlightIfMouseover(maxLabel);
            listing.DrawDescription("ToolkitRaids.MaxPoints.Description".TranslateSimple());

            listing.End();
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref MergeRaids, "mergeRaids");
            Scribe_Values.Look(ref Duration, "duration", 60);
            Scribe_Values.Look(ref MaximumAllowedPoints, "maxPoints", 20000f);
            Scribe_Values.Look(ref UseStoryteller, "storyteller");
            Scribe_Values.Look(ref PointsPerPerson, "pointsPerPerson", 50f);
        }
    }
}
