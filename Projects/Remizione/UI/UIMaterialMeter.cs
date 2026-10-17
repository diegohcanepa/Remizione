using Adberration;
using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// UIMaterialMeter
    /// </summary>
    public sealed class UIMaterialMeter : SessionGameObject<GameSession>
    {
        #region Private fields

        private readonly TextSprite amountText;
        private readonly Sprite icon;
        private int lastKnownCount = -1;
        private readonly Material material;
        private readonly FloatTween rotationTween = new();
        private readonly Vector2Tween scaleTween = new();

        #endregion

        #region Constructor

        // Constructor
        public UIMaterialMeter(GameSession session, Material material, Vector2 position)
            : base(session)
        {
            this.material = material;

            // Amount
            this.amountText = new TextSprite(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.TerraDark,
                PivotOrigin = RectanglePoint.Bottom,
                Position = position,
                Scale = ScaleInfo.Text.Large,
                Text = "00"
            };

            // Icon
            this.icon = new(Atlases.UI.GetImage($"{material}Icon"))
            {
                PivotOrigin = RectanglePoint.Center,
                Position = amountText.BoundingBox.GetPoint(RectanglePoint.Top, 0, -5)
            };
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            icon.Draw(gameTime);
            if (lastKnownCount > 0)
                amountText.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            icon.Update(gameTime);

            if (lastKnownCount != Session.PlayerData.GetMaterialCount(material))
            {
                lastKnownCount = Session.PlayerData.GetMaterialCount(material);
                icon.Opacity = lastKnownCount > 0 ? 1 : .5f;
                amountText.Text = $"{lastKnownCount}";
                amountText.Color = ColorPalette.Text.Terra;
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

            //Sound.Play(SoundNames.ItemAdd);
        }
    }
}