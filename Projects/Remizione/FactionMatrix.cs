namespace Remizione
{
    /// <summary>
    /// FactionMatrix
    /// </summary>
    public static class FactionMatrix
    {
        // IsHostile
        public static bool IsHostile(GameThing attacker, GameThing target)
        {
            if (attacker is not Actor attackerActor)
                return false;

            if (target is not Actor targetActor)
                return false;

            if (attacker == target || target.IsDead)
                return false;

            // Regla de los Penitentes: son rivales entre sí por botín y supervivencia
            if (attackerActor.Faction == Faction.Penitent && targetActor.Faction == Faction.Penitent)
                return true;

            return (attackerActor.Faction, targetActor.Faction) switch
            {
                // Monstruos atacan a todos los vivos
                (Faction.Creature, Faction.Player) => true,
                (Faction.Creature, Faction.Penitent) => true,

                // Penitentes hostiles atacan al jugador y a los monstruos
                (Faction.Penitent, Faction.Player) => true,
                (Faction.Penitent, Faction.Creature) => true,

                // El jugador considera hostiles a monstruos y penitentes
                (Faction.Player, Faction.Creature) => true,
                (Faction.Player, Faction.Penitent) => true,

                // Todo lo demás (incluidos Neutrales) no genera hostilidad por defecto
                _ => false
            };
        }
    }
}