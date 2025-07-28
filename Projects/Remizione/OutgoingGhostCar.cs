using Engendro;
using EngendroAdventure.Scripting;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// OutgoingGhostCar
    /// </summary>
    public sealed class OutgoingGhostCar : IsometricProp
    {
        // Constructor
        public OutgoingGhostCar(GameSession session, string name)
            : base(session, name)
        {
        }

        #region Protected members

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            IsTurnedOn = false;
            Light?.TurnOff(true);
            this.AnimationPlayer.Play("Off");
        }

        #endregion

        // DestinationOffset
        [ScriptProperty(CodingContext.EntityDeclaration)]
        public Vector2 DestinationOffset { get; set; }

        // IsTurnedOn
        [ScriptProperty]
        public bool IsTurnedOn { get; private set; }

        // Launch
        [ScriptMethod]
        public void Launch()
        {
            Tweens.PositionTween = Vector2Tween.Create(TweenStyle.CubicIn, Position, Position + DestinationOffset, 2000);
        }

        // RoomTheme
        [ScriptProperty]
        public ProceduralRoomTheme RoomTheme { get; set; }

        // TurnOn
        public void TurnOn()
        {
            AnimationPlayer.Play("On", false);
            Light?.TurnOn();
            IsTurnedOn = true;
        }
    }
}
