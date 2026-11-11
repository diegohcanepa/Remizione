using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// CombatHurtState
    /// </summary>
    public sealed class CombatHurtState : State<Actor>
    {
        private float stunTimer;
        private const float DefaultStunDuration = 0.4f; // 400ms de stun/reacción al golpe

        // Enter
        public override void Enter()
        {
            // 1. Cancelamos cualquier movimiento o acción que estuviera ejecutando
            Owner.StopMoving();
            Owner.IsDealingContactDamage = false;

            // 2. Seteamos la duración del stun
            stunTimer = DefaultStunDuration;

            // Optional: Owner.PlayAnimation(AnimationNames.Hurt);
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            stunTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Al recuperarse del golpe, pasa a Cooldown para darle aire al jugador
            if (stunTimer <= 0f)
                Machine.ChangeState<CombatCooldownState>();
        }
    }
}