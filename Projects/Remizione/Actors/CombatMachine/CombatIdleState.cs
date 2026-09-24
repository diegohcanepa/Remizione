using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    public class CombatIdleState : State<Actor>
    {
        public override void Update(GameTime gameTime)
        {
            var target = Owner.Session.Player;
            if (target == null || Owner.CombatBehavior?.Archetype is not { } arch)
                return;

            // 1. Verificación barata primero: ¿está dentro del radio de alerta?
            if (Owner.DistanceToTarget(target) <= arch.AwarenessRange)
            {
                // 2. Verificación cara después: ¿hay línea de visión limpia sin obstáculos?
                if (Owner.HasLineOfSightTo(target))
                {
                    Owner.IsHostile = true;
                    Machine.ChangeState<CombatStepState>();
                }
            }
        }
    }
}