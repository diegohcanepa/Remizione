using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione.UI
{
    /// <summary>
    /// CycleInfo
    /// </summary>
    public sealed class CycleInfo : GameObject
    {
        private Cycle? lastKnownCycle;
        private readonly GameSession session;
        private readonly TextSprite title;

        // Constructor
        public CycleInfo(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            this.title = new TextSprite(Game, Fonts.MainOutline)
            {
                PivotOrigin = RectanglePoint.RightTop,
                Position = Screen.SafeArea.GetPoint(RectanglePoint.RightTop, -3, 3),
                Scale = ScaleInfo.Text.Medium
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            title.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (lastKnownCycle == null || lastKnownCycle != session.Environment.CurrentCycle)
            {
                lastKnownCycle = session.Environment.CurrentCycle;
                title.Color = lastKnownCycle == Cycle.Penance ? ColorPalette.Cycle.Penance : ColorPalette.Cycle.Indulgence;
                title.Text = TextRepository.GetValue($"Cycle.{lastKnownCycle}");
            }
        }

        #endregion
    }
}
