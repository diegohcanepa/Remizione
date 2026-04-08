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
        // Solo el 22% de las veces que el Brain procesa, decidirá morder.
        // Esto genera esa sensación de que "anda por ahí caminando" antes de saltar.
        public override Ratio AttackChance => 0.22f;

        // Las ratas en este diseño son persistentes: solo huyen si están al borde de la muerte.
        public override Ratio FleeHPThreshold => 0.08f;
        public override Ratio FleeChance => 0.15f;

        // El Lurker usa 'Move' para merodear cuando no está atacando.
        // (Tu sistema de movimiento debería manejar el "rodear" al jugador aquí).
        public override CombatDecisionType IdleMoveType => CombatDecisionType.Move;

        /// <summary>
        /// Devuelve un cooldown errático. 
        /// Las pausas largas simulan al animal "observando" o "dudando".
        /// </summary>
        public override int GetNextCooldown()
        {
            // Entre 1.2 y 3.5 segundos entre decisiones.
            return Random.Shared.Next(1200, 3500);
        }
    }
}