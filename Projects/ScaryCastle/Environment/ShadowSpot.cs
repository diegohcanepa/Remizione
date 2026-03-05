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

        private readonly Sprite spotImage;
        private readonly GameThing thing;

        #endregion

        #region Constructor

        // Constructor
        public ShadowSpot(GameThing thing)
            : base(thing.Game)
        {
            this.thing = thing;
            this.spotImage = new Sprite(Game)
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
                spotImage.Position = thing.GetAnchoredPosition(AnchorPosition);

                if (thing.Altitude > 0)
                    spotImage.Y += thing.Altitude;

                spotImage.Draw(gameTime);
            }
        }

        #endregion

        // AnchorPosition
        public Vector2 AnchorPosition { get; set; }

        // Size
        public int Size
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    spotImage.RenderImage = Atlases.Environment.FindImage($"{nameof(ShadowSpot)}{value}");
                }
            }
        }
    }
}
