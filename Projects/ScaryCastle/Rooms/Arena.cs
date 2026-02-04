namespace ScaryCastle
{
    /// <summary>
    /// Arena
    /// </summary>
    public sealed class Arena : GameRoom
    {
        // Arena
        public Arena(GameSession session, string name)
            : base(session, name)
        {
            FollowPlayer = false;
            Zoom = 1.2f;
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            Session.BeginCombat();
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            Session.Camera.Position = new(120, 900);
        }
    }
}
