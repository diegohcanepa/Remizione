using System;

namespace Remizione
{
    /// <summary>
    /// Archetypes
    /// </summary>
    public static class Archetypes
    {
        // Get
        public static CombatArchetype Get(CombatArchetypeName archetypeName)
        {
            return archetypeName switch
            {
                CombatArchetypeName.Harasser => Harasser,
                CombatArchetypeName.Lurker => Lurker,
                _ => throw new ArgumentOutOfRangeException(nameof(archetypeName))
            };
        }

        // Harasser
        public static HarasserArchetype Harasser { get; } = new();

        // Lurker
        public static LurkerArchetype Lurker { get; } = new();
    }
}