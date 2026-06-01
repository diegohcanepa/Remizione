using Engendro;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// VolatileArchetype
    /// Enemigo fanático que camina sin frenar directo al jugador para inmolarse.
    /// </summary>
    public sealed class VolatileArchetype : CombatArchetype
    {
        // 100% de agresividad. Si está en rango, ataca (explota) de una.
        public override Ratio AttackChance => 1;

        // Su ataque es pura colisión física.
        public override bool ConsiderContactAsIntent => true;

        // No tiene instinto de preservación.
        public override Ratio FleeHPThreshold => 0;
        public override Ratio FleeChance => 0;

        // Si no está explotando, camina hacia el jugador de forma constante.
        public override CombatDecisionType IdleMoveType => CombatDecisionType.Move;

        // No le interesa mantener distancia social. Quiere estar encima tuyo (0px).
        public override float MinComfortDistance => 0;
        public override float MaxComfortDistance => 10;

        // Camina un poco más lento que los demás para darle tiempo al jugador 
        // de reventarlo a distancia antes de que llegue.
        public override float MovementSpeedFactor => .85f;

        public override int GetNextCooldown()
        {
            // Cooldown bajísimo e imperceptible (100 a 200ms).
            // Esto hace que el Brain revalúe constantemente y nunca haga pausas raras.
            return Random.Shared.Next(100, 200);
        }
    }
}