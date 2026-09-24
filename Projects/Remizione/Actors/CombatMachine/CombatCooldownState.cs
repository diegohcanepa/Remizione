using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// CombatCooldownState
    /// </summary>
    public sealed class CombatCooldownState : State<Actor>
    {
        private float timer;

        // Enter
        public override void Enter()
        {
            Owner.StopMoving();

            // Leemos los segundos de pausa del arquetipo (ej: 1.5s)
            float duration = Owner.CombatBehavior?.Archetype.CooldownDuration ?? 1.5f;
            timer = duration;
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Término el descanso, vuelve a evaluar la persecución
            if (timer <= 0f)
                Machine.ChangeState<CombatStepState>();
        }
    }
}
