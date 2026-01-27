namespace ScaryCastle
{
    /// <summary>
    /// Zabul
    /// </summary>
    public sealed class Zabul : Actor
    {
        // Constructor
        public Zabul(GameSession session, string name)
            : base(session, name)
        {
            BodySize = ActorSize.Small;
            AnimationSettings.SupressAll();
            Guts = 0;
            FloatingForce = 1;
            ShadowSpotSize = 5;
        }
    }
}
