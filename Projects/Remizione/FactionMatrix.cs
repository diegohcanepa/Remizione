namespace Remizione
{
    /// <summary>
    /// FactionMatrix
    /// </summary>
    public static class FactionMatrix
    {
        public static bool IsHostile(Actor attacker, Actor target)
        {
            if (attacker == target || target.IsDead)
                return false;

            // Regla de los Penitentes: son rivales entre sí por botín y supervivencia
            if (attacker.Faction == Faction.Penitent && target.Faction == Faction.Penitent)
                return true;

            return (attacker.Faction, target.Faction) switch
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