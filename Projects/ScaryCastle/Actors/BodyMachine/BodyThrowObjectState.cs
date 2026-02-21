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

        // CombatIntent
        public CombatIntent? CombatIntent { get; set; }

        // Enter
        public override void Enter()
        {
            base.Enter();
            objectThrown = false;
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (CombatIntent != null)
            {
                if (!objectThrown && Owner.AnimationPlayer.Frame?.IsEvent == true)
                {
                    Owner.Session.ObjectPools.GetThrownObject(CombatIntent.ThrownObject)?.Launch(Owner, CombatIntent);
                    objectThrown = true;
                }
            }

            if (!Owner.AnimationPlayer.IsPlaying)
                Machine.ChangeState<BodyStandState>();
        }
    }
}