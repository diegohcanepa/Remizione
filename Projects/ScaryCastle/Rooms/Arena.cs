namespace ScaryCastle.Rooms
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
            Zoom = 1.3f;
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            Session.Camera.StopFollowing();
            Session.Camera.FocusCenter();
            Session.BeginCombat();
        }
    }
}
