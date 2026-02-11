using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// ActorAnimateState
    /// </summary>
    public sealed class ActorAnimateState : ActorState
    {
        // Constructor
        public ActorAnimateState()
            : base()
        {
        }

        // Preserve
        public bool Preserve { get; set; }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (!Preserve && !Owner.AnimationPlayer.IsPlaying)
                Machine.ChangeState<ActorStandState>();
        }
    }
}
