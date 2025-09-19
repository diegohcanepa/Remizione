using Adberration;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
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

        // OnPopulate
        protected override void OnPopulate()
        {
            if (RoomPosition == RoomPosition.First)
            {
                // Entrance rail
                if (Session.GetEntity<GameThing>("EntranceRail") is GameThing entranceRail)
                {
                    var sizeInCells = entranceRail.GetRequiredGridSpace(ProceduralRoomGrid.CellSize);
                    if (MainGrid.TryReserveSpace(sizeInCells, out int col, out int row))
                        Children.Add(entranceRail);
                }
            }
            else
            {
                // Left tower
                if (Session.GetEntity<IsometricProp>("LeftTower") is IsometricProp leftTower)
                {
                    var sizeInCells = leftTower.GetRequiredGridSpace(ProceduralRoomGrid.CellSize);
                    if (MainGrid.TryReserveSpace(sizeInCells, out _, out _))
                    {
                        this.LeftTower = CreateRuntimeClone(leftTower.StaticName) as IsometricProp;
                        Children.Add(leftTower);
                    }
                }
            }

            // Right tower
            if (RoomPosition != RoomPosition.Last)
            {
                if (Session.GetEntity<IsometricProp>("RightTower") is IsometricProp rightTower)
                {
                    var sizeInCells = rightTower.GetRequiredGridSpace(ProceduralRoomGrid.CellSize);
                    if (MainGrid.TryReserveSpace(sizeInCells, out _, out _))
                    {
                        this.RightTower = CreateRuntimeClone(rightTower.StaticName) as IsometricProp;
                        if (this.RightTower != null)
                        {
                            this.RightTower.Position = new Vector2(CustomWidth, rightTower.BoundingBox.Height + 20);
                            Children.Add(this.RightTower);
                        }
                    }
                }
            }
        }

        // OnPopulateCompleted
        protected override void OnPopulateCompleted()
        {
            SetupExitRideCar();
        }

        #endregion

        // LeftTower
        public IsometricProp? LeftTower { get; private set; }

        // RightTower
        public IsometricProp? RightTower { get; private set; }
    }
}
