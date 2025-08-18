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
            DamageStyle = DamageStyle.Blink;
        }

        // FindEnemy
        public override GameThing? FindEnemy() => Session.Player;
    }
}
