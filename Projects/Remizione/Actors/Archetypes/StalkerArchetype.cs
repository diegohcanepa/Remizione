using Engendro;

namespace Remizione
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

        // FleeChance
        public override Ratio FleeChance => 0.00f;

        // MeleeRange
        public override float MeleeRange => 40;
    }
}