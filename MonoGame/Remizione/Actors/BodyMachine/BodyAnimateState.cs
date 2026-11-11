using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// BodyAnimateState
    /// </summary>
    public sealed class BodyAnimateState : BodyState
    {
        // Constructor
        public BodyAnimateState()
            : base()
        {
        }

        // Preserve
        public bool Preserve { get; set; }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (!Preserve && !Owner.AnimationPlayer.IsPlaying)
                Machine.ChangeState<BodyStandState>();
        }
    }
}
