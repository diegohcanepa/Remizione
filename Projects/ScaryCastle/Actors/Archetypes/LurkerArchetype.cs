using Engendro;

namespace ScaryCastle
{
    /// <summary>
    /// LurkerArchetype
    /// Arquetipo para enemigos oportunistas como la rata.
    /// Prefiere merodear cerca del jugador y atacar esporádicamente.
    /// </summary>
    public sealed class LurkerArchetype : CombatArchetype
    {
        // Solo el 22% de las veces que el Brain procesa, decidirá morder.
        // Esto genera esa sensación de que "anda por ahí caminando" antes de saltar.
        public override Ratio AttackChance => .22f;

        // FallbackMovement
        public override FallbackMovementKind FallbackMovement => FallbackMovementKind.Lurk;

        // FleeHPThreshold
        public override Ratio FleeHPThreshold => 0.08f;

        // FleeChance
        public override Ratio FleeChance => 0.15f;

        // MeleeRange
        public override int MeleeRange => 30;
    }
}