namespace Remizione
{
    // ActorAnimationSettings
    public sealed class ActorAnimationSettings
    {
        // MoveBounce
        public bool MoveBounce { get; set; } = true;

        // MoveSway
        public bool MoveSway { get; set; }

        // SupressAll
        public void SupressAll()
        {
            MoveBounce = false;
            MoveSway = false;
        }
    }
}
