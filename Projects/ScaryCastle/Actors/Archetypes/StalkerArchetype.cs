using Engendro;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// StalkerArchetype
    /// Para enemigos lentos, pesados y de mirada fija.
    /// No huye, no duda, solo avanza y ataca con mayor frecuencia.
    /// </summary>
    public sealed class StalkerArchetype : CombatArchetype
    {
        // Más agresivo que la rata. El 45% de las veces intentará atacarte.
        // Al ser un ojo gigante, su "ataque" es su único propósito.
        public override Ratio AttackChance => .45f;

        // CRÍTICA: Un ojo místico/monstruoso no tiene miedo. 
        // Ponemos el umbral de huida en 0 porque no retrocede nunca.
        public override Ratio FleeHPThreshold => 0.00f;
        public override Ratio FleeChance => 0.00f;

        // A diferencia del Lurker que "merodea", este siempre se mueve hacia vos.
        public override CombatDecisionType IdleMoveType => CombatDecisionType.Move;

        /// <summary>
        /// Cooldowns más cortos y consistentes.
        /// Esto hace que el movimiento del ojo se sienta más fluido y "teledirigido"
        /// que el de la rata, que es más espasmódico.
        /// </summary>
        public override int GetNextCooldown()
        {
            // Entre 0.8 y 1.5 segundos. Reacciona rápido a tus movimientos.
            return Random.Shared.Next(2500, 3500);
        }

        /// <summary>
        /// Opcional: Podrías definir que el Ojo no "salta" (Charge) 
        /// sino que simplemente te "pisa" (Attack) con un impacto pesado.
        /// </summary>
        public override CombatDecisionType GetDecisionType(CombatIntent intent)
        {
            // Si quieres que el ojo sea una masa pesada que no salta,
            // podrías forzar 'Attack' en lugar de 'Charge' incluso para contacto.
            return CombatDecisionType.Attack;
        }

        // MinComfortDistance
        public override float MinComfortDistance => 0;

        // MaxComfortDistance
        public override float MaxComfortDistance => 20;
    }
}