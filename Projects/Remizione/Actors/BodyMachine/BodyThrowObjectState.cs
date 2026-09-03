using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// ActorThrowObjectState
    /// </summary>
    public sealed class ActorThrowObjectState : BodyAnimatedState
    {
        private ThrownProp? thrownObject;

        // Constructor
        public ActorThrowObjectState()
            : base(AnimationNames.ThrowObject, false, true)
        {
        }

        // Prop
        public Prop? Prop { get; set; }

        // Target
        public GameThing? Target { get; set; }

        // Enter
        public override void Enter()
        {
            base.Enter();
            thrownObject = null;
        }

        // Exit
        public override void Exit()
        {
            base.Exit();
            Prop = null;
            Target = null;
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (Prop != null && Target != null)
            {
                if (thrownObject == null && Owner.AnimationPlayer.Frame?.IsTrigger == true)
                {
                    thrownObject = new ThrownProp(Owner, Prop);
                    thrownObject.Throw(Target);
                }
            }

            if (!Owner.AnimationPlayer.IsPlaying)
                Machine.ChangeState<BodyStandState>();
        }
    }
}