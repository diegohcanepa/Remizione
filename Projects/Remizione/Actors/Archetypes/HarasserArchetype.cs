using Engendro;

namespace Remizione
{
    /// <summary>
    /// HarasserArchetype
    /// Arquetipo para enemigos oportunistas como la rata.
    /// Merodea cerca del jugador y ataca con frecuecia.
    /// </summary>
    public sealed class HarasserArchetype : CombatArchetype
    {
        // AttackChance
        public override Ratio AttackChance => .9f;

        // FleeHPThreshold
        public override Ratio FleeHPThreshold => 0.08f;

        // FleeChance
        public override Ratio FleeChance => 0.15f;

        // MeleeRange
        public override int MeleeRange => 30;
    }
}