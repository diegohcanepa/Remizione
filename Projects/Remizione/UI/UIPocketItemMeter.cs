using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// UIPocketItemMeter
    /// </summary>
    public class UIPocketItemMeter : GameObject
    {
        #region Private members

        private readonly Sprite icon;
        private readonly Vector2 iconScale = Vector2.One;
        private readonly FloatTween rotationTween = new();
        private readonly Vector2Tween scaleTween = new();
        private readonly TextSprite valueText;

        #endregion

        #region Constructor

        // Constructor
        public UIPocketItemMeter(PocketItemType pocketItemType)
            : base()
        {
            this.PocketItemType = pocketItemType;

            // Value text
            this.valueText = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.Bottom,
                Scale = ScaleInfo.Text.ExtraLarge,
                Text = "00"
            };

            // Icon
            this.icon = new(Atlases.UI.GetImage($"{pocketItemType}"))
            {
                PivotOrigin = RectanglePoint.Bottom
            };

            this.Value = 0;
        }

        #endregion

        #region Private members

        // Animate
        private void Animate()
        {
            if (!icon.Tweens.IsTweening)
            {
                rotationTween.Start(TweenStyle.QuadraticInOut, 0, 15, 50, 6);
                icon.Tweens.RotationTween = rotationTween;

                scaleTween.Start(TweenStyle.QuadraticInOut, iconScale, iconScale * 1.3f, 150, 2);
                icon.Tweens.ScaleTween = scaleTween;
            }
        }

        // PlayCollectSound
        private void PlayCollectSound()
        {
            if (PocketItemType == PocketItemType.Coin)
            {
                Sound.Play(SoundNames.CollectCoin);
            }
            else
            {
                Sound.Play(SoundNames.PickupKey);
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            icon.Draw(gameTime);
            valueText.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            icon.Update(gameTime);
        }

        #endregion

        // BoundingBox
        public RectangleF BoundingBox => RectangleF.Union(icon.BoundingBox, valueText.BoundingBox);

        // PocketItemType
        public PocketItemType PocketItemType { get; }

        // Position
        public Vector2 Position
        {
            get => valueText.Position;
            set
            {
                if (value != valueText.Position)
                {
                    valueText.Position = value;
                    icon.Position = valueText.BoundingBox.GetPoint(RectanglePoint.Top);
                }
            }
        }

        // Value
        public int Value
        {
            get;
            set
            {
                if (value != field)
                {
                    if (field != -1)
                    {
                        Animate();
                        PlayCollectSound();
                    }

                    field = value;
                    valueText.Text = $"x{field}";
                }
            }
        } = -1;
    }
}
