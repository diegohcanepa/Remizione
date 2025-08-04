using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Remizione.UI
{
    /// <summary>
    /// UICountdownMeter
    /// </summary>
    public class UICountdownMeter : GameObject
    {
        private SoundInstance? alarmSound;
        private int lastKnownValue = -1;
        private readonly GameSession session;
        private readonly TextSprite text;
        private readonly Vector2Tween scaleTween = new();

        // Constructor
        public UICountdownMeter(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            // Text
            this.text = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Red,
                PivotOrigin = RectanglePoint.Top,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.Top, 0, 0),
                Scale = ScaleInfo.Text.Giant,
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!session.IsCountdownActive)
                return;

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp, BlendState.AlphaBlend, null);
            text.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (!session.Countdown.IsBetween(0, GameSettings.CountdownWarning))
                return;

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
            }

            text.Update(gameTime);
        }

        #endregion

        // StopAlarm
        public void StopAlarm()
        {
            text.Tweens.Reset();
            text.Scale = ScaleInfo.Text.Giant;
            alarmSound?.Stop(3000);
            alarmSound = null;
        }
    }
}
