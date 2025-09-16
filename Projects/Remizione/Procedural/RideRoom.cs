using Adberration;
using Engendro;
using Engendro.Audio;
using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// RideRoom
    /// </summary>
    public sealed class RideRoom : ProceduralRoom
    {
        // Constructor
        public RideRoom(GameSession session, string name)
            : base(session, name)
        {
        }

        #region Private members

        // SetupRideCars
        private void SetupRideCars()
        {
            var childList = new List<Thing>(Children);

            foreach (var thing in childList)
            {
                if (thing is Tower roomConnector)
                {
                    var staticName = roomConnector.NW ? "OutgoingRideCarNW" : "OutgoingRideCarNE";

                    if (CreateRuntimeClone(staticName) is not OutgoingRideCar car)
                        throw new InvalidOperationException("Failed to create RideCar instance.");

                    Children.Add(car);
                    roomConnector.RideCar = car;
                    car.Position = roomConnector.BoundingBox.GetPoint(RectanglePoint.RightBottom) + roomConnector.RideCarOffset;
                }
            }
        }

        #endregion

        #region Protected members

        // RequiresPersistence
        protected override bool RequiresPersistence => false;

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            AudioManager.Music.PlayTag("Ride");
            Session.AwaitRoutine(RoutineNames.IncomingRideCarIntro);
        }

        // OnUnload
        protected override void OnUnload()
        {
            base.OnUnload();
            Children.Clear();
            Session.CleanUpRuntimeEntities();
        }

        // OnPopulate
        protected override void OnPopulate()
        {
            // Entrance rail
            if (Session.GetEntity<GameThing>("EntranceRail") is GameThing entranceRail)
            {
                var sizeInCells = entranceRail.GetRequiredGridSpace(ProceduralRoomGrid.CellSize);
                if (MainGrid != null && MainGrid.TryReserveSpace(sizeInCells, out int col, out int row))
                    Children.Add(entranceRail);
            }
        }

        // OnPopulateCompleted
        protected override void OnPopulateCompleted()
        {
            SetupRideCars();
        }

        #endregion
    }
}
