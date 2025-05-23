using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remizione.UI;
using System;

namespace Remizione
{
    /// <summary>
    /// HUD
    /// </summary>
    public sealed class HUD : GameObject, IInputHandler
    {
        #region Private fields

        private readonly UIControl bagIcon;
        private readonly ScoreText gpScore;
        private readonly TextSprite interactionTarget;
        private readonly UIDerivedStats playerStats;
        private readonly UIControl prayerIcon;
        private readonly ImageSprite savingIcon;
        private readonly GameSession session;
        private readonly TextSprite statusText;

        #endregion

        #region Constructor

        // Constructor
        public HUD(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            this.playerStats = new(session.Game);

            // DestinationMark
            this.DestinationMark = new DestinationMark(session);

            // Log
            this.Log = new(Game);

            // Saving icon
            this.savingIcon = new ImageSprite(Game, Atlases.UI.SavingIcon)
            {
                PivotOrigin = RectanglePoint.RightTop,
                Position = Screen.Area.GetPoint(RectanglePoint.RightTop, -8, 6)
            };

            // GP score
            this.gpScore = new ScoreText(session.Game)
            {
                HideZero = true,
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.SafeArea.GetPoint(RectanglePoint.RightBottom, -2, 0),
                Scale = ScaleInfo.Text.VeryLarge
            };

            // Interaction target
            this.interactionTarget = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                MaximumWidth = (int)(Screen.NativeWidth * .8f),
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.SafeArea.GetPoint(RectanglePoint.Bottom, 0, -6),
                Scale = ScaleInfo.Text.Large
            };

            // Message text
            this.statusText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Terra,
                PivotOrigin = RectanglePoint.Top,
                Position = Screen.Area.GetPoint(RectanglePoint.Top, 0, 8),
                Scale = ScaleInfo.Text.VeryLarge
            };

            // Prayer icon
            this.prayerIcon = new UIControl(Game)
            {
                DisplayMode = UIControlDisplayMode.ImageOnly,
                ImageName = "PrayerIcon",
                ImageScale = ScaleInfo.UIElement.Large,
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = Screen.SafeArea.GetPoint(RectanglePoint.LeftBottom, 4, -4),
            };

            // Bag icon
            this.bagIcon = new UIControl(Game)
            {
                DisplayMode = UIControlDisplayMode.ImageOnly,
                ImageName = "BagIcon",
                ImageScale = ScaleInfo.UIElement.Large,
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = prayerIcon.BoundingBox.GetPoint(RectanglePoint.RightBottom, 2, 0)
            };
        }

        #endregion

        #region Private members

        // FormatSentenceText
        private static string FormatSentenceText(string name)
        {
            const string dots = "...";

            int totalLength = dots.Length + name.Length + dots.Length;

            Span<char> buffer = stackalloc char[totalLength];

            // Copy prefix
            dots.AsSpan().CopyTo(buffer);
            int offset = dots.Length;

            // Copy name
            name.AsSpan().CopyTo(buffer.Slice(offset));
            offset += name.Length;

            // Copy suffix
            dots.AsSpan().CopyTo(buffer.Slice(offset));

            // Return string from buffer
            return new string(buffer);
        }

        // UpdateInteractionTarget
        private void UpdateInteractionTarget()
        {
            if (session.IsCurrentScene && MouseCursor.Instance.State != MouseCursorState.Wait &&
                session.Player?.InteractiveTarget is GameThing target &&
                (!session.TargetMode || target.CanBeTargeted))
            {
                if (target != interactionTarget.Tag)
                {
                    interactionTarget.Tag = target;
                    interactionTarget.Text = FormatSentenceText(target.LocalizedDisplayName);
                }
            }
            else
            {
                interactionTarget.Text = null;
                interactionTarget.Tag = null;
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            playerStats.Draw(gameTime);

            if (session.IsCurrentScene)
            {
                Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
                statusText.Draw(gameTime);
                interactionTarget.Draw(gameTime);
                Game.SpriteBatch.End();
            }

            if (session.Player != null && session.FullHUD)
            {
                gpScore.Draw(gameTime);
                prayerIcon.Draw(gameTime);
                bagIcon.Draw(gameTime);
            }

            Log.Draw(gameTime);

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
            playerStats.Update(gameTime);

            UpdateInteractionTarget();
            statusText.Update(gameTime);

            if (session.Player != null)
            {
                gpScore.Score = session.Player.Stats.GP;
                gpScore.Update(gameTime);
            }

            DestinationMark.Update(gameTime);
            Log.Update(gameTime);
            savingIcon.Update(gameTime);
        }

        #endregion

        // CanHandleInput
        public bool CanHandleInput => throw new NotImplementedException();

        // DestinationMark
        public DestinationMark DestinationMark { get; }

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
        }

        // Log
        public UILog Log { get; }

        // Reset
        public void Reset()
        {
            playerStats.Actor = session.Player;
        }

        // ShowSavingIcon
        public void ShowSavingIcon()
        {
            savingIcon.Tweens.OpacityTween = FloatTween.Create(TweenStyle.QuadraticInOut, 1, .8f, 300, 10);
        }

        // Status
        public string Status
        {
            get => statusText.Text ?? string.Empty;
            set
            {
                if (statusText.Text != value)
                {
                    statusText.Text = value;
                    statusText.Tweens.OpacityTween = FloatTween.Create(TweenStyle.CubicInOut, 0, 1, 500);
                }
            }
        }
    }
}
