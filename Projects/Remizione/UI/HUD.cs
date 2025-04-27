using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remizione.UI;
using Windows.Gaming.Input;

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
        private readonly TextSprite narrationText;
        private readonly ImageSprite savingIcon;
        private readonly GameSession session;
        private readonly Meter staminaMeter;

        // Constructor
        public HUD(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            // Context menu
            this.ContextMenu = new UIContextMenu(session.Game, session.Camera, Fonts.CommonOutline)
            {
                OptionTextScale = ScaleInfo.Text.Medium,
                UseSelector = false
            };

            ContextMenu.AddOption("@CombatItems.Attack", "Attack");
            ContextMenu.AddOption("@CombatItems.Guard", "Guard");
            ContextMenu.AddOption("@CombatItems.UseItem", "Use item...");

            // DestinationMark
            this.DestinationMark = new DestinationMark(session);

            // Echo message
            this.EchoMessage = new EchoMessage(session.Game);

            // Cycle info
            this.cycleInfo = new CycleInfo(session);

            // Quick slots
            this.QuickSlots = new QuickSlots(session);

            // Saving icon
            this.savingIcon = new ImageSprite(Game, Atlases.UI.SavingIcon)
            {
                PivotOrigin = RectanglePoint.RightTop,
                Position = Screen.Area.GetPoint(RectanglePoint.RightTop, -8, 6)
            };

            this.hpMeter = new Meter(Game, ColorPalette.HPMeter.Back, ColorPalette.HPMeter.Fore) { Position = new(6, 5) };
            this.fpMeter = new Meter(Game, ColorPalette.FPMeter.Back, ColorPalette.FPMeter.Fore) { Position = new(6, 8) };
            this.staminaMeter = new Meter(Game, ColorPalette.StaminaMeter.Back, ColorPalette.StaminaMeter.Fore) { Position = new(6, 11) };

            // GP score
            this.gpScore = new ScoreText(session.Game)
            {
                HideZero = true,
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.SafeArea.GetPoint(RectanglePoint.RightBottom, -2, 0),
                Scale = ScaleInfo.Text.Large
            };

            this.narrationText = new TextSprite(Game, Fonts.MainOutline)
            {
                Color = ColorPalette.Text.Light,
                MaximumWidth = (int)(Screen.NativeWidth * .7f),
                PauseOnPunctuationMarks = false,
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.Area.GetPoint(RectanglePoint.Bottom, 0, -10),
                Scale = ScaleInfo.Text.Medium
            };
        }

        #region Private members

        // DrawMeters
        private void DrawMeters(GameTime gameTime, Actor actor)
        {
            Game.SpriteBatch.Begin(Game.Camera);

            // HP
            hpMeter.MaximumValue = actor.MaxHP;
            hpMeter.Value = actor.HP;
            hpMeter.Draw(gameTime);

            // FP
            fpMeter.MaximumValue = actor.MaxFP;
            fpMeter.Value = actor.FP;
            fpMeter.Draw(gameTime);

            // Stamina
            staminaMeter.MaximumValue = actor.MaxStamina;
            staminaMeter.Value = actor.Stamina;
            staminaMeter.Draw(gameTime);

            Game.SpriteBatch.End();
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            narrationText.Draw(gameTime);
            Game.SpriteBatch.End();

            if (session.Player != null && session.FullHUD)
            {
                cycleInfo.Draw(gameTime);
                DrawMeters(gameTime, session.Player);
                //QuickSlots.Draw(gameTime);
                gpScore.Draw(gameTime);
            }

            if (session.CombatManager.CurrentActor == session.Player)
                ContextMenu.Draw(gameTime);

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
            if (session.Player != null)
                ContextMenu.Position = session.Player.BoundingBox.GetPoint(RectanglePoint.Top, -ContextMenu.BoundingBox.Width / 2, -ContextMenu.BoundingBox.Height);

            ContextMenu.Update(gameTime);

            narrationText.Update(gameTime);

            if (session.Player != null)
            {
                fpMeter.Update(gameTime);
                hpMeter.Update(gameTime);
                staminaMeter.Update(gameTime);
                gpScore.Score = session.Player.Stats.GP;
                gpScore.Update(gameTime);
            }

            cycleInfo.Update(gameTime);
            DestinationMark.Update(gameTime);
            EchoMessage.Update(gameTime);
            QuickSlots.Update(gameTime);
            savingIcon.Update(gameTime);
        }

        #endregion

        public UIContextMenu ContextMenu { get; }

        // DestinationMark
        public DestinationMark DestinationMark { get; }

        // EchoMessage
        public EchoMessage EchoMessage { get; }

        // QuickSlots
        public QuickSlots QuickSlots { get; }

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
            QuickSlots.Invalidate();
        }

        // ShowSavingIcon
        public void ShowSavingIcon()
        {
            savingIcon.Tweens.OpacityTween = FloatTween.Create(TweenStyle.QuadraticInOut, 1, .8f, 300, 10);
        }
    }
}
