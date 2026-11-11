using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// BodyFatigueState
    /// </summary>
    public sealed class BodyFatigueState : BodyAnimatedState
    {
        // Constructor
        public BodyFatigueState()
            : base(AnimationNames.Fatigue, false)
        {
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (!Owner.AnimationPlayer.IsPlaying)
                Machine.ChangeState<BodyStandState>();
        }
    }
}
