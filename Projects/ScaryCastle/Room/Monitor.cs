using Engendro;

namespace ScaryCastle
{
    /// <summary>
    /// Monitor
    /// </summary>
    public sealed class Monitor : CloseUpRoom
    {
        // Constructor
        public Monitor(GameSession session, string name)
            : base(session, name)
        {
        }

        // OnClose
        protected override void OnClose()
        {
            base.OnClose();
            if (Session.PreviousRoom != null)
            {
                TransitionManager.CurrentTransition.In(0);
                TransitionManager.CurrentTransition.Out();
                Session.EnterRoom(Session.PreviousRoom);
            }
        }
    }
}
