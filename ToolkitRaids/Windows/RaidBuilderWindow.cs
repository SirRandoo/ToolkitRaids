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
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using SirRandoo.ToolkitRaids.Models;
using SirRandoo.ToolkitRaids.UX;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitRaids.Windows;

internal sealed class RaidBuilderWindow : Window
{
    private string _raidLeader = RaidMod.GenerateNameForRaid();
    private readonly List<string> _raiders = [];
    private Vector2 _scrollPosition = Vector2.zero;

    /// <inheritdoc />
    public override Vector2 InitialSize => new Vector2(250, 300);

    /// <inheritdoc />
    public override void DoWindowContents(Rect inRect)
    {
        var listing = new Listing_Standard();
        var leaderRegion = new Rect(0f, 0f, inRect.width, Text.SmallFontHeight);
        var generateRegion = new Rect(0f, inRect.height - Text.SmallFontHeight, inRect.width, Text.SmallFontHeight);
        var raidersRegion = new Rect(0f, leaderRegion.height + Text.SmallFontHeight * 2, inRect.width, inRect.height - leaderRegion.height - Text.SmallFontHeight * 3);
        var raidersListRegion = new Rect(0f, Text.SmallFontHeight, inRect.width, raidersRegion.height - Text.SmallFontHeight * 2);
        var raidersScrollView = new Rect(0f, 0f, raidersListRegion.width - 16f, Text.SmallFontHeight * _raiders.Count);

        listing.Begin(leaderRegion);

        (Rect leaderLabelRegion, Rect leaderFieldRegion) = listing.Split(0.5f);
        LabelDrawer.Draw(leaderLabelRegion, "ToolkitRaids.Windows.Builder.RaidLeader".TranslateSimple());

        if (FieldDrawer.DrawTextField(leaderFieldRegion, _raidLeader, out string? newLeader))
        {
            _raidLeader = newLeader;
        }

        listing.End();

        listing.Begin(raidersRegion);

        (Rect raidersLabelRegion, Rect raidersAddBtnRegion) = listing.Split(0.5f);

        LabelDrawer.Draw(raidersLabelRegion, "ToolkitRaids.Windows.Builder.Raiders".TranslateSimple());

        if (Widgets.ButtonText(raidersAddBtnRegion, "ToolkitRaids.Windows.Builder.AddRaider".TranslateSimple()))
        {
            var dialog = new NameDialog();
            dialog.Submitted += (_, s) => _raiders.Add(s);

            Find.WindowStack.Add(dialog);
        }

        listing.Begin(raidersListRegion);
        var listRegionZeroed = new Rect(0f, 0f, raidersListRegion.width, raidersListRegion.height);

        _scrollPosition = GUI.BeginScrollView(listRegionZeroed, _scrollPosition, raidersScrollView);

        int removalIndex = -1;

        for (var i = 0; i < _raiders.Count; i++)
        {
            string raider = _raiders[i];
            Rect lineRegion = listing.GetRect(Text.SmallFontHeight);
            lineRegion.width = raidersListRegion.width - 16f;

            if (!lineRegion.IsVisible(listRegionZeroed, _scrollPosition))
            {
                continue;
            }

            if (i % 2 == 0)
            {
                Widgets.DrawLightHighlight(lineRegion);
            }

            LabelDrawer.Draw(lineRegion, raider);

            Rect removeBtnRegion = LayoutHelper.IconRect(lineRegion.x + lineRegion.width - lineRegion.height, lineRegion.y, lineRegion.height, lineRegion.height);

            if (Widgets.ButtonImage(removeBtnRegion, Widgets.CheckboxOffTex))
            {
                removalIndex = i;
            }
        }

        GUI.EndScrollView();
        listing.End();

        listing.End();

        if (removalIndex != -1)
        {
            _raiders.RemoveAt(removalIndex);
        }

        if (Widgets.ButtonText(generateRegion, "ToolkitRaids.Windows.Builder.Generate".TranslateSimple()))
        {
            Close();
        }
    }

    /// <inheritdoc />
    public override void PostClose()
    {
        var component = Current.Game.GetComponent<GameComponentTwitchRaid>();

        if (component == null)
        {
            RaidLogger.Warn("Could not find component Twitch raid game component; discarding raid...");

            return;
        }

        component.RegisterRaid(new Raid { Leader = _raidLeader, Army = _raiders, Timer = RaidMod.Instance.Settings.Duration });
    }

    [PublicAPI]
    private sealed class NameDialog : Window
    {
        private string _name = string.Empty;

        public event EventHandler<string> Submitted = null!;
        public event EventHandler Cancelled = null!;

        /// <inheritdoc />
        public override Vector2 InitialSize => new Vector2(200, 80);

        /// <inheritdoc />
        public override void DoWindowContents(Rect inRect)
        {
            var fieldRegion = new Rect(0f, 0f, inRect.width, Text.SmallFontHeight);
            var cancelRegion = new Rect(0f, inRect.height - Text.SmallFontHeight, fieldRegion.width * 0.5f, Text.SmallFontHeight);
            var submitRegion = new Rect(cancelRegion.width, cancelRegion.y, cancelRegion.width, Text.SmallFontHeight);

            GUI.BeginGroup(inRect);

            var controlName = $"TextField_{fieldRegion.x}{fieldRegion.y}";
            GUI.SetNextControlName(controlName);

            if (FieldDrawer.DrawTextField(fieldRegion, _name, out string? newName))
            {
                _name = newName;
            }

            GUI.FocusControl(controlName);

            if (Widgets.ButtonText(submitRegion, "Submit"))
            {
                OnSubmitted(_name);

                Close(false);
            }

            if (Widgets.ButtonText(cancelRegion, "Cancel"))
            {
                OnCancelled();

                Close(false);
            }

            GUI.EndGroup();
        }

        private void OnSubmitted(string e)
        {
            Submitted?.Invoke(this, e);

            Close();
        }

        private void OnCancelled()
        {
            Cancelled?.Invoke(this, EventArgs.Empty);

            Close();
        }

        /// <inheritdoc />
        public override void OnAcceptKeyPressed()
        {
            OnSubmitted(_name);
        }

        /// <inheritdoc />
        public override void OnCancelKeyPressed()
        {
            OnCancelled();
        }
    }
}
