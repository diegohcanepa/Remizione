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
        private readonly Sprite flyingIcon = new() { PivotOrigin = RectanglePoint.Center };
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
                Position = amountText.BoundingBox.GetPoint(RectanglePoint.Top, 0, -6)
            };
        }

        #endregion

        #region Private members

        // AddCore
        private void AddCore()
        {
            rotationTween.Start(TweenStyle.QuadraticInOut, 0, 15, 50, 6);
            scaleTween.Start(TweenStyle.QuadraticInOut, Vector2.One, Vector2.One * 1.3f, 150, 2);

            icon.Tweens.RotationTween = rotationTween;
            icon.Tweens.ScaleTween = scaleTween;

            Session.PlayerData.AddMaterial(material);

            //Sound.Play(SoundNames.ItemAdd);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            flyingIcon.Draw(gameTime);
            icon.Draw(gameTime);
            if (lastKnownCount > 0)
                amountText.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            flyingIcon.Update(gameTime);
            icon.Update(gameTime);

            if (lastKnownCount != Session.PlayerData.GetMaterialCount(material))
            {
                lastKnownCount = Session.PlayerData.GetMaterialCount(material);
                icon.Opacity = lastKnownCount > 0 ? 1 : .5f;
                amountText.Text = $"{lastKnownCount}/5";
                amountText.Color = ColorPalette.Text.Terra;
            }
        }

        #endregion

        // Add
        public void Add(Vector2 worldPosition)
        {
            //Sound.Play(SoundNames.MoveToSack);

            var startPos = ((worldPosition - Session.Camera.Position) * Session.Camera.Zoom) + (new Vector2(Screen.NativeWidth, Screen.NativeHeight) * .5f);

            flyingIcon.RenderImage = icon.RenderImage;
            flyingIcon.Position = startPos;
            flyingIcon.Scale = ScaleInfo.UIElement.Large;
            flyingIcon.Tweens.PositionTween = Vector2Tween.Create(TweenStyle.CubicInOut, flyingIcon.Position, icon.BoundingBox.Center, 1400);
            flyingIcon.Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.CubicInOut, flyingIcon.Scale, ScaleInfo.UIElement.Large, 600, 2, AddCore);
        }
    }
}