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
            : base("ThrowObject", false, true)
        {
        }

        // Prop
        public Prop? Prop { get; set; }

        // Enter
        public override void Enter()
        {
            base.Enter();
            thrownObject = null;
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (Prop != null)
            {
                if (thrownObject == null && Owner.AnimationPlayer.Frame?.IsTrigger == true)
                {
                    thrownObject = new ThrownProp(Owner, Prop);
                    thrownObject.Throw();
                }
            }

            if (!Owner.AnimationPlayer.IsPlaying)
                Machine.ChangeState<BodyStandState>();
        }
    }
}