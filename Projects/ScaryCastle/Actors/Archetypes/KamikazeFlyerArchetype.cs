using Engendro;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// KamikazeFlyerArchetype
    /// Arquetipo para enemigos voladores que se posicionan sobre el objetivo
    /// y se lanzan en picada vertical.
    /// </summary>
    public sealed class KamikazeFlyerArchetype : CombatArchetype
    {
        // 35% de chance de iniciar la picada en cada tick del Brain si está en posición.
        public override Ratio AttackChance => .35f;

        // El impacto es por contacto directo (físico).
        public override bool ConsiderContactAsIntent => true;

        // Es un kamikaze: no huye jamás. Muere intentando matarte.
        public override Ratio FleeHPThreshold => 0.00f;
        public override Ratio FleeChance => 0.00f;

        // Si no está atacando, su decisión es 'Move' (moverse al "techo" sobre el jugador).
        public override CombatDecisionType IdleMoveType => CombatDecisionType.Move;

        // Distancia horizontal mínima y máxima que tolera. 
        // Queremos que esté prácticamente alineado en el eje X con el jugador (0 a 30px).
        public override float MinComfortDistance => 0f;
        public override float MaxComfortDistance => 30f;

        // Vuela un 20% más rápido que los enemigos terrestres para reubicarse arriba tuyo.
        public override float MovementSpeedFactor => 1.2f;

        public override int GetNextCooldown()
        {
            // Decisiones rápidas (0.5 a 1.2 segundos) para reposicionarse agresivamente.
            return Random.Shared.Next(500, 1200);
        }

        public override CombatDecisionType GetDecisionType(CombatIntent intent)
        {
            // Forzamos que su ataque sea de tipo 'Charge' (Embestida/Picada).
            return CombatDecisionType.Charge;
        }
    }
}