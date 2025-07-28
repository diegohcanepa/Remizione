using EngendroAdventure.Scripting;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// RoomConnector
    /// </summary>
    public class RoomConnector : IsometricProp
    {
        private const string Open = "Open";

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
            AnimationPlayer.Play("Closed");
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (IsOpen && AnimationPlayer.Animation?.Name != Open)
            {
                AnimationPlayer.Play(Open, false);
                GhostCar?.TurnOn();
            }
        }

        #endregion

        // GhostCar
        public OutgoingGhostCar? GhostCar { get; set; }

        // GhostCarOffset
        [ScriptProperty]
        public Vector2 GhostCarOffset { get; set; }

        // IsOpen
        [ScriptProperty]
        public bool IsOpen => Session.IsCountdownActive;

        // RoomTheme
        [ScriptProperty]
        public ProceduralRoomTheme RoomTheme { get; set; }

        // NW
        [ScriptProperty(CodingContext.EntityDeclaration)]
        public bool NW { get; set; } = true;
    }
}
