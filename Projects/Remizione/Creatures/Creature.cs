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
            Affinity = Affinity.Evil;
            AllowInteraction = false;
            ContactDamageKind = DamageKind.Lightning;
            PowerBonus = 1;
            PlacementPhase = PlacementPhase.Creature;
        }
    }
}
