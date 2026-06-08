using Engendro;
using System;

namespace ScaryCastle
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

        // GetIntentWeight
        public override float GetIntentWeight(CombatIntent intent, Actor actor, float distance)
        {
            return .8f;
        }

        // GetPatienceTolerance
        public override int GetPatienceTolerance()
        {
            // Entre 1.2 y 3.5 segundos entre decisiones.
            return Random.Shared.Next(1200, 3500);
        }
    }
}