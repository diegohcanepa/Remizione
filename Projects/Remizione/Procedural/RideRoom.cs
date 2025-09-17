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
        public RideRoom(GameSession session, string name, int roomIndex, bool isLastRoom)
            : base(session, name, RoomKind.RideRoom, roomIndex, isLastRoom)
        {
        }

        #region Private members

        // SetupExitRideCar
        private void SetupExitRideCar()
        {
            if (Session.GetEntity<ExitRideCar>(nameof(ExitRideCar)) is not ExitRideCar exitRideCar)
                throw new InvalidOperationException("Exit ride car not found.");

            var childList = new List<Thing>(Children);

            foreach (var thing in childList)
            {
                if (thing is ExitTower exitTower)
                {
                    Children.Add(exitRideCar);
                    exitTower.RideCar = exitRideCar;
                    exitRideCar.Position = exitTower.BoundingBox.GetPoint(RectanglePoint.RightBottom) + exitTower.RideCarOffset;
                    break;
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

            if (RoomPosition == RoomPosition.First)
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
            if (RoomPosition == RoomPosition.First)
            {
                // Entrance rail
                if (Session.GetEntity<GameThing>("EntranceRail") is GameThing entranceRail)
                {
                    var sizeInCells = entranceRail.GetRequiredGridSpace(ProceduralRoomGrid.CellSize);
                    if (MainGrid != null && MainGrid.TryReserveSpace(sizeInCells, out int col, out int row))
                        Children.Add(entranceRail);
                }
            }
            else
            {
                // Back tower
                if (Session.GetEntity<GameThing>("BackTower") is GameThing backTower)
                {
                    var sizeInCells = backTower.GetRequiredGridSpace(ProceduralRoomGrid.CellSize);
                    if (MainGrid != null && MainGrid.TryReserveSpace(sizeInCells, out _, out _))
                        Children.Add(backTower);
                }
            }
        }

        // OnPopulateCompleted
        protected override void OnPopulateCompleted()
        {
            SetupExitRideCar();
        }

        #endregion
    }
}
