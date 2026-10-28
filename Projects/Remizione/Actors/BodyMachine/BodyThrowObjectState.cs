using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ActorThrowObjectState
    /// </summary>
    public sealed class ActorThrowObjectState : BodyAnimatedState
    {
        private Vector2? targetPos;
        private ThrownProp? thrownObject;

        // Constructor
        public ActorThrowObjectState()
            : base(AnimationNames.ThrowObject, false, true)
        {
        }

        // Prop
        public Prop? Prop { get; set; }

        // Target
        public GameThing? Target
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    targetPos = field?.Position;
                }
            }
        }

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
            if (Prop != null && Target != null && targetPos != null)
            {
                if (thrownObject == null && Owner.AnimationPlayer.Frame?.IsTrigger == true)
                {
                    thrownObject = new ThrownProp(Owner, Prop);
                    thrownObject.Throw(targetPos.Value);
                }
            }

            if (!Owner.AnimationPlayer.IsPlaying)
                Machine.ChangeState<BodyStandState>();
        }
    }
}