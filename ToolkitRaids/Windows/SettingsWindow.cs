// MIT License
//
// Copyright (c) 2024 SirRandoo
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using RimWorld;
using SirRandoo.ToolkitRaids.UX;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitRaids.Windows;

internal sealed class SettingsWindow : ProxySettingsWindow
{
    private string _durationBuffer = RaidMod.Instance.Settings.Duration.ToString("N0");
    private bool _durationBufferValid = true;
    private string _durationDescription = null!;
    private string _durationLabel = null!;
    private string _maximumAllowedPointsBuffer = RaidMod.Instance.Settings.MaximumAllowedPoints.ToString("N0");
    private bool _maximumAllowedPointsBufferValid = true;
    private string _maxPointsDescription = null!;
    private string _maxPointsLabel = null!;
    private string _mergeRaidsDescription = null!;
    private string _mergeRaidsLabel = null!;
    private string _messageDescription = null!;
    private string _messageLabel = null!;
    private string _minimumDescription = null!;
    private string _minimumLabel = null!;
    private string _minimumRaidersBuffer = RaidMod.Instance.Settings.MinimumRaiders.ToString("N0");
    private bool _minimumRaidersBufferValid = true;
    private string _personPointsDescription = null!;
    private string _personPointsLabel = null!;
    private string _pointsPerPersonBuffer = RaidMod.Instance.Settings.PointsPerPerson.ToString("N2");
    private bool _pointsPerPersonBufferValid = true;
    private string _raidMessage = RaidMod.Instance.Settings.MessageToSend;

    private bool _shouldMergeRaids = RaidMod.Instance.Settings.MergeRaids;
    private bool _shouldSendRaidMessage = RaidMod.Instance.Settings.SendMessage;
    private bool _shouldUseStoryteller = RaidMod.Instance.Settings.UseStoryteller;
    private string _storytellerRaidDescription = null!;
    private string _storytellerRaidLabel = null!;

    /// <inheritdoc />
    internal SettingsWindow(Mod mod) : base(mod)
    {
        doCloseX = true;
        doCloseButton = false;
    }

    protected override void GetTranslations()
    {
        _mergeRaidsLabel = "ToolkitRaids.MergeRaids.Label".TranslateSimple();
        _mergeRaidsDescription = "ToolkitRaids.MergeRaids.Description".TranslateSimple();

        _storytellerRaidLabel = "ToolkitRaids.StorytellerRaid.Label".TranslateSimple();
        _storytellerRaidDescription = "ToolkitRaids.StorytellerRaid.Description".TranslateSimple();

        _messageLabel = "ToolkitRaids.RaidMessage.Label".TranslateSimple();
        _messageDescription = "ToolkitRaids.RaidMessage.Description".TranslateSimple();

        _durationLabel = "ToolkitRaids.Duration.Label".TranslateSimple();
        _durationDescription = "ToolkitRaids.Duration.Description".TranslateSimple();

        _minimumLabel = "ToolkitRaids.MinimumRaiders.Label".TranslateSimple();
        _minimumDescription = "ToolkitRaids.MinimumRaiders.Description".TranslateSimple();

        _personPointsLabel = "ToolkitRaids.PersonPoints.Label".TranslateSimple();
        _personPointsDescription = "ToolkitRaids.PersonPoints.Description".TranslateSimple();

        _maxPointsLabel = "ToolkitRaids.MaxPoints.Label".TranslateSimple();
        _maxPointsDescription = "ToolkitRaids.MaxPoints.Description".TranslateSimple();
    }

    /// <inheritdoc />
    protected override void DrawSettings(Rect region)
    {
        var listing = new Listing_Standard(GameFont.Small) { maxOneColumn = true };

        listing.Begin(region.ContractedBy(16f));

        DrawSettingsInternal(listing);

        listing.End();
    }

    private void DrawSettingsInternal(Listing listing)
    {
        if (DrawBoolSetting(listing, _mergeRaidsLabel, _mergeRaidsDescription, ref _shouldMergeRaids))
        {
            RaidMod.Instance.Settings.MergeRaids = _shouldMergeRaids;
        }

        if (DrawBoolSetting(listing, _storytellerRaidLabel, _storytellerRaidDescription, ref _shouldUseStoryteller))
        {
            RaidMod.Instance.Settings.UseStoryteller = _shouldUseStoryteller;
        }

        if (DrawToggleableTextFieldSetting(listing, _messageLabel, _messageDescription, ref _shouldSendRaidMessage, ref _raidMessage))
        {
            RaidMod.Instance.Settings.MessageToSend = _raidMessage;
            RaidMod.Instance.Settings.SendMessage = _shouldSendRaidMessage;
        }

        (Rect durationLabelRegion, Rect durationFieldRegion) = listing.GetRect(UiConstants.LineHeight).Split();

        LabelDrawer.Draw(durationLabelRegion, _durationLabel);
        DrawSettingDescription(listing, _durationDescription);

        if (FieldDrawer.DrawNumberField(durationFieldRegion, out int duration, ref _durationBuffer, ref _durationBufferValid, 30, 600))
        {
            RaidMod.Instance.Settings.Duration = duration;
        }

        (Rect minimumRaidersLabelRegion, Rect minimumRaidersFieldRegion) = listing.GetRect(UiConstants.LineHeight).Split();

        LabelDrawer.Draw(minimumRaidersLabelRegion, _minimumLabel);
        DrawSettingDescription(listing, _minimumDescription);

        if (FieldDrawer.DrawNumberField(minimumRaidersFieldRegion, out int minimumRaiders, ref _minimumRaidersBuffer, ref _minimumRaidersBufferValid))
        {
            RaidMod.Instance.Settings.MinimumRaiders = minimumRaiders;
        }

        if (RaidMod.Instance.Settings.UseStoryteller)
        {
            return;
        }

        listing.Gap(Text.SmallFontHeight);

        (Rect pointsPerPersonLabelRegion, Rect pointsPerPersonFieldRegion) = listing.GetRect(UiConstants.LineHeight).Split();

        LabelDrawer.Draw(pointsPerPersonLabelRegion, _personPointsLabel);
        DrawSettingDescription(listing, _personPointsDescription);

        if (FieldDrawer.DrawNumberField(pointsPerPersonFieldRegion, out float pointsPerPerson, ref _pointsPerPersonBuffer, ref _pointsPerPersonBufferValid, 1f))
        {
            RaidMod.Instance.Settings.PointsPerPerson = pointsPerPerson;
        }

        (Rect maxPointsLabelRegion, Rect maxPointsFieldRegion) = listing.GetRect(UiConstants.LineHeight).Split();

        LabelDrawer.Draw(maxPointsLabelRegion, _maxPointsLabel);
        DrawSettingDescription(listing, _maxPointsDescription);

        if (FieldDrawer.DrawNumberField(maxPointsFieldRegion, out float maxPointsPerPerson, ref _maximumAllowedPointsBuffer, ref _maximumAllowedPointsBufferValid))
        {
            RaidMod.Instance.Settings.MaximumAllowedPoints = maxPointsPerPerson;
        }
    }

    private static bool DrawBoolSetting(Listing listing, string label, string description, ref bool state)
    {
        Rect settingRegion = listing.GetRect(UiConstants.LineHeight);

        Rect checkboxRegion = LayoutHelper.IconRect(
            settingRegion.x + settingRegion.width - settingRegion.height,
            settingRegion.y,
            settingRegion.height,
            settingRegion.height
        );

        settingRegion = settingRegion.Trim(Direction8Way.East, checkboxRegion.width);

        LabelDrawer.Draw(settingRegion, label);
        DrawSettingDescription(listing, description);

        return CheckboxDrawer.DrawCheckbox(checkboxRegion, ref state);
    }

    private static bool DrawToggleableTextFieldSetting(Listing listing, string label, string description, ref bool state, ref string content)
    {
        (Rect labelRegion, Rect fieldRegion) = listing.GetRect(UiConstants.LineHeight).Split();
        Rect checkboxRegion = LayoutHelper.IconRect(fieldRegion.x + fieldRegion.width - fieldRegion.height, fieldRegion.y, fieldRegion.height, fieldRegion.height);
        Rect textFieldRegion = fieldRegion.Trim(Direction8Way.West, fieldRegion.height);

        if (state)
        {
            checkboxRegion = LayoutHelper.IconRect(fieldRegion.x, fieldRegion.y, fieldRegion.height, fieldRegion.height);
        }
        else
        {
            textFieldRegion = Rect.zero;
        }

        LabelDrawer.Draw(labelRegion, label);
        DrawSettingDescription(listing, description);

        bool changed = CheckboxDrawer.DrawCheckbox(checkboxRegion, ref state);

        if (!state)
        {
            return changed;
        }

        if (!FieldDrawer.DrawTextField(textFieldRegion, content, out string? newContent))
        {
            return changed;
        }

        content = newContent;
        changed = true;

        return changed;
    }

    private static void DrawSettingDescription(Listing listing, string description)
    {
        Vector2 descriptionSize = DescriptionDrawer.GetTextBlockSize(description, listing.ColumnWidth, 0.8f);
        var descriptionRegion = new Rect(0f, listing.CurHeight, descriptionSize.x, descriptionSize.y);
        listing.Gap(descriptionRegion.height);

        DescriptionDrawer.DrawDescription(descriptionRegion, description);
    }
}
