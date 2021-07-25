using SirRandoo.ToolkitRaids.Models;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitRaids
{
    [StaticConstructorOnStartup]
    public class RaidDialog : Window
    {
        private static readonly Gradient TimerGradient;

        private GameComponentTwitchRaid _component;
        private string _footerText;
        private string _leaderText;
        private Vector2 _scrollPos = Vector2.zero;

        private string _titleText;
        private string _troopText;

        static RaidDialog()
        {
            TimerGradient = new Gradient();

            var colorKey = new GradientColorKey[2];
            colorKey[0].color = Color.green;
            colorKey[0].time = 0.0f;
            colorKey[1].color = Color.red;
            colorKey[1].time = 1.0f;

            var alphaKey = new GradientAlphaKey[2];
            alphaKey[0].alpha = 1.0f;
            alphaKey[0].time = 0.0f;
            alphaKey[1].alpha = 1.0f;
            alphaKey[1].time = 1.0f;

            TimerGradient.SetKeys(colorKey, alphaKey);
        }

        public RaidDialog()
        {
            GetTranslations();

            optionalTitle = _titleText;

            doCloseX = true;
            draggable = true;
            closeOnCancel = false;
            closeOnAccept = false;
            focusWhenOpened = false;
            closeOnClickedOutside = false;
            absorbInputAroundWindow = false;
            preventCameraMotion = false;
        }

        public override Vector2 InitialSize => new Vector2(300, 300);

        private void GetTranslations()
        {
            _titleText = "ToolkitRaids.Windows.Raid.Title".TranslateSimple();
            _troopText = "ToolkitRaids.Windows.Raid.Troops".TranslateSimple();
            _leaderText = "ToolkitRaids.Windows.Raid.Leader".TranslateSimple();
            _footerText = "ToolkitRaids.Windows.Raid.Footer".TranslateSimple();
        }

        public override void PreOpen()
        {
            _component = Current.Game.GetComponent<GameComponentTwitchRaid>();

            if (_component != null)
            {
                base.PreOpen();
                return;
            }

            RaidLogger.Warn("Twitch raid component is null!");
            Close();
        }

        public override void DoWindowContents(Rect inRect)
        {
            GUI.BeginGroup(inRect);
            GameFont cache = Text.Font;
            var listing = new Listing_Standard(GameFont.Small);

            var contentRect = new Rect(0f, 0f, inRect.width, inRect.height - Text.SmallFontHeight);
            var viewPort = new Rect(
                0f,
                0f,
                inRect.width - 16f,
                _component.AllRaidsForReading.Count * (Text.SmallFontHeight * 3f)
            );

            Widgets.BeginScrollView(contentRect, ref _scrollPos, viewPort);
            listing.Begin(viewPort);

            for (var index = 0; index < _component.AllRaidsForReading.Count; index++)
            {
                Raid raid = _component.AllRaidsForReading[index];
                Rect line = listing.GetRect(Text.LineHeight * 3f);
                var leaderRect = new Rect(0f, line.y, line.width, Text.LineHeight);
                var armyRect = new Rect(0f, line.y + Text.LineHeight, line.width, Text.LineHeight);
                var progressRect = new Rect(0f, line.y + Text.LineHeight * 2f, line.width, Text.LineHeight);
                float progress = raid.Timer / Settings.Duration;

                if (index % 2 == 1)
                {
                    Widgets.DrawLightHighlight(line);
                }

                Widgets.Label(leaderRect, $"{_leaderText}: {raid.Leader}");
                Widgets.Label(armyRect, $"{_troopText}: {raid.ArmyCountLabel}");


                GUI.color = TimerGradient.Evaluate(1f - progress);
                Widgets.FillableBar(progressRect, progress, Texture2D.whiteTexture, null, true);
                GUI.color = Color.white;
            }

            listing.End();
            Widgets.EndScrollView();

            var footerRect = new Rect(0f, inRect.height - Text.LineHeight, inRect.width, Text.LineHeight);

            GUI.BeginGroup(footerRect);
            Widgets.Label(new Rect(0f, 0f, inRect.width, Text.LineHeight), _footerText);
            GUI.EndGroup();

            GUI.EndGroup();
            Text.Font = cache;
        }

        public override void WindowUpdate()
        {
            if (Time.time % 2 < 1 && !_component.AllRaidsForReading.Any())
            {
                Close(false);
            }
        }

        protected override void SetInitialSizeAndPosition()
        {
            windowRect = new Rect(
                UI.screenWidth - InitialSize.x,
                UI.screenHeight / 2f - InitialSize.y / 2f,
                InitialSize.x,
                InitialSize.y
            );
        }
    }
}
