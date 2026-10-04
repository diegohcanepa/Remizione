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

            float duration = Owner.CombatBehavior?.Archetype.CooldownDuration ?? 1.5f;
            timer = duration;
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Términó el descanso, forzamos un Idle para que escanee inmediatamente
            if (timer <= 0f)
                Machine.ChangeState<CombatIdleState>();
        }
    }
}