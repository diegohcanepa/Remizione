using Engendro;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// LurkerArchetype
    /// Arquetipo para enemigos oportunistas como la rata.
    /// Prefiere merodear cerca del jugador y atacar esporádicamente.
    /// </summary>
    public sealed class LurkerArchetype : CombatArchetype
    {
        // AllowRandomMove
        public override bool AllowRandomMove => true;

        // Solo el 22% de las veces que el Brain procesa, decidirá morder.
        // Esto genera esa sensación de que "anda por ahí caminando" antes de saltar.
        public override Ratio AttackChance => .22f;

        // ConsiderContactAsIntent
        public override bool ConsiderContactAsIntent => true;

        // FleeHPThreshold
        public override Ratio FleeHPThreshold => 0.08f;

        // FleeChance
        public override Ratio FleeChance => 0.15f;

        public override int GetPatienceTolerance()
        {
            return Random.Shared.Next(2, 5);
        }

        public override int MoveRange => 50;
    }
}