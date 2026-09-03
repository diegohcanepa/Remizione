using System;

namespace Remizione
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
                CombatArchetypeName.KamikazeFlyer => KamikazeFlyer,
                CombatArchetypeName.Volatile => Volatile,
                //CombatArchetypeName.Berserk => Berserk,
                //CombatArchetypeName.Coward => Coward,
                _ => throw new ArgumentOutOfRangeException(nameof(archetypeName))
            };
        }

        // Harasser
        public static HarasserArchetype Harasser { get; } = new();

        // KamikazeFlyer
        public static KamikazeFlyerArchetype KamikazeFlyer { get; } = new();

        // Lurker
        public static LurkerArchetype Lurker { get; } = new();

        // Stalker
        public static StalkerArchetype Stalker { get; } = new();

        // Volatile
        public static VolatileArchetype Volatile { get; } = new();
    }
}