using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// ActorDeathState
    /// </summary>
    public sealed class ActorDeathState : ActorAnimatedState
    {
        // Constructor
        public ActorDeathState()
            : base(AnimationNames.Death, false)
        {
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!Owner.AnimationPlayer.IsPlaying)
            {
                if (Owner.IsPlayer)
                    Owner.Session.AwaitRoutine(RoutineNames.GameOver);
                else
                    Machine.ChangeState<ActorStandState>();
            }
        }
    }
}
