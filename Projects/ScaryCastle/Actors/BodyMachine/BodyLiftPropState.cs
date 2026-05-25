using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// BodyLiftPropState
    /// </summary>
    public sealed class BodyLiftPropState : BodyAnimatedState
    {
        private bool eventDone;

        // Constructor
        public BodyLiftPropState()
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
            if (!eventDone && Target?.IsLiftable == true && Owner.AnimationPlayer.Frame?.IsTrigger == true)
            {
                eventDone = true;
                Owner.PlaySound(SoundNames.Gesture1);
                Owner.ActiveThrowable = Target;
                return;
            }

            if (!Owner.AnimationPlayer.IsPlaying)
                Machine.ChangeState<BodyStandState>();
        }
    }
}