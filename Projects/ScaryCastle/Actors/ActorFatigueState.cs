using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// ActorFatigueState
    /// </summary>
    public sealed class ActorFatigueState : ActorAnimatedState
    {
        // Constructor
        public ActorFatigueState()
            : base(AnimationNames.Fatigue, true)
        {
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Owner.Session.Will == 100)
                Machine.ChangeState<ActorStandState>();
        }
    }
}
