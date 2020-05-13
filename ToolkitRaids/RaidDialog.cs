using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitRaids
{
    [StaticConstructorOnStartup]
    public class RaidDialog : Window
    {
        private static readonly Gradient TimerGradient;

        private GameComponentTwitchRaid _component;
        private Vector2 _scrollPos = Vector2.zero;
        private TaggedString _footerText;
        private TaggedString _leaderText;

        private TaggedString _titleText;
        private TaggedString _troopText;

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
            _titleText = "ToolkitRaids.Windows.Raid.Title".Translate();
            _troopText = "ToolkitRaids.Windows.Raid.Troops".Translate();
            _leaderText = "ToolkitRaids.Windows.Raid.Leader".Translate();
            _footerText = "ToolkitRaids.Windows.Raid.Footer".Translate();
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
            var colorCache = GUI.color;
            var listing = new Listing_Standard();

            var contentRect = new Rect(0f, 0f, inRect.width, inRect.height - Text.LineHeight);
            var viewPort = new Rect(
                0f,
                0f,
                inRect.width - 16f,
                _component.AllRaidsForReading.Count * (Text.LineHeight * 3f)
            );

            GUI.BeginGroup(contentRect);
            listing.BeginScrollView(contentRect, ref _scrollPos, ref viewPort);

            for (var index = 0; index < _component.AllRaidsForReading.Count; index++)
            {
                var raid = _component.AllRaidsForReading[index];
                var line = listing.GetRect(Text.LineHeight * 3f);
                var leaderRect = new Rect(0f, line.y, line.width, Text.LineHeight);
                var armyRect = new Rect(0f, line.y + Text.LineHeight, line.width, Text.LineHeight);
                var progressRect = new Rect(0f, line.y + Text.LineHeight * 2f, line.width, Text.LineHeight);
                var progress = raid.Timer / Settings.Duration;

                if (index % 2 == 1)
                {
                    Widgets.DrawLightHighlight(line);
                }

                Widgets.Label(leaderRect, $"{_leaderText.RawText}: {raid.Leader}");
                Widgets.Label(armyRect, $"{_troopText.RawText}: {raid.Army.Count:N0}");


                GUI.color = TimerGradient.Evaluate(1f - progress);
                Widgets.FillableBar(progressRect, progress, Texture2D.whiteTexture, null, true);
                GUI.color = colorCache;
            }

            GUI.EndGroup();
            listing.EndScrollView(ref viewPort);

            var footerRect = new Rect(0f, inRect.height - Text.LineHeight, inRect.width, Text.LineHeight);

            GUI.BeginGroup(footerRect);
            Widgets.Label(new Rect(0f, 0f, inRect.width, Text.LineHeight), _footerText);
            GUI.EndGroup();

            GUI.EndGroup();
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
