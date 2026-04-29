using System;

namespace ScaryCastle
{
    /// <summary>
    /// SideRoom
    /// </summary>
    public sealed class SideRoom : RideRoom
    {
        // Constructor
        public SideRoom(GameSession session, RoomNode roomNode)
            : base(session, roomNode)
        {
            if (roomNode.RoomType != RoomType.SideRoom)
                throw new InvalidOperationException($"Invalid room type for SideRoom: {roomNode.RoomType}");
        }

        #region Private members

        // ActivateTimer
        private bool ActivateTimer()
        {
            if (Session.CurrentRun == null)
                return false;

            // 1. Probabilidad base (0.2 a 0.8 según progreso)
            float baseChance = 0.20f + (Session.CurrentRun.Progress * 0.60f);

            // 2. Aplicamos Luck como un factor de mitigación.
            // Si luck es 0, dividimos por 1 (no cambia nada).
            // Usamos Max(0, luck) para que una suerte negativa no haga explotar la división.
            float finalProbability = baseChance / (1.0f + Math.Max(0.0f, Session.PlayerStats.Luck.Value));

            // 3. Clamp de seguridad
            finalProbability = Math.Clamp(finalProbability, 0.05f, 0.95f);

            // 4. Roll
            return (float)Random.Shared.NextDouble() < finalProbability;
        }

        #endregion

        // OnActivate
        protected override void OnActivate()
        {
            base.OnActivate();

            /*
            if (!Visited && Session.CurrentRun != null && ActivateTimer())
                Session.HUD.Countdown.Start(GameSettings.CorridorFrontGateCooldown);
            */
        }
    }
}
