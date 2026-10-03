using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// BodyPlaceHeldPropState
    /// </summary>
    public sealed class BodyPlaceHeldPropState : BodyAnimatedState
    {
        private bool eventDone;

        // Constructor
        public BodyPlaceHeldPropState()
            : base(AnimationNames.Place, false)
        {
        }

        // Enter
        public override void Enter()
        {
            base.Enter();
            Owner.PlaySound(SoundNames.PenitentEffortRelease);
            eventDone = false;
        }

        // Target
        public Prop? Target { get; set; }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (!eventDone && Owner.AnimationPlayer.Frame?.IsTrigger == true)
            {
                eventDone = true;
                return;
            }

            if (!Owner.AnimationPlayer.IsPlaying)
                Machine.ChangeState<BodyStandState>();
        }
    }
}