using System;
using JetBrains.Annotations;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitRaids.Helpers
{
    internal static class SettingsHelper
    {
        [NotNull]
        public static Tuple<Rect, Rect> ToForm(this Rect region, float factor = 0.8f)
        {
            var left = new Rect(region.x, region.y, region.width * factor - 2f, region.height);

            return new Tuple<Rect, Rect>(
                left.Rounded(),
                new Rect(left.x + left.width + 2f, left.y, region.width - left.width - 2f, left.height).Rounded()
            );
        }

        public static Rect TrimLeft(this Rect region, float amount)
        {
            return new Rect(region.x + amount, region.y, region.width - amount, region.height);
        }

        public static Rect WithWidth(this Rect region, float width)
        {
            return new Rect(region.x, region.y, width, region.height);
        }

        [NotNull]
        public static Tuple<Rect, Rect> GetRectAsForm([NotNull] this Listing listing, float factor = 0.8f)
        {
            return listing.GetRect(Text.LineHeight).ToForm(factor);
        }

        public static void DrawDescription([NotNull] this Listing listing, string description, Color color)
        {
            GameFont fontCache = Text.Font;
            GUI.color = color;
            Text.Font = GameFont.Tiny;
            float height = Text.CalcHeight(description, listing.ColumnWidth * 0.7f);
            DrawLabel(
                listing.GetRect(height).TrimLeft(10f).WithWidth(listing.ColumnWidth * 0.7f).Rounded(),
                description,
                TextAnchor.UpperLeft,
                GameFont.Tiny
            );
            GUI.color = Color.white;
            Text.Font = fontCache;

            listing.Gap(6f);
        }

        public static void DrawDescription([NotNull] this Listing listing, string description)
        {
            DrawDescription(listing, description, new Color(0.72f, 0.72f, 0.72f));
        }

        public static void DrawLabel(
            Rect region,
            string text,
            TextAnchor anchor = TextAnchor.MiddleLeft,
            GameFont fontScale = GameFont.Small,
            bool vertical = false
        )
        {
            Text.Anchor = anchor;
            Text.Font = fontScale;

            if (vertical)
            {
                region.y += region.width;
                GUIUtility.RotateAroundPivot(-90f, region.position);
            }

            Widgets.Label(region, text);

            if (vertical)
            {
                GUI.matrix = Matrix4x4.identity;
            }

            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;
        }
    }
}
