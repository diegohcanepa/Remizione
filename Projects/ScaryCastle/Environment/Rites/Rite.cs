using Engendro;
using Microsoft.Xna.Framework;
using System.Linq;

namespace ScaryCastle
{
    /// <summary>
    /// Rite
    /// </summary>
    public abstract class Rite : GameThing
    {
        #region Private fields

        private int cooldown;
        private readonly ImageSprite areaRange;
        private readonly Item item;

        #endregion

        // Constructor
        protected Rite(GameSession session, Item item, Vector2 castPosition)
            : base(session, string.Empty)
        {
            this.item = item;
            this.Position = castPosition;
            this.cooldown = item.Definition.ExecutionDelay;

            this.Atlas = Atlases.Environment;
            this.Scale = ScaleInfo.UIElement.Small;
            this.areaRange = new(Game, Atlases.Environment.FindImage($"AreaRange{item.Definition.AreaRange}"))
            {
                Color = ColorPalette.Text.Red * .6f,
                PivotOrigin = RectanglePoint.Center,
                Position = castPosition
            };
            areaRange.Tweens.OpacityTween = FloatTween.Create(TweenStyle.CubicInOut, 1, .7f, 100, -1);
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
                areaRange.Draw(gameTime);
                return;
            }

            base.OnDraw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (cooldown > 0)
            {
                areaRange.Update(gameTime);
                cooldown -= gameTime.ElapsedGameTime.Milliseconds;
                if (cooldown <= 0)
                {
                    OnCast();

                    if (Room != null)
                        item.Use(this, areaRange.BoundingBox, Room);
                }
                return;
            }

            base.OnUpdate(gameTime);

            if (!AnimationPlayer.IsPlaying)
                Unparent();
        }

        #endregion
    }
}
