using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// BodyDeathState
    /// </summary>
    public sealed class BodyDeathState : BodyAnimatedState
    {
        // Constructor
        public BodyDeathState()
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
                    Owner.Session.AwaitRoutine(RoutineNames.DeathByHealth);
                else
                    Machine.ChangeState<BodyStandState>();
            }
        }
    }
}
