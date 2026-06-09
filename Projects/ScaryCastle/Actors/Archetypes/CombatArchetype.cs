using Engendro;
using System;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// CombatArchetype
    /// </summary>
    public abstract class CombatArchetype
    {
        // AllowRandomMove
        // Por defecto, los NPCs no se mueven aleatoriamente. Solo lo hacen si el arquetipo lo permite explícitamente.
        public virtual bool AllowRandomMove => false;

        // AttackChance
        // Probabilidad (0 a 1) de que el NPC intente un ataque en su turno.
        // Un valor bajo (0.2) crea un comportamiento de "acecho".
        public abstract Ratio AttackChance { get; }

        // FleeChance
        // Probabilidad de que efectivamente huya una vez herido.
        public abstract Ratio FleeChance { get; }

        // FleeHPThreshold
        // Umbral de vida (0 a 1) por debajo del cual el NPC considera huir.
        public abstract Ratio FleeHPThreshold { get; }

        // GetDecisionType
        public virtual CombatDecisionType GetDecisionType(CombatIntent intent)
        {
            return CombatDecisionType.Attack;
        }

        // GetIntentWeight
        // Calcula el peso específico de un intent sin descartar proyectiles por distancia.
        public virtual float GetIntentWeight(CombatIntent intent, Actor actor, float distance)
        {
            // 1. Si es a distancia, en rooms chicas siempre tiene peso válido.
            if (intent.ActionKind == ActionKind.Projectile)
            {
                return intent.SpawnWeight;
            }

            // 2. Si es cuerpo a cuerpo, solo tiene peso si el jugador entró en el rango operativo de Melee.
            if (intent.ActionKind == ActionKind.Proximity)
            {
                return distance > MeleeAttackRange ? 0 : intent.SpawnWeight;
            }

            return 0;
        }

        // MeleeAttackRange
        public virtual int MeleeAttackRange => 20;

        // SelectIntent
        // Selecciona un ataque de la lista disponible basándose en pesos y distancia.
        public virtual CombatIntent? SelectIntent(Actor actor, IList<CombatIntent> intents, float distance)
        {
            if (intents == null || intents.Count == 0)
                return null;

            // Usamos stackalloc para evitar asignaciones en el Heap (GC Friendly)
            Span<float> weights = stackalloc float[intents.Count];
            float totalWeight = 0;
            bool anyInRange = false;

            for (int i = 0; i < intents.Count; i++)
            {
                var intent = intents[i];
                float weight = GetIntentWeight(intent, actor, distance);

                if (weight > 0)
                    anyInRange = true;

                weights[i] = weight;
                totalWeight += weight;
            }

            // Si nada está en rango o todos los pesos son 0
            if (!anyInRange || totalWeight <= 0)
                return null;

            // Selección Aleatoria Ponderada
            float roll = (float)Random.Shared.NextDouble() * totalWeight;
            float cumulative = 0;

            for (int i = 0; i < intents.Count; i++)
            {
                cumulative += weights[i];
                if (roll <= cumulative) return intents[i];
            }

            return null;
        }
    }
}