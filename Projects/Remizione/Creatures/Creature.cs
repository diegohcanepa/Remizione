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
            HitEffect = HitEffect.Blink;
            PowerBonus = 1;
            PlacementPhase = PlacementPhase.Creature;
        }

        // FindEnemy
        public override GameThing? FindEnemy() => Session.Player;
    }
}
