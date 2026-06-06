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

        public override int GetPatienceTolerance()
        {
            // Cooldown bajísimo e imperceptible (100 a 200ms).
            // Esto hace que el Brain revalúe constantemente y nunca haga pausas raras.
            return Random.Shared.Next(100, 200);
        }
    }
}