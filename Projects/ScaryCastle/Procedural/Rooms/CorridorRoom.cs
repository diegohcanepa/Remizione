using Adberration;
using Adberration.Scripting;
using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// Corridor
    /// </summary>
    public sealed class CorridorRoom : RideRoom
    {
        #region Constants

        private const string CorridorExitName = "CorridorExit";
        private const string CorridorLeftGateName = "CorridorLeftGate";
        private const string CorridorLeftGateImageName = "LeftGate";
        private const string CorridorLeftGateRoutineName = "CorridorLeftGate-MoveDown";
        private const string CorridorRightGateRoutineName = "CorridorRightGate-MoveUp";
        private const string CorridorLeftWallName = "CorridorLeftWall";
        private const string CorridorRightGateName = "CorridorRightGate";
        private const string CorridorRightGateImageName = "RightGate";
        private const string CorridorRightWallName = "CorridorRightWall";

        #endregion

        #region Constructor

        // Constructor
        public CorridorRoom(GameSession session, RoomNode roomNode)
            : base(session, roomNode)
        {
            if (roomNode.RoomType != RoomType.Corridor)
                throw new InvalidOperationException($"Invalid room type for SideRoom: {roomNode.RoomType}");
        }

        #endregion

        #region Private members

        // AddGate
        private void AddGate(Prop gate, Vector2 position)
        {
            gate.Atlas = Atlas;
            gate.PivotOrigin = RectanglePoint.LeftTop;
            gate.RenderLayer = RenderLayer.Background;
            gate.DepthOffset = -1;
            gate.Position = position;
            Children.Add(gate);
        }

        #endregion

        #region Protected members

        // OnActivate
        protected override void OnActivate()
        {
            base.OnActivate();

            if (!Visited)
            {
                if (Session.ScriptLibrary.FindRoutine(CorridorLeftGateRoutineName) is Script script)
                    Session.ScriptProcessor.StartScript(script);
            }
        }

        // OnChildRemoved
        protected override void OnChildRemoved(Entity child)
        {
            base.OnChildRemoved(child);

            if (child == Guard)
            {
                Guard = null;

                if (Session.ScriptLibrary.FindRoutine(CorridorRightGateRoutineName) is Script script)
                {
                    if (Session.GetEntity<Prop>(CorridorExitName) is Prop exit)
                    {
                        exit.ApproachPosition = RoomNode.Definition.ExitApproachPosition;
                        exit.Hotspot.SetVertices(RoomNode.Definition.ExitHotspot);
                        Children.Add(exit);
                    }

                    Session.HUD.Message.Show(MessageKind.TheWayIsOpen, 3000);

                    Session.ScriptProcessor.StartScript(script);
                }
            }
        }

        // OnPopulating
        protected override void OnPopulating()
        {
            base.OnPopulating();

            if (Session.FindDeclaredThing(CorridorLeftGateName) is Prop leftGate)
            {
                leftGate.DefaultImageName = CorridorLeftGateImageName;
                AddGate(leftGate, RoomNode.Definition.LeftGatePosition);
            }

            if (Session.FindDeclaredThing(CorridorRightGateName) is Prop rightGate)
            {
                rightGate.DefaultImageName = CorridorRightGateImageName;
                AddGate(rightGate, RoomNode.Definition.RightGatePosition);
            }

            if (Session.FindDeclaredThing(CorridorLeftWallName) is Prop leftWall)
            {
                leftWall.Atlas = Atlas;
                Children.Add(leftWall);
            }

            if (Session.FindDeclaredThing(CorridorRightWallName) is Prop rightWall)
            {
                rightWall.Atlas = Atlas;
                Children.Add(rightWall);
            }

            // Corridor guard
            if (RoomNode.Definition.GuardActorPosition is Vector2 position)
            {
                this.Guard = SpawnActor(ActorRole.Guard, position);
                this.Guard?.SuspendRandomMoveUntilVisible = true;

                Session.HUD.GuardMeter.Target = Guard;
            }
        }

        #endregion

        // Guard
        public Actor? Guard { get; private set; }
    }
}
