using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// BodyFatigueState
    /// </summary>
    public sealed class BodyFatigueState : BodyAnimatedState
    {
        private int timer;

        // Constructor
        public BodyFatigueState()
            : base(AnimationNames.Fatigue, true)
        {
        }

        // Enter
        public override void Enter()
        {
            base.Enter();
            timer = 2000;
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            timer -= gameTime.ElapsedGameTime.Milliseconds;
            if (timer <= 0)
                Machine.ChangeState<BodyStandState>();
        }
    }
}
