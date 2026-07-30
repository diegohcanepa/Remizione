namespace ScaryCastle
{
    /// <summary>
    /// FlyingSkull
    /// </summary>
    public sealed class FlyingSkull : Actor
    {
        // Constructor
        public FlyingSkull(GameSession session, string name)
            : base(session, name)
        {
            BodySize = BodySize.Small;
            AnimationSettings.SupressAll();
            RemainsKind = RemainsKind.None;
            FloatingForce = 1;
            ShadowSpotSize = 9;
        }
    }
}
