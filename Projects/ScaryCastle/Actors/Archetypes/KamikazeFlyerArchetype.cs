using Engendro;

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

        // Es un kamikaze: no huye jamás. Muere intentando matarte.
        public override Ratio FleeHPThreshold => 0.00f;
        public override Ratio FleeChance => 0.00f;

        public override CombatDecisionType GetDecisionType(CombatIntent intent)
        {
            // Forzamos que su ataque sea de tipo 'Charge' (Embestida/Picada).
            return CombatDecisionType.Charge;
        }
    }
}