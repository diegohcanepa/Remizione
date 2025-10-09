namespace Remizione
{
    /// <summary>
    /// Creature
    /// </summary>
    public class Creature : Actor
    {
        // Constructor
        public Creature(GameSession session, string name)
            : base(session, name)
        {
            Faction = Faction.Enemy;
            AllowInteraction = false;
            ContactDamage = true;
            PowerBonus = 1;
            PlacementPhase = PlacementPhase.Creature;
        }
    }
}
