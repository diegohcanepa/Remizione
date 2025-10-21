using Engendro;
using Engendro.Audio;

namespace Remizione
{
    /// <summary>
    /// SaintPeregrine
    /// </summary>
    public sealed class SaintPeregrine : IsometricProp
    {
        // Constructor
        public SaintPeregrine(GameSession session, string name)
            : base(session, name)
        {
            this.HitEffect = HitEffect.Shake;
            this.HitTestPolygon = TestPolygon.Hotspot;
            PropState = PropState.Locked;
        }

        #region Protected members

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

                    if (rideRoom.RoomPhase == RunPhase.End)
                    {
                        Sound.Play(SoundNames.SaintPeregrineFreedom);
                    }
                    else
                    {
                        rideRoom.RightConnector.Collider = new Polygon("0,0;0,46;11,49;24,42;29,43;17,51;28,56;43,54;51,46;45,0");
                        rideRoom.RightConnector.AnimationPlayer.Play(AnimationNames.Opening, false);
                        Sound.Play(SoundNames.SaintPeregrine);
                    }
                }

                PlaySound(SoundNames.TowerDoorClose);
            }
        }

        #endregion
    }
}
