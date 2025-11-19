using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// IsoRideRoom
    /// </summary>
    public sealed class IsoRideRoom : ProceduralRoom
    {
        private static readonly ChanceTable terrainTable = new();
        private static readonly Dictionary<RoomSize, Vector2[]> terrainVertices = [];

        // Static constructor
        static IsoRideRoom()
        {
            terrainTable.Add(RoomSize.Small.ToString(), 1, 100);
            //terrainTable.Add(RoomSize.Medium.ToString(), 1, 100);

            terrainVertices[RoomSize.Small] = ReadOnlyPolygon.GetVertices("295,17;295,122;5,122;5,17");
            //terrainVertices[RoomSize.Medium] = ReadOnlyPolygon.GetVertices("324,18;324,120;7,120;7,18");
        }

        // Constructor
        public IsoRideRoom(GameSession session, string name, int roomIndex, bool isLastRoom)
            : base(session, name, RoomKind.Hub, roomIndex)
        {
            AllowGlobalLight = true;
        }

        #region Protected members

        // OnPopulating
        protected override void OnPopulating()
        {
            /*
            if (RoomPhase == RunPhase.Start)
            {
                // Entrance rail
                if (Session.GetEntity<GameThing>("EntranceRail") is GameThing entranceRail)
                {
                    Grid.ReserveSpace(entranceRail);
                    Children.Add(entranceRail);
                }
            }
            else
            {
                // Left tower
                LeftTower = CreateRuntimeClone("LeftTower") as IsometricProp;
                if (LeftTower != null)
                {
                    Grid.ReserveSpace(LeftTower, false);
                    Children.Add(LeftTower);

                    if (CreateRuntimeClone("LeftTowerPatch") is IsometricProp leftTowerPatch)
                    {
                        leftTowerPatch.Position = LeftTower.Position;
                        Children.Add(leftTowerPatch);
                    }

                    if (Session.ScriptLibrary.GetRoutine(RoutineNames.GotoPreviousRunRoom) is Script script)
                    {
                        Vector2[] vertices = ReadOnlyPolygon.GetVertices("18,39;15,49;5,45;6,39");
                        AddTriggerArea("PreviousRoom", script, null, true, true, false, null, vertices);
                    }
                }
            }

            // Right tower
            if (RoomPhase != RunPhase.End)
            {
                RightConnector = CreateRuntimeClone("RightTower") as IsometricProp;
                if (RightConnector != null)
                {
                    RightConnector.Position = new Vector2(CustomWidth - 9, RightConnector.BoundingBox.Height - 4);
                    Grid.ReserveSpace(RightConnector, false);
                    Children.Add(RightConnector);

                    if (CreateRuntimeClone("RightTowerPatch") is IsometricProp rightTowerPatch)
                    {
                        rightTowerPatch.Position = RightConnector.Position;
                        Children.Add(rightTowerPatch);
                    }

                    if (Session.ScriptLibrary.GetRoutine(RoutineNames.GotoNextRunRoom) is Script script)
                    {
                        var lt = RightConnector.BoundingBox.GetPoint(RectanglePoint.LeftTop);
                        Vector2[] vertices = ReadOnlyPolygon.GetVertices("35,44;27,50;19,44;23,41");

                        for (var i = 0; i < vertices.Length; i++)
                        {
                            vertices[i] += lt;
                        }

                        AddTriggerArea("NextRoom", script, null, true, true, false, null, vertices);
                    }
                }
            }

            // Exit Tunnel
            else
            {      
                this.RightConnector = Session.GetEntity<IsometricProp>("ExitRail");

                if (this.RightConnector != null)
                {
                    this.RightConnector.Position = new Vector2(CustomWidth, 0);
                    Grid.ReserveSpace(RightConnector, false);
                    Children.Add(this.RightConnector);

                    if (Session.GetEntity<IsometricProp>("ExitTunnel") is IsometricProp exitTunnel)
                    {
                        exitTunnel.Position = RightConnector.Position;
                        Children.Add(exitTunnel);
                    }

                    if (Session.GetEntity<IsometricProp>("ExitTunnelPatch") is IsometricProp exitTunnelPatch)
                    {
                        exitTunnelPatch.Position = RightConnector.Position;
                        exitTunnelPatch.Y += 30;
                        Children.Add(exitTunnelPatch);
                    }

                    if (Session.GetEntity<ExitRideCar>(nameof(ExitRideCar)) is ExitRideCar exitRideCar)
                    {
                        exitRideCar.Position = RightConnector.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 15, -6);
                        Grid.ReserveSpace(exitRideCar, false);
                        Children.Add(exitRideCar);
                    }
                }
            }

            if (RightConnector != null)
            {
                if (CreateRuntimeClone(nameof(SaintPeregrine)) is SaintPeregrine statue)
                {
                    statue.Position = this.RightConnector.BoundingBox.GetPoint(RectanglePoint.LeftBottom, -5, -15);
                    if (RoomPhase == RunPhase.End)
                        statue.Position += new Vector2(20, -41);

                    Grid.ReserveSpace(statue, false);
                    Children.Add(statue);
                }
            }

            Grid.MarkOccupiedMargin("Margin", 1, 1, 1, 2);

            */
        }
        
        #endregion

        // LeftTower
        public IsometricProp? LeftTower { get; private set; }

        // OpenRightTower
        [ScriptMethod]
        public void OpenRightTower()
        {
            AnimationPlayer.Play("Open", false);
        }

        // RequiredCoins
        public int RequiredCoins { get; set; } = 3;

        // RightConnector
        public IsometricProp? RightConnector { get; private set; }
    }
}
