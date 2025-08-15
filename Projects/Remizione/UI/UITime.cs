using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Remizione.UI
{
    /// <summary>
    /// UITime
    /// </summary>
    public class UITime : GameObject
    {
        private SoundInstance? alarmSound;
        private readonly ImageSprite icon;
        private int lastKnownValue = -1;
        private readonly GameSession session;
        private readonly TextSprite text;
        private readonly Vector2Tween scaleTween = new();

        // Constructor
        public UITime(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            // Icon
            this.icon = new(Game, Atlases.UI.ClockIcon)
            {
                PivotOrigin = RectanglePoint.Top,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.Top, -10, -1),
                Scale = ScaleInfo.UIElement.Medium
            };

            // Text
            this.text = new(Game, Fonts.CommonOutline)
            {
                PivotOrigin = RectanglePoint.Left,
                Position = icon.BoundingBox.GetPoint(RectanglePoint.Right, 0, .5f),
                Scale = ScaleInfo.Text.Giant
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp);
            icon.Draw(gameTime);
            text.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (lastKnownValue != session.Countdown)
            {
                if (alarmSound == null && session.Countdown < GameSettings.CountdownCritical)
                {
                    alarmSound = Sound.Play(SoundNames.ExitAlarm, true);
                    scaleTween.Start(TweenStyle.QuadraticInOut, ScaleInfo.Text.Giant, ScaleInfo.Text.Giant * 1.04f, 300, -1);
                    text.Tweens.ScaleTween = scaleTween;
                }

                lastKnownValue = session.Countdown;
                var t = TimeSpan.FromMilliseconds(session.Countdown);
                text.Text = string.Format("{0:D2}:{1:D2}", (int)t.TotalMinutes, t.Seconds);

                if (lastKnownValue <= GameSettings.CountdownCritical)
                    text.Color = ColorPalette.Text.Red;
                else if (lastKnownValue <= GameSettings.CountdownWarning)
                    text.Color = ColorPalette.Text.Highlight;
                else
                    text.Color = ColorPalette.Text.Default;
            }

            text.Update(gameTime);
        }

        #endregion

        // Reset
        public void StopAlarm()
        {
            text.Tweens.Reset();
            text.Scale = ScaleInfo.Text.Giant;
            alarmSound?.Stop(3000);
            alarmSound = null;
        }
    }
}
