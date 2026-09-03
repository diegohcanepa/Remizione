namespace ScaryCastle
{
    // ActorAnimationSettings
    public sealed class ActorAnimationSettings
    {
        // DetachedHead
        public bool DetachedHead { get; set; } = true;

        // MoveBounce
        public bool MoveBounce { get; set; } = true;

        // MoveSway
        public bool MoveSway { get; set; } = true;

        // SupressAll
        public void SupressAll()
        {
            DetachedHead = false;
            MoveBounce = false;
            MoveSway = false;
        }
    }
}
