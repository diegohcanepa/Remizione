using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ShadowSpot
    /// </summary>
    public sealed class ShadowSpot : GameObject
    {
        #region Private fields

        private readonly Actor actor;
        private int size;
        private readonly ImageSprite spotImage;

        #endregion

        #region Constructor

        // Constructor
        public ShadowSpot(Actor actor)
            : base(actor.Game)
        {
            this.actor = actor;
            this.spotImage = new ImageSprite(Game)
            {
                Color = ColorPalette.ShadowSpot,
                PivotOrigin = RectanglePoint.Middle
            };

            this.Size = 6;
        }

        #endregion

        #region Private members

        // GetCurrentOpacity
        private float GetCurrentOpacity()
        {
            if (spotImage.Tweens.OpacityTween == null)
                return ColorPalette.ShadowOpacity;
            else
                return spotImage.Tweens.OpacityTween.CurrentValue;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!spotImage.IsEmpty)
            {
                spotImage.Effects = actor.Effects;
                spotImage.Opacity = GetCurrentOpacity() * actor.Opacity;
                var x = actor.IsFlippedHorizontally ? -Offset.X : Offset.X;
                var y = Offset.Y;
                spotImage.Position = actor.BoundingBox.GetPoint(RectanglePoint.Bottom, x, y);

                if (actor.Altitude > 0)
                    spotImage.Y += actor.Altitude;

                spotImage.Draw(gameTime);
            }
        }

        #endregion

        // Offset
        public Vector2 Offset { get; set; }

        // Size
        public int Size
        {
            get => size;
            set
            {
                if (value != size)
                {
                    this.size = value;
                    spotImage.Image = Atlases.Environment.GetImage($"{nameof(ShadowSpot)}{value}");
                }
            }
        }
    }
}
