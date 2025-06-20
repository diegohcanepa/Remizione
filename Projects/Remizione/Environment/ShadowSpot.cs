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
        private ShadowSpotSize size;
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

            this.Size = ShadowSpotSize.W6;
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
            if (size != ShadowSpotSize.None)
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
        public ShadowSpotSize Size
        {
            get => size;
            set
            {
                if (value != size)
                {
                    this.size = value;
                    spotImage.Image = value == ShadowSpotSize.None ? null : Atlases.Environment.GetImage(nameof(ShadowSpot) + value.ToString());
                }
            }
        }
    }
}
