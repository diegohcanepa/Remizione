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
            this.HitEffect = HitEffect.Shake;
            this.HitTestPolygon = TestPolygon.Hotspot;
            this.lightLayer = new(Game);
            this.lightLayer.Tweens.OpacityTween = FloatTween.Create(TweenStyle.Linear, 1, .85f, 80, -1);

            this.AttachedLight = new Light(Game, "")
            {
                PivotOrigin = RectanglePoint.Center
            };

            HighlightInteraction = false;
            PropState = PropState.Locked;
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            lightLayer.Position += GetShakeOffset();
            lightLayer.Draw(gameTime);
            lightLayer.Position -= GetShakeOffset();
        }

        // OnPropStateChanged
        protected override void OnPropStateChanged()
        {
            AnimationPlayer.Play(PropState == PropState.Locked ? AnimationNames.Locked : AnimationNames.Unlocked, false);

            if (PropState == PropState.Locked)
            {
                lightLayer.Image = Atlas?.GetImage($"{StaticName}LightRed");
                if (AttachedLight != null)
                    AttachedLight.Color = Color.Red;
            }
            else
            {
                lightLayer.Image = Atlas?.GetImage($"{StaticName}LightGreen");
                if (AttachedLight != null)
                    AttachedLight.Color = Color.Green;
            }

            AllowInteraction = PropState == PropState.Locked;

            if (LoadState != LoadState.Loaded)
                return;

            if (PropState == PropState.Unlocked && Room is RideRoom rideRoom && rideRoom.RightTower != null)
            {
                if (rideRoom.RoomPhase != RunPhase.End)
                    rideRoom.RightTower.Collider = new Polygon("0,0;0,46;11,49;24,42;29,43;15,51;28,56;43,54;51,46;45,0");

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
            if (AttachedLight != null)
                AttachedLight.Position = BoundingBox.GetPoint(RectanglePoint.LeftTop, 6, 23);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            lightLayer.Update(gameTime);
        }

        #endregion
    }
}
