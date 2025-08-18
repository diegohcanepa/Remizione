using Adberration;
using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// OcculusMinionPatrolState
    /// </summary>
    internal class OcculusMinionPatrolState : AIState
    {
        private int cooldown;

        // Constructor
        public OcculusMinionPatrolState(AIStateMachine stateMachine)
            : base(stateMachine, AIStateName.Patrol)
        {
        }

        // Move
        private void Move()
        {
            cooldown = 0;

            var distance = Randomizer.Next(50, 100);

            if (Owner.Direction == FacingDirection.Right)
                Owner.MoveTo(Owner.Position - new Vector2(distance, 0));
            else
                Owner.MoveTo(Owner.Position + new Vector2(distance, 0));
        }

        // Enter
        public override void Enter()
        {
            Move();
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (cooldown > 0)
            {
                cooldown -= gameTime.ElapsedGameTime.Milliseconds;

                if (cooldown <= 0)
                    Move();

                return;
            }

            if (!Owner.IsMoving)
                cooldown = 1000;
        }
    }
}
