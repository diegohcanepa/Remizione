using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// SaintPeregrine
    /// </summary>
    public sealed class SaintPeregrine : IsometricProp
    {
        private readonly ImageSprite eyes;

        // Constructor
        public SaintPeregrine(GameSession session, string name)
            : base(session, name)
        {
            this.HitEffect = HitEffect.Shake;
            this.HitTestPolygon = TestPolygon.Hotspot;
            PropState = PropState.Locked;

            this.eyes = new ImageSprite(Game, Atlas?.GetImage($"{StaticName}Eyes"))
            {
            };

            eyes.Tweens.OpacityTween = FloatTween.Create(TweenStyle.Linear, 1, .7f, 70, -1);
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            if (PropState == PropState.Unlocked)
                eyes.Draw(gameTime);
        }

        // OnPropStateChanged
        protected override void OnPropStateChanged()
        {
            AnimationPlayer.Play(PropState == PropState.Locked ? AnimationNames.Locked : AnimationNames.Unlocked, false);

            AllowInteraction = PropState == PropState.Locked;

            if (LoadState != LoadState.Loaded)
                return;

            if (PropState == PropState.Unlocked && Room is RideRoom rideRoom)
            {
                if (rideRoom.LeftTower != null)
                {
                    if (rideRoom.RoomPhase != RunPhase.Start)
                        rideRoom.LeftTower.Collider = new Polygon("45,0;45,46;35,51;12,43;7,44;29,52;16,56;0,49;0,0");

                    rideRoom.LeftTower.PropState = PropState.Open;
                    rideRoom.LeftTower.AnimationPlayer.Play(AnimationNames.Opening, false);
                }

                if (rideRoom.RightConnector != null)
                {
                    rideRoom.RightConnector.PropState = PropState.Open;
                    if (rideRoom.RoomPhase != RunPhase.End)
                    {
                        rideRoom.RightConnector.Collider = new Polygon("0,0;0,46;11,49;24,42;29,43;17,51;28,56;43,54;51,46;45,0");
                        PlaySound(SoundNames.TowerDoorClose);
                    }
                    rideRoom.RightConnector.AnimationPlayer.Play(AnimationNames.Opening, false);
                    Sound.Play(SoundNames.SaintPeregrine);
                }
            }
        }

        // OnTransform
        protected override void OnTransform(TransformChange change)
        {
            base.OnTransform(change);
            eyes?.MatchTransform(Sprite);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            eyes.Update(gameTime);
        }

        #endregion
    }
}
