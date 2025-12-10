namespace Remizione
{
    /// <summary>
    /// Enemy
    /// </summary>
    public class Enemy : Actor
    {
        // Constructor
        public Enemy(GameSession session, string name)
            : base(session, name)
        {
            AllowInteraction = false;
            ContactDamageType = DamageType.Physical;
            Faction = Faction.Evil;
        }
    }
}
