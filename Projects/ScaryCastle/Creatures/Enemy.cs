namespace ScaryCastle
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
            Faction = Faction.Evil;
        }
    }
}
