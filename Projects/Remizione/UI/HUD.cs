using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remizione.UI;

namespace Remizione
{
    /// <summary>
    /// HUD
    /// </summary>
    public sealed class HUD : GameObject
    {
        private readonly CycleInfo cycleInfo;
        private readonly Meter fpMeter;
        private readonly ScoreText gpScore;
        private readonly Meter hpMeter;
        private readonly TextSprite[] labels;
        private readonly TextSprite narrationText;
        private readonly ImageSprite savingIcon;
        private readonly GameSession session;
        private readonly Meter willpowerMeter;
        private readonly Meter willpowerMeterLarge;
        private readonly TextSprite willpowerMeterLabel;

        // Constructor
        public HUD(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            // DestinationMark
            this.DestinationMark = new DestinationMark(session);

            // Echo message
            this.EchoMessage = new EchoMessage(session.Game);

            // Cycle info
            this.cycleInfo = new CycleInfo(session);

            // Saving icon
            this.savingIcon = new ImageSprite(Game, Atlases.UI.SavingIcon)
            {
                PivotOrigin = RectanglePoint.RightTop,
                Position = Screen.Area.GetPoint(RectanglePoint.RightTop, -8, 6)
            };

            this.hpMeter = new Meter(Game, ColorPalette.HPMeter.Back, ColorPalette.HPMeter.Fore, 2.4f);
            this.fpMeter = new Meter(Game, ColorPalette.FPMeter.Back, ColorPalette.FPMeter.Fore, 2.4f);
            this.willpowerMeter = new Meter(Game, ColorPalette.WillpowerMeter.Back, ColorPalette.WillpowerMeter.Fore, 2.4f);

            labels = new TextSprite[3];
            labels[0] = CreateLabel(Game, TextRepository.GetValue("@Attributes.Secondary.Vitality"));
            labels[1] = CreateLabel(Game, TextRepository.GetValue("@Attributes.Secondary.Faith"));
            labels[2] = CreateLabel(Game, TextRepository.GetValue("@Attributes.Secondary.Willpower"));

            LayoutMeters();

            this.willpowerMeterLarge = new Meter(Game, ColorPalette.WillpowerMeter.Back, ColorPalette.WillpowerMeter.Fore, 3.6f)
            { 
                Alignment = HorizontalAlignment.Center,
                Position = Screen.SafeArea.GetPoint(RectanglePoint.Top, 0, 10)
            };

            // GP score
            this.gpScore = new ScoreText(session.Game)
            {
                HideZero = true,
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.SafeArea.GetPoint(RectanglePoint.RightBottom, -2, 0),
                Scale = ScaleInfo.Text.Large
            };

            this.narrationText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.LightRed,
                MaximumWidth = (int)(Screen.NativeWidth * .7f),
                PauseOnPunctuationMarks = false,
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.Area.GetPoint(RectanglePoint.Bottom, 0, -10),
                Scale = ScaleInfo.Text.Large
            };

            // Willpower label
            this.willpowerMeterLabel = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.LightRed,
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.SafeArea.GetPoint(RectanglePoint.Top, 0, 11),
                Scale = ScaleInfo.Text.Medium,
                Text = "@Attributes.Secondary.Willpower"
            };
        }

        #region Private members

        // CreateLabel
        private static TextSprite CreateLabel(EngendroGame game, string key)
        {
            return new TextSprite(game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.LightRed,
                PivotOrigin = RectanglePoint.Right,
                Position = Screen.SafeArea.GetPoint(RectanglePoint.RightTop, -3, 3),
                Scale = ScaleInfo.Text.Small,
                Text = key
            };
        }

        // DrawMeters
        private void DrawMeters(GameTime gameTime, Actor actor)
        {
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            labels[0].Draw(gameTime);
            labels[1].Draw(gameTime);
            if (!session.CombatManager.IsActive)
                labels[2].Draw(gameTime);
            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(Game.Camera);

            // HP
            hpMeter.MaximumValue = actor.MaxHP;
            hpMeter.Value = actor.HP;
            hpMeter.Draw(gameTime);

            // FP
            fpMeter.MaximumValue = actor.MaxFP;
            fpMeter.Value = actor.FP;
            fpMeter.Draw(gameTime);

            willpowerMeterLarge.MaximumValue = actor.MaxWillpower;
            willpowerMeterLarge.Value = actor.Willpower;

            willpowerMeter.MaximumValue = actor.MaxWillpower;
            willpowerMeter.Value = actor.Willpower;

            // Willpower
            if (session.CombatManager.IsActive)
                willpowerMeterLarge.Draw(gameTime);
            else
                willpowerMeter.Draw(gameTime);

            Game.SpriteBatch.End();
        }

        // LayoutMeters
        private void LayoutMeters()
        {
            float x = 0;
            for (var i = 0; i < labels.Length; i++)
            {
                if (labels[i].BoundingBox.Width > x)
                    x = labels[i].BoundingBox.Width;
            }

            x += 5;
            float y = 5;
            for (var i = 0; i < labels.Length; i++)
            {
                labels[i].X = x;
                labels[i].Y = y;

                y += labels[i].BoundingBox.Height - 1;
            }

            hpMeter.Position = labels[0].BoundingBox.GetPoint(RectanglePoint.RightTop, 1, .5f);
            fpMeter.Position = labels[1].BoundingBox.GetPoint(RectanglePoint.RightTop, 1, .5f);
            willpowerMeter.Position = labels[2].BoundingBox.GetPoint(RectanglePoint.RightTop, 1, .5f);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            if (session.CombatManager.IsActive)
                willpowerMeterLabel.Draw(gameTime);
            narrationText.Draw(gameTime);
            Game.SpriteBatch.End();

            if (session.Player != null && session.FullHUD)
            {
                //cycleInfo.Draw(gameTime);
                DrawMeters(gameTime, session.Player);
                gpScore.Draw(gameTime);
            }

            EchoMessage.Draw(gameTime);

            if (savingIcon.Tweens.IsTweening)
            {
                Game.SpriteBatch.Begin(Game.Camera);
                savingIcon.Draw(gameTime);
                Game.SpriteBatch.End();
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            narrationText.Update(gameTime);

            if (session.Player != null)
            {
                fpMeter.Update(gameTime);
                hpMeter.Update(gameTime);
                willpowerMeter.Update(gameTime);
                gpScore.Score = session.Player.Stats.GP;
                gpScore.Update(gameTime);
                willpowerMeterLabel.Update(gameTime);
                willpowerMeterLarge.Update(gameTime);
            }

            cycleInfo.Update(gameTime);
            DestinationMark.Update(gameTime);
            EchoMessage.Update(gameTime);
            savingIcon.Update(gameTime);
        }

        #endregion

        // DestinationMark
        public DestinationMark DestinationMark { get; }

        // EchoMessage
        public EchoMessage EchoMessage { get; }

        // NarrationText
        public string NarrationText
        {
            get => narrationText.Text ?? string.Empty;
            set
            {
                if (narrationText.Text != value)
                {
                    narrationText.Text = value;
                    narrationText.Tweens.OpacityTween = FloatTween.Create(TweenStyle.CubicInOut, 0, 1, 500);
                }
            }
        }

        // Reset
        public void Reset()
        {
        }

        // ShowSavingIcon
        public void ShowSavingIcon()
        {
            savingIcon.Tweens.OpacityTween = FloatTween.Create(TweenStyle.QuadraticInOut, 1, .8f, 300, 10);
        }
    }
}
