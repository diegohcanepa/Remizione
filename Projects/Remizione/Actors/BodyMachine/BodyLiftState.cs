using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// BodyLiftState
    /// </summary>
    public sealed class BodyLiftState : BodyAnimatedState
    {
        private bool eventDone;

        // Constructor
        public BodyLiftState()
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
                //Owner.PlaySound(SoundNames.Gesture1);
                Owner.ActiveThrowable = Target;
                return;
            }

            if (!Owner.AnimationPlayer.IsPlaying)
                Machine.ChangeState<BodyStandState>();
        }
    }
}