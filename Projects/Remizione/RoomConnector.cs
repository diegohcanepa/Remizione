using EngendroAdventure.Scripting;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// RoomConnector
    /// </summary>
    public class RoomConnector : IsometricProp
    {
        private bool isOpened;
        private const string ClosedState = "Closed";
        private const string OpenState = "Open";

        // Constructor
        public RoomConnector(GameSession session, string name)
            : base(session, name)
        {
            this.Atlas = Atlases.Environment;
            this.PlacementPhase = PlacementPhase.RoomConnector;
        }

        #region Protected members

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            isOpened = false;
            AnimationPlayer.Play(ClosedState);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            if (Session.IsCountdownActive && !isOpened)
                Open();
        }

        #endregion

        // GhostCar
        [ScriptProperty]
        public OutgoingGhostCar? GhostCar { get; set; }

        // GhostCarOffset
        [ScriptProperty]
        public Vector2 GhostCarOffset { get; set; }

        // IsOpen
        [ScriptProperty]
        public bool IsOpen => Session.IsCountdownActive || isOpened;

        // Open
        [ScriptMethod]
        public void Open()
        {
            if (!isOpened && AnimationPlayer.Animation?.Name != OpenState)
            {
                AnimationPlayer.Play(OpenState, false);
                GhostCar?.TurnOn();
            }
        }

        // RoomTheme
        [ScriptProperty]
        public ProceduralRoomTheme RoomTheme { get; set; }

        // NW
        [ScriptProperty(CodingContext.EntityDeclaration)]
        public bool NW { get; set; } = true;
    }
}
