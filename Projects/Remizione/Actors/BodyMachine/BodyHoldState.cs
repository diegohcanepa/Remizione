using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// BodyHoldState
    /// </summary>
    public sealed class BodyHoldState : BodyAnimatedState
    {
        private bool eventDone;

        // Constructor
        public BodyHoldState()
            : base(AnimationNames.PickUp, false)
        {
        }

        // Enter
        public override void Enter()
        {
            base.Enter();
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
                Owner.PlaySound(SoundNames.PenitentEffort);
                Owner.HeldProp = Target;
                return;
            }

            if (!Owner.AnimationPlayer.IsPlaying)
                Machine.ChangeState<BodyStandState>();
        }
    }
}