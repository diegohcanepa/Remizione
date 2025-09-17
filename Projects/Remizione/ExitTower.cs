using Adberration.Scripting;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ExitTower
    /// </summary>
    public sealed class ExitTower : Tower
    {
        // Constructor
        public ExitTower(GameSession session, string name)
            : base(session, name)
        {
        }

        #region Protected members

        // OnOpen
        protected override void OnOpen()
        {
            base.OnOpen();
            RideCar?.TurnOn();
        }

        #endregion

        // RideCar
        [ScriptProperty]
        public ExitRideCar? RideCar { get; set; }

        // RideCarOffset
        [ScriptProperty]
        public Vector2 RideCarOffset { get; set; }
    }
}
