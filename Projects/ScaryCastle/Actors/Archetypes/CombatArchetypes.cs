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
                CombatArchetypeName.Lurker => Lurker,
                //CombatArchetypeName.Berserk => Berserk,
                //CombatArchetypeName.Coward => Coward,
                _ => throw new ArgumentOutOfRangeException(nameof(archetypeName))
            };
        }

        // Lurker
        public static CombatArchetype Lurker { get; } = new LurkerArchetype();
    }
}