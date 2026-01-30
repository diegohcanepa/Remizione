using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// EnemyTurnState
    /// </summary>
    public sealed class EnemyTurnState(Arena arena) : ArenaState(arena)
    {
        // Enter
        public override void Enter()
        {
            base.Enter();
            Arena.PlayEnemyCard();
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (TimeInState > 1.5f)
                Arena.TransitionTo(new DrawHandState(Arena));
        }
    }
}
