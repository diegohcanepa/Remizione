using System;

namespace ScaryCastle
{
    /// <summary>
    /// Archetypes
    /// </summary>
    public static class Archetypes
    {
        // Berserk
        //public static CombatArchetype Berserk { get; } = new BerserkArchetype();

        // Coward
        //public static CombatArchetype Coward { get; } = new CowardArchetype();

        // Get
        public static CombatArchetype Get(CombatArchetypeName archetypeName)
        {
            return archetypeName switch
            {
                CombatArchetypeName.Harasser => Harasser,
                CombatArchetypeName.Lurker => Lurker,
                CombatArchetypeName.Stalker => Stalker,
                //CombatArchetypeName.Berserk => Berserk,
                //CombatArchetypeName.Coward => Coward,
                _ => throw new ArgumentOutOfRangeException(nameof(archetypeName))
            };
        }

        // Harasser
        public static HarasserArchetype Harasser { get; } = new();

        // Lurker
        public static LurkerArchetype Lurker { get; } = new();

        // Stalker
        public static StalkerArchetype Stalker { get; } = new();
    }
}