using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ActorStandState
    /// </summary>
    public sealed class ActorStandState : ActorState
    {
        private int idleCooldown;

        // Constructor
        public ActorStandState(Actor owner)
            : base(owner, ActorStateNames.Stand, ActorStateSettings.LoopAnimation)
        {
        }

        // CheckTransitions
        public override string? CheckTransitions()
        {
            if (Owner.IsPlayer && idleCooldown < 0)
                return ActorStateNames.Idle;
            else
                return base.CheckTransitions();
        }

        // Enter
        public override void Enter()
        {
            base.Enter();
            idleCooldown = 15000;
            Owner.StopMoving();
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (idleCooldown >= 0)
                idleCooldown -= gameTime.ElapsedGameTime.Milliseconds;
        }
    }
}
