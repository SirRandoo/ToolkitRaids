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

using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitRaids.UX;

/// <summary>
///     A utility class for rendering descriptions for UI elements, like settings.
/// </summary>
public static class DescriptionDrawer
{
    public static readonly Color DescriptionTextColor = new(0.72f, 0.72f, 0.72f);
    public static readonly Color ExperimentalTextColor = new(1f, 0.53f, 0.76f);

    public static string ExperimentalNoticeText => "This content is experimental, and may be removed or changed at any time.".TranslateSimple();

    public static void DrawTextBlock(Rect region, string content, Color color)
    {
        LabelDrawer.Draw(region, content, color, TextAnchor.UpperLeft, GameFont.Tiny);
    }

    public static void DrawExperimentalNotice(Rect region, string? content = null)
    {
        if (string.IsNullOrEmpty(content))
        {
            content = ExperimentalNoticeText;
        }

        DrawTextBlock(region, content!, ExperimentalTextColor);
    }

    public static void DrawDescription(Rect region, string content)
    {
        DrawTextBlock(region, content, DescriptionTextColor);
    }

    public static float GetLineHeight(float width, string content)
    {
        GameFont previous = Text.Font;

        Text.Font = GameFont.Tiny;
        float height = Text.CalcHeight(content, width);
        Text.Font = previous;

        return height;
    }

    public static float GetExperimentalNoticeLineHeight(float width)
    {
        GameFont previous = Text.Font;

        Text.Font = GameFont.Tiny;
        float height = Text.CalcHeight(ExperimentalNoticeText, width);
        Text.Font = previous;

        return height;
    }

    public static Vector2 GetTextBlockSize(string content, float width, float splitPercent)
    {
        float finalWidth = width * splitPercent;

        return new Vector2(finalWidth, GetLineHeight(finalWidth, content));
    }
}
