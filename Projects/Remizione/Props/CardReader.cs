using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// CardReader
    /// </summary>
    public class CardReader : IsometricProp
    {
        private readonly ImageSprite lightLayer;

        // Constructor
        public CardReader(GameSession session, string name)
            : base(session, name)
        {
            this.lightLayer = new(Game);

            this.Light = new Light(Game, "")
            {
                PivotOrigin = RectanglePoint.Center
            };

            HighlightInteraction = false;
            HitEffect = HitEffect.Shake;
            PropState = PropState.Locked;
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);
            lightLayer.Draw(gameTime);
        }

        // OnPropStateChanged
        protected override void OnPropStateChanged()
        {
            AnimationPlayer.Play(PropState == PropState.Locked ? AnimationNames.Locked : AnimationNames.Unlocked, false);

            if (PropState == PropState.Locked)
            {
                lightLayer.Image = Atlas?.GetImage($"{StaticName}LightRed");
                if (Light != null)
                    Light.Color = Color.Red;
            }
            else
            {
                lightLayer.Image = Atlas?.GetImage($"{StaticName}LightGreen");
                if (Light != null)
                    Light.Color = Color.Green;
            }

            if (LoadState != LoadState.Loaded)
                return;

            if (PropState == PropState.Unlocked && Room is RideRoom rideRoom && rideRoom.RightTower != null)
            {
                if (rideRoom.RoomPosition != RoomPosition.Last)
                    rideRoom.RightTower.Collider = new Polygon("0,0;0,46;10,51;33,43;38,44;18,54;29,56;44,49;45,0");

                rideRoom.RightTower.PropState = PropState.Open;
                rideRoom.RightTower.AnimationPlayer.Play(AnimationNames.Opening, false);
                PlaySound(SoundNames.TowerDoorClose);
            }
        }

        // OnTransform
        protected override void OnTransform(TransformChange change)
        {
            base.OnTransform(change);
            lightLayer?.MatchTransform(Sprite);
            if (Light != null)
                Light.Position = BoundingBox.GetPoint(RectanglePoint.LeftTop, 6, 23);
        }

        #endregion
    }
}
