using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// ShadowSpot
    /// </summary>
    public sealed class ShadowSpot : GameObject
    {
        #region Private fields

        private readonly ImageSprite spotImage;
        private readonly GameThing thing;

        #endregion

        #region Constructor

        // Constructor
        public ShadowSpot(GameThing thing)
            : base(thing.Game)
        {
            this.thing = thing;
            this.spotImage = new ImageSprite(Game)
            {
                Color = ColorPalette.ShadowSpot,
                PivotOrigin = RectanglePoint.Center
            };
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
                spotImage.Effects = thing.Effects;
                spotImage.Opacity = GetCurrentOpacity() * thing.Opacity;
                var x = thing.IsFlippedHorizontally ? -Offset.X : Offset.X;
                var y = Offset.Y;
                spotImage.Position = thing.BoundingBox.GetPoint(RectanglePoint.Bottom, x, y);

                if (thing.Altitude > 0)
                    spotImage.Y += thing.Altitude;

                spotImage.Draw(gameTime);
            }
        }

        #endregion

        // Offset
        public Vector2 Offset { get; set; }

        // Size
        public int Size
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    spotImage.Image = Atlases.Environment.FindImage($"{nameof(ShadowSpot)}{value}");
                }
            }
        }
    }
}
