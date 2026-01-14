namespace ScaryCastle
{
    /// <summary>
    /// BloodyEye
    /// </summary>
    public sealed class BloodyEye : Actor
    {
        // Constructor
        public BloodyEye(GameSession session, string name)
            : base(session, name)
        {
            AnimationSettings.SupressAll();
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
        }
    }
}
