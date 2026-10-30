using Adberration;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// UIMaterialMeter
    /// </summary>
    public sealed class UIMaterialMeter : SessionGameObject<GameSession>
    {
        #region Private fields

        private readonly UICapacity capacity = new() { PivotOrigin = RectanglePoint.Bottom, Position = Screen.HUDArea.GetPoint(RectanglePoint.LeftBottom, 20, 0) };
        private readonly Sprite flyingIcon = new() { PivotOrigin = RectanglePoint.Center };
        private readonly Sprite icon;
        private int lastKnownCount = -1;
        private readonly FloatTween rotationTween = new();
        private readonly Vector2Tween scaleTween = new();

        #endregion

        #region Constructor

        // Constructor
        public UIMaterialMeter(GameSession session)
            : base(session)
        {
            // Icon
            this.icon = new(Atlases.UI.CraftIcon)
            {
                PivotOrigin = RectanglePoint.Center,
                Position = capacity.BoundingBox.GetPoint(RectanglePoint.Top, 0, -6)
            };
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            flyingIcon.Draw(gameTime);
            icon.Draw(gameTime);
            capacity.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            flyingIcon.Update(gameTime);
            icon.Update(gameTime);

            if (lastKnownCount != Session.PlayerData.Materials)
            {
                lastKnownCount = Session.PlayerData.Materials;
                capacity.CurrentAmount = Session.PlayerData.Materials;
                capacity.MaximumAmount = 10;
                //capacity.Text = $"{Session.PlayerData.Materials}/{10}";
                //amountText.Color = inventory.IsFull ? Colors.Palette.Red : Colors.Palette.Terra;
            }
        }

        #endregion

        // Animate
        public void Animate()
        {
            rotationTween.Start(TweenStyle.QuadraticInOut, 0, 15, 50, 6);
            scaleTween.Start(TweenStyle.QuadraticInOut, Vector2.One, Vector2.One * 1.3f, 150, 2);

            icon.Tweens.RotationTween = rotationTween;
            icon.Tweens.ScaleTween = scaleTween;

            Sound.Play(SoundNames.ItemAdd);
        }

        // AnimateAdd
        public void AnimateAdd(Vector2 hudPos)
        {
            Sound.Play(SoundNames.CraftActive);

            flyingIcon.RenderImage = Atlases.UI.CraftIcon;
            flyingIcon.Position = hudPos;
            flyingIcon.Scale = ScaleInfo.UIElement.Large;
            flyingIcon.Tweens.PositionTween = Vector2Tween.Create(TweenStyle.CubicInOut, flyingIcon.Position, icon.BoundingBox.Center, 1400);
            flyingIcon.Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.CubicInOut, flyingIcon.Scale, ScaleInfo.UIElement.Large, 600, 2, Animate);
        }
    }
}