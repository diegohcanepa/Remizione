using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione.UI
{
    /// <summary>
    /// UICycleMeter
    /// </summary>
    public sealed class UICycleMeter : GameObject
    {
        private readonly Vector2 textScale = ScaleInfo.Text.ExtraLarge;
        private readonly GameSession session;
        private readonly TextSprite symbol1;
        private readonly TextSprite symbol2;
        private readonly TextSprite symbol3;

        // Constructor
        public UICycleMeter(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            this.symbol1 = new(Game, Fonts.CommonOutline)
            {
                PivotOrigin = RectanglePoint.Top,
                Position = Screen.SafeArea.GetPoint(RectanglePoint.RightTop, -8, 1),
                Scale = textScale,
                Text = "6"
            };

            this.symbol2 = new(Game, Fonts.CommonOutline)
            {
                PivotOrigin = RectanglePoint.RightTop,
                Position = symbol1.BoundingBox.GetPoint(RectanglePoint.LeftBottom, -4f, 0),
                Rotation = 4,
                Scale = textScale,
                Text = "6"
            };

            this.symbol3 = new(Game, Fonts.CommonOutline)
            {
                PivotOrigin = RectanglePoint.RightTop,
                Position = symbol1.BoundingBox.GetPoint(RectanglePoint.RightBottom, 1.2f, 3),
                Rotation = -4,
                Scale = textScale,
                Text = "6"
            };
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearWrap);
            symbol1.Draw(gameTime);
            symbol2.Draw(gameTime);
            symbol3.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (session.Environment.Cycle == Cycle.Penance)
            {
                symbol1.Color = ColorPalette.Cycle.PenanceActive;
                symbol2.Color = ColorPalette.Cycle.PenanceActive;
                symbol3.Color = ColorPalette.Cycle.PenanceActive;
            }
            else
            {
                float ratio = (float)session.Environment.CycleCooldown / GameSettings.CycleDuration;

                symbol1.Color = ratio < .8f ? ColorPalette.Cycle.Penance : ColorPalette.Cycle.Indulgence;
                symbol2.Color = ratio < .5f ? ColorPalette.Cycle.Penance : ColorPalette.Cycle.Indulgence;
                symbol3.Color = ratio < .2f ? ColorPalette.Cycle.Penance : ColorPalette.Cycle.Indulgence;
            }
        }
    }
}
