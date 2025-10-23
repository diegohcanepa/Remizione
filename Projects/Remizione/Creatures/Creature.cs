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
            AllowInteraction = false;
            ContactDamageType = DamageType.Physical;
            Faction = Faction.Evil;
            PowerBonus = 1;
            PlacementPhase = PlacementPhase.Creature;
        }
    }
}
