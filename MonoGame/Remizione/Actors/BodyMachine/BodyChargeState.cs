using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// BodyChargeState
    /// </summary>
    public sealed class BodyChargeState : BodyAnimatedState
    {
        // Constructor
        public BodyChargeState()
            : base(AnimationNames.Charge, false)
        {
        }

        // Enter
        public override void Enter()
        {
            base.Enter();
            Owner.MoveTo(Destination);
        }

        // Target
        public Vector2 Destination { get; set; }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (!Owner.IsMoving)
                Machine.ChangeState<BodyStandState>();
        }
    }
}