using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// PlayerCardResolutionState
    /// </summary>
    public sealed class PlayerCardResolutionState(Arena arena) : ArenaState(arena)
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
                if (Arena.PlayerCard is Card card)
                    Arena.Session.Deck.Discard(card);
                handDiscarded = true;
            }

            // Apply damage
            if (!damageDealt && TimeInState > 1.5f)
            {
                Arena.HitEnemy();
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
                    Arena.TransitionTo(new EnemyCardResolutionState(Arena));
                }
            }
        }
    }
}
