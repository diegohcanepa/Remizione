using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// ActorThrowObjectState
    /// </summary>
    public sealed class ActorThrowObjectState : BodyAnimatedState
    {
        private bool objectThrown;

        // Constructor
        public ActorThrowObjectState()
            : base("ThrowObject", false, true)
        {
        }

        // Enter
        public override void Enter()
        {
            base.Enter();
            objectThrown = false;
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (!objectThrown && Owner.AnimationPlayer.Frame?.IsEvent == true)
            {
                if (Owner.Session.ObjectPools.FindThrownObject("Duck") is ThrownObject throwable)
                {
                    //if (Owner.WhooshSound != null)
                        //  Owner.PlaySound(Owner.WhooshSound);

                    throwable.Launch(Owner);
                }

                objectThrown = true;
            }

            if (!Owner.AnimationPlayer.IsPlaying)
                Machine.ChangeState<BodyStandState>();
        }
    }
}