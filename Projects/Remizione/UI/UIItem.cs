using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// UIItem
    /// </summary>
    public sealed class UIItem : GameObject
    {
        private readonly ImageSprite image;
        private Item? item;
        private readonly ItemVisualState state;

        // Constructor
        public UIItem(EngendroGame game)
            : base(game)
        {
            this.image = new(game)
            {
                PivotOrigin = RectanglePoint.Middle
            };

            Reset();
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            image.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            image.Update(gameTime);
        }

        #endregion

        // ChangeVisualState
        public void ChangeVisualState(ItemVisualState state, bool immediate)
        {
            const int tweenDuration = 300;

            if (immediate)
            {
                image.Tweens.Reset();
                image.Color = state == ItemVisualState.Active || state == ItemVisualState.Combine ? ColorPalette.UIItem.Active : ColorPalette.UIItem.Inactive;
                image.Scale = state == ItemVisualState.Active ? ScaleInfo.UIItem.Active : ScaleInfo.UIItem.Inactive;
                return;
            }

            if (state == ItemVisualState.Combine)
            {
                image.Scale = ScaleInfo.UIItem.Inactive;
                image.Tweens.ColorTween = ColorTween.Create(TweenStyle.CubicInOut, image.Color, Color.White, tweenDuration);
                image.Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.QuadraticInOut, image.Scale, image.Scale * 1.1f, tweenDuration, -1);
            }
            else if (state == ItemVisualState.Combined)
            {
                image.Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.CubicIn, Vector2.Zero, ScaleInfo.UIItem.Active, tweenDuration * 4);
                image.Tweens.ColorTween = ColorTween.Create(TweenStyle.CubicInOut, Color.Transparent, ColorPalette.UIItem.Active, tweenDuration);
            }
            else if (state == ItemVisualState.Active)
            {
                image.Tweens.ColorTween = ColorTween.Create(TweenStyle.CubicInOut, image.Color, Color.White, tweenDuration);
                image.Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.CubicInOut, image.Scale, ScaleInfo.UIItem.Active, tweenDuration);
            }
            else
            {
                image.Tweens.ColorTween = ColorTween.Create(TweenStyle.CubicInOut, image.Color, ColorPalette.UIItem.Inactive, tweenDuration);
                image.Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.CubicInOut, image.Scale, ScaleInfo.UIItem.Active, tweenDuration);
            }
        }

        // Item
        public Item? Item
        {
            get => item;
            set
            {
                if (value != item)
                {
                    item = value;
                    image.Image = item?.MetaItem.Image;
                }
            }
        }

        // Position
        public Vector2 Position
        {
            get => image.Position;
            set => image.Position = value;
        }

        // Reset
        public void Reset()
        {
            Item = null;
            image.Image = null;
            Position = Vector2.Zero;
            ChangeVisualState(ItemVisualState.Inactive, true);
        }

        // State
        public ItemVisualState State { get; }
    }
}
