using Engendro;

namespace Remizione
{
    /// <summary>
    /// VolatileArchetype
    /// Enemigo fanático que camina sin frenar directo al jugador para inmolarse.
    /// </summary>
    public sealed class VolatileArchetype : CombatArchetype
    {
        // 100% de agresividad. Si está en rango, ataca (explota) de una.
        public override Ratio AttackChance => 1;

        // No tiene instinto de preservación.
        public override Ratio FleeHPThreshold => 0;
        public override Ratio FleeChance => 0;
    }
}