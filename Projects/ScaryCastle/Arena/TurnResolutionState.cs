using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// TurnResolutionState
    /// </summary>
    public sealed class TurnResolutionState(Arena arena) : ArenaState(arena)
    {
        private bool _handDiscarded = false;
        private bool _damageDealt = false;

        // Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Paso A: Descartar mano visualmente (al inicio)
            if (!_handDiscarded && TimeInState > 0.5f)
            {
                Arena.Session.Deck.DiscardHand();
                _handDiscarded = true;
            }

            // Paso B: Aplicar efecto carta enemiga (Pausa dramática)
            if (!_damageDealt && TimeInState > 2.0f)
            {
                ApplyEnemyEffect();
                _damageDealt = true;
            }

            // Paso C: Comprobar victoria/derrota (Al final)
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

        private void ApplyEnemyEffect()
        {
            if (Arena.EnemyCard != null)
            {
                Arena.EnemyCard.Definition.Apply(Arena.Enemy, Arena.Player, false);
                Arena.EnemyCard = null; // Consumimos la carta
                                        // Aquí irían efectos de sonido de golpe, screenshake, etc.
            }
        }
    }
}
