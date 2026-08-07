using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// BodyFatigueState
    /// </summary>
    public sealed class BodyFatigueState : BodyAnimatedState
    {
        private const int cooldown = 400;
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
            timer = cooldown;
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            timer -= gameTime.ElapsedGameTime.Milliseconds;
            if (timer <= 0)
            {
                Owner.Stamina++;
                if (Owner.Stamina >= Owner.MaxStamina / 2)
                    Machine.ChangeState<BodyStandState>();
                else
                    timer = cooldown;
            }
        }
    }
}
