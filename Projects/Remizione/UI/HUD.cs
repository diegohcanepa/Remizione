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
        private readonly ImageSprite angerIconLarge;
        private readonly Meter angerMeter;
        private readonly Meter angerMeterLarge;
        private readonly TextSprite angerMeterLabel;
        private readonly CycleInfo cycleInfo;
        private readonly Meter faithMeter;
        private readonly ScoreText gpScore;
        private readonly Meter hpMeter;
        private readonly ImageSprite[] meterIcons;
        private readonly TextSprite narrationText;
        private readonly ImageSprite savingIcon;
        private readonly GameSession session;

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

            this.hpMeter = new Meter(Game, ColorPalette.HPMeter.Back, ColorPalette.HPMeter.Fore, 2.8f);
            this.faithMeter = new Meter(Game, ColorPalette.FaithMeter.Back, ColorPalette.FaithMeter.Fore, 2.8f);
            this.angerMeter = new Meter(Game, ColorPalette.Anger.Back, ColorPalette.Anger.Fore, 2.8f);

            meterIcons = new ImageSprite[3];
            meterIcons[0] = new ImageSprite(Game, Atlases.UI.SpiritIcon) { Scale = ScaleInfo.UIIcon.Small };
            meterIcons[1] = new ImageSprite(Game, Atlases.UI.FaithIcon) { Scale = ScaleInfo.UIIcon.Small };
            meterIcons[2] = new ImageSprite(Game, Atlases.UI.AngerIcon) { Scale = ScaleInfo.UIIcon.Small };

            meterIcons[0].Position = new(4);
            meterIcons[1].Position = meterIcons[0].BoundingBox.GetPoint(RectanglePoint.LeftBottom, 0, .5f);
            meterIcons[2].Position = meterIcons[1].BoundingBox.GetPoint(RectanglePoint.LeftBottom, 0, .5f);

            hpMeter.Position = new(10, 5);
            faithMeter.Position = new(10, 10);
            angerMeter.Position = new(10, 15);

            this.angerIconLarge = new ImageSprite(Game, Atlases.UI.AngerIcon) { PivotOrigin = RectanglePoint.Right, Scale = ScaleInfo.UIIcon.Medium };

            this.angerMeterLarge = new Meter(Game, ColorPalette.Anger.Back, ColorPalette.Anger.Fore, 3.6f)
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

            // Anger label
            this.angerMeterLabel = new TextSprite(Game, Fonts.MainOutline)
            {
                Color = ColorPalette.Text.Dark,
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.SafeArea.GetPoint(RectanglePoint.Top, 0, 11),
                Scale = ScaleInfo.Text.Medium,
                Text = "@Attributes.Secondary.Anger"
            };
        }

        #region Private members

        // DrawMeters
        private void DrawMeters(GameTime gameTime, Actor actor)
        {
            Game.SpriteBatch.Begin(Game.Camera);

            meterIcons[0].Draw(gameTime);
            meterIcons[1].Draw(gameTime);

            if (session.CombatManager.IsActive)
                angerIconLarge.Draw(gameTime);
            else
                meterIcons[2].Draw(gameTime);

            // HP
            hpMeter.MaximumValue = actor.MaxHP;
            hpMeter.Value = actor.HP;
            hpMeter.Draw(gameTime);

            // FP
            faithMeter.MaximumValue = actor.MaxFaith;
            faithMeter.Value = actor.Faith;
            faithMeter.Draw(gameTime);

            angerMeterLarge.MaximumValue = actor.MaxAnger;
            angerMeterLarge.Value = actor.Anger;

            angerMeter.MaximumValue = actor.MaxAnger;
            angerMeter.Value = actor.Anger;

            // Anger
            if (session.CombatManager.IsActive)
                angerMeterLarge.Draw(gameTime);
            else
                angerMeter.Draw(gameTime);

            Game.SpriteBatch.End();
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            if (session.CombatManager.IsActive)
                angerMeterLabel.Draw(gameTime);
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
                faithMeter.Update(gameTime);
                hpMeter.Update(gameTime);
                angerMeter.Update(gameTime);
                gpScore.Score = session.Player.Stats.GP;
                gpScore.Update(gameTime);
                angerMeterLabel.Update(gameTime);
                angerMeterLarge.Update(gameTime);
            }

            cycleInfo.Update(gameTime);
            DestinationMark.Update(gameTime);
            EchoMessage.Update(gameTime);
            savingIcon.Update(gameTime);

            if (session.CombatManager.IsActive)
                angerIconLarge.Position = angerMeterLarge.BoundingBox.GetPoint(RectanglePoint.Left, -1, 0);
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
