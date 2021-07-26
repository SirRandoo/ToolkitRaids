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
        public static bool UseStoryteller = true;
        public static int Duration = 60;
        public static bool SendMessage;
        public static string MessageToSend = "!so %raider%";
        public static int MinimumRaiders = 2;
        public const float AddictionlessLuciferium = 0.05f;

        public static void Draw(Rect canvas)
        {
            var listing = new Listing_Standard(GameFont.Small) {maxOneColumn = true};

            listing.Begin(canvas.ContractedBy(16f));

            listing.CheckboxLabeled("ToolkitRaids.MergeRaids.Label".TranslateSimple(), ref MergeRaids);
            listing.DrawDescription("ToolkitRaids.MergeRaids.Description".TranslateSimple());
            listing.CheckboxLabeled("ToolkitRaids.StorytellerRaid.Label".TranslateSimple(), ref UseStoryteller);
            listing.DrawDescription("ToolkitRaids.StorytellerRaid.Description".TranslateSimple());

            (Rect sendMsgLabel, Rect sendMsgField) = listing.GetRectAsForm();
            var sendMsgCheck = new Rect(
                sendMsgField.x + sendMsgField.width - sendMsgField.height,
                sendMsgField.y,
                sendMsgField.height,
                sendMsgField.height
            );

            if (SendMessage)
            {
                sendMsgLabel = sendMsgLabel.WithWidth(sendMsgLabel.width - sendMsgLabel.height);
                sendMsgCheck = new Rect(
                    sendMsgLabel.x + sendMsgLabel.width,
                    sendMsgLabel.y,
                    sendMsgLabel.height,
                    sendMsgLabel.height
                );
            }

            Widgets.Checkbox(sendMsgCheck.x, sendMsgCheck.y, ref SendMessage, sendMsgLabel.height);
            SettingsHelper.DrawLabel(sendMsgLabel, "ToolkitRaids.RaidMessage.Label".TranslateSimple());
            listing.DrawDescription("ToolkitRaids.RaidMessage.Description".TranslateSimple());

            if (SendMessage)
            {
                MessageToSend = Widgets.TextField(sendMsgField, MessageToSend);
            }

            (Rect timeLabel, Rect timeField) = listing.GetRectAsForm();
            var durationBuffer = Duration.ToString();

            SettingsHelper.DrawLabel(timeLabel, "ToolkitRaids.Duration.Label".TranslateSimple());
            Widgets.TextFieldNumeric(timeField, ref Duration, ref durationBuffer, 30, 600);
            listing.DrawDescription("ToolkitRaids.Duration.Description".TranslateSimple());

            (Rect minLabel, Rect minField) = listing.GetRectAsForm();
            var minBuffer = MinimumRaiders.ToString();

            SettingsHelper.DrawLabel(minLabel, "ToolkitRaids.MinimumRaiders.Label".TranslateSimple());
            Widgets.TextFieldNumeric(minField, ref MinimumRaiders, ref minBuffer);
            listing.DrawDescription("ToolkitRaids.MinimumRaiders.Description".TranslateSimple());

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
            listing.DrawDescription("ToolkitRaids.PersonPoints.Description".TranslateSimple());

            (Rect maxLabel, Rect maxField) = listing.GetRectAsForm();
            var maxBuffer = MaximumAllowedPoints.ToString(CultureInfo.InvariantCulture);

            Widgets.Label(maxLabel, "ToolkitRaids.MaxPoints.Label".TranslateSimple());
            Widgets.TextFieldNumeric(maxField, ref MaximumAllowedPoints, ref maxBuffer, PointsPerPerson);
            listing.DrawDescription("ToolkitRaids.MaxPoints.Description".TranslateSimple());

            listing.End();
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref MergeRaids, "mergeRaids");
            Scribe_Values.Look(ref Duration, "duration", 60);
            Scribe_Values.Look(ref MaximumAllowedPoints, "maxPoints", 20000f);
            Scribe_Values.Look(ref UseStoryteller, "storyteller", true);
            Scribe_Values.Look(ref PointsPerPerson, "pointsPerPerson", 50f);
            Scribe_Values.Look(ref SendMessage, "sendMessage");
            Scribe_Values.Look(ref MessageToSend, "messageToSend", "!so %raider%");
            Scribe_Values.Look(ref MinimumRaiders, "minimumRaiders", 2);
        }
    }
}
