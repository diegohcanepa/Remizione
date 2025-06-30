using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione.UI
{
    /// <summary>
    /// CraftingMark
    /// </summary>
    public sealed class CraftingMark : GameObject
    {
        private readonly GameSession session;
        private readonly ImageSprite image;

        // Constructor
        public CraftingMark(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            this.image = new ImageSprite(Game, Atlases.Environment.CraftingMark)
            {
                PivotOrigin = RectanglePoint.Middle,
                Scale = ScaleInfo.UIElement.Medium
            };

            image.Tweens.OpacityTween = FloatTween.Create(TweenStyle.CubicInOut, .3f, .5f, 500, -1);
            image.Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.CubicInOut, image.Scale, image.Scale * .8f, 250, -1);
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (IsVisible)
            {
                Game.SpriteBatch.Begin(session.Camera);
                image.Draw(gameTime);
                Game.SpriteBatch.End();
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            IsVisible = false;
            if (session.Player == null || session.IsAwaiting)
                return;

            session.Player.GetCraftData(out var position, out var prop, out var canPlace);
            if (prop != null && position != null)
            {
                image.Position = position.Value;
                image.Color = canPlace ? ColorPalette.Text.Green : ColorPalette.Text.Red;
                IsVisible = true;
            }

            image.Update(gameTime);
        }

        #endregion

        // IsVisible
        public bool IsVisible { get; private set; }
    }
}
