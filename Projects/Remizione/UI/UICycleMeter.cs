using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione.UI
{
    /// <summary>
    /// UICycleMeter
    /// </summary>
    public sealed class UICycleMeter : GameObject
    {
        private readonly ColorTween colorTween = new();
        private readonly TextSprite cycleText;
        private Cycle lastKnownCycle;
        private readonly FloatTween opacityTween = new() { BounceDelay = 2000 };
        private readonly GameSession session;
        private readonly TextSprite symbol1;
        private readonly TextSprite symbol2;
        private readonly TextSprite symbol3;
        private readonly Vector2 textScale = ScaleInfo.Text.ExtraLarge;

        // Constructor
        public UICycleMeter(GameSession session)
            : base(session.Game)
        {
            this.session = session;
            this.lastKnownCycle = session.Environment.Cycle;

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

            this.cycleText = new(Game, Fonts.CommonOutline)
            {
                Opacity = 0,
                PivotOrigin = RectanglePoint.Right,
                Position = symbol1.BoundingBox.GetPoint(RectanglePoint.LeftBottom, -2, -3),
                Scale = ScaleInfo.Text.VeryLarge,
            };
        }

        // ChangeCycle
        private void ChangeCycle()
        {
            Sound.Play(SoundNames.CycleHorn);

            lastKnownCycle = session.Environment.Cycle;

            if (lastKnownCycle == Cycle.Indulgence)
                colorTween.Start(TweenStyle.CubicIn, ColorPalette.Cycle.PenanceActive, ColorPalette.Cycle.Indulgence, 2000);

            opacityTween.Start(TweenStyle.Linear, 0, 1, 3000, 2);
            cycleText.Color = lastKnownCycle == Cycle.Penance ? ColorPalette.Cycle.PenanceActive : ColorPalette.Cycle.Indulgence;
            cycleText.Text = Localization.GetValue(lastKnownCycle);
            cycleText.Tweens.OpacityTween = opacityTween;
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearWrap);
            symbol1.Draw(gameTime);
            symbol2.Draw(gameTime);
            symbol3.Draw(gameTime);
            cycleText.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (lastKnownCycle != session.Environment.Cycle)
                ChangeCycle();

            colorTween.Update(gameTime);
            cycleText.Update(gameTime);

            if (session.Environment.Cycle == Cycle.Penance)
            {
                symbol1.Color = ColorPalette.Cycle.PenanceActive;
                symbol2.Color = ColorPalette.Cycle.PenanceActive;
                symbol3.Color = ColorPalette.Cycle.PenanceActive;
            }
            else
            {
                if (colorTween.IsRunning)
                {
                    symbol1.Color = colorTween.CurrentValue;
                    symbol2.Color = colorTween.CurrentValue;
                    symbol3.Color = colorTween.CurrentValue;
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
}
