using Engendro;
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
        private int lastKnownValue = -1;
        private readonly GameSession session;
        private readonly TextSprite text;

        // Constructor
        public UICountdownMeter(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            // Text
            this.text = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Red,
                PivotOrigin = RectanglePoint.LeftTop,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.LeftTop, 3, 7),
                Scale = ScaleInfo.Text.Giant,
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!session.Countdown.IsBetween(0, GameSettings.CountdownAlert))
                return;

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp, BlendState.AlphaBlend, null);
            text.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (!session.Countdown.IsBetween(0, GameSettings.CountdownAlert))
                return;

            if (lastKnownValue != session.Countdown)
            {
                lastKnownValue = session.Countdown;
                var t = TimeSpan.FromMilliseconds(session.Countdown);
                text.Text = string.Format("{0:D2}:{1:D2}", (int)t.TotalMinutes, t.Seconds);
            }
        }

        #endregion
    }
}
