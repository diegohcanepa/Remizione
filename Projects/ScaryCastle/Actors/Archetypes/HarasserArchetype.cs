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

        // ConsiderContactAsIntent
        public override bool ConsiderContactAsIntent => true;

        // FleeHPThreshold
        public override Ratio FleeHPThreshold => 0.08f;

        // FleeChance
        public override Ratio FleeChance => 0.15f;

        // GetIntentWeight
        public override float GetIntentWeight(CombatIntent intent, Actor actor, float distance)
        {
            return .8f;
        }

        // IdleMoveType
        public override CombatDecisionType IdleMoveType => CombatDecisionType.Move;

        // GetNextCooldown
        public override int GetNextCooldown()
        {
            // Entre 1.2 y 3.5 segundos entre decisiones.
            return Random.Shared.Next(1200, 3500);
        }

        // MinComfortDistance
        public override float MinComfortDistance => 60;

        // MaxComfortDistance
        public override float MaxComfortDistance => 110;
    }
}