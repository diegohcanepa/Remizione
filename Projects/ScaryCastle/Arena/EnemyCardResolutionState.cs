using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// EnemyCardResolutionState
    /// </summary>
    public sealed class EnemyCardResolutionState(Arena arena) : ArenaState(arena)
    {
        private bool damageDealt;
        private bool handDiscarded;

        // Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Discard card
            if (!handDiscarded && TimeInState > .5f)
            {
                if (Arena.EnemyCard is Card card)
                    card.MoveTo(card.Position - new Vector2(150, 0), 300, 0, false);
                handDiscarded = true;
            }

            // Apply damage
            if (!damageDealt && TimeInState > 1.5f)
            {
                Arena.HitPlayer();
                damageDealt = true;
            }

            // Victory / Loss
            if (TimeInState > 3.0f)
            {
                if (Arena.Player.HP <= 0)
                {
                    Arena.TransitionTo(new DefeatState(Arena));
                }
                else if (Arena.Enemy.HP <= 0)
                {
                    Arena.TransitionTo(new VictoryState(Arena));
                }
                else
                {
                    // Ciclo continua: Turno Enemigo de nuevo
                    Arena.TransitionTo(new EnemyTurnState(Arena));
                }
            }
        }
    }
}
