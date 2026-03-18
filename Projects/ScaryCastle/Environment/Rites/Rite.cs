using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// Rite
    /// </summary>
    public abstract class Rite : GameThing
    {
        #region Private fields

        private int cooldown;
        private readonly Sprite areaMarker;

        #endregion

        // Constructor
        protected Rite(GameSession session, Item item, Vector2 castPosition)
            : base(session, string.Empty)
        {
            this.Item = item;
            this.Position = castPosition;
            this.cooldown = item.Definition.ExecutionDelay;
            this.RenderLayer = RenderLayer.OverBackground;

            this.Atlas = Atlases.Environment;
            this.Scale = ScaleInfo.UIElement.Small;
            this.areaMarker = new(Atlases.Environment.FindImage($"AreaMarker{item.Definition.AreaRange}"))
            {
                Color = new(ColorPalette.MouseCursorHighlightBlue),
                PivotOrigin = RectanglePoint.Center,
                Position = castPosition
            };
            areaMarker.Tweens.OpacityTween = FloatTween.Create(TweenStyle.CubicInOut, 1, .7f, 100, -1);
        }

        #region Protected members

        // OnCast
        protected virtual void OnCast()
        {
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (cooldown > 0)
            {
                areaMarker.Draw(gameTime);
                return;
            }

            base.OnDraw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (cooldown > 0)
            {
                areaMarker.Update(gameTime);
                cooldown -= gameTime.ElapsedGameTime.Milliseconds;
                if (cooldown <= 0)
                {
                    RenderLayer = RenderLayer.Default;

                    OnCast();

                    if (Room != null)
                        Item.Use(this, Room);
                }
                return;
            }

            base.OnUpdate(gameTime);

            if (!AnimationPlayer.IsPlaying)
                Unparent();
        }

        #endregion

        // Item
        public Item Item { get; }
    }
}
