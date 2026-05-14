using Adberration.Scripting;
using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

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
        private const string CorridorLeftWallName = "CorridorLeftWall";
        private const string CorridorLever = "CorridorLever";
        private const string CorridorRightGateName = "CorridorRightGate";
        private const string CorridorRightGateImageName = "RightGate";
        private const string CorridorRightWallName = "CorridorRightWall";
        private const string CorridorRightWallPatchName = "CorridorRightWallPatch";

        #endregion

        #region Private fields

        private int closedDoorTimer = -1;

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

        // CloseCorridorDoorsCore
        private void CloseCorridorDoorsCore()
        {
            var shake = false;

            foreach (var door in Children.OfType<RideDoor>())
            {
                door.Close();
                shake = true;
            }

            if (shake)
                Session.Camera.Shake(TweenStyle.Linear, new(.8f), 50, 6);
        }

        #endregion

        #region Protected members

        // OnActivate
        protected override void OnActivate()
        {
            base.OnActivate();

            // Move away NPCs from player
            foreach (var npc in Children.OfType<Actor>())
            {
                if (!npc.IsPlayer && npc.X < 90)
                    npc.X = BoundingBox.Width / 2;
            }

            if (!Visited)
            {
                if (Session.ScriptLibrary.FindRoutine(CorridorLeftGateRoutineName) is Script script)
                    Session.AwaitScript(script);

                Session.HUD.Countdown.Start(GameSettings.CountdownDuration, false);
            }
            else if (Session.PreviousRoom is SideRoom)
            {
                Session.CloseCorridorDoor();
            }
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();

            if (Session.Player != null)
                Session.Player.Faith++;

            if (Session.FindDeclaredThing(CorridorRightWallPatchName) is Prop rightWallPatch)
            {
                rightWallPatch.Atlas = Atlas;
                rightWallPatch.Position = BoundingBox.GetPoint(RectanglePoint.RightTop);
            }
        }

        // OnPopulating
        protected override void OnPopulating()
        {
            base.OnPopulating();

            foreach (var door in Children.OfType<RideDoor>())
            {
                if (door.DoorDirection == RideDoorDirection.Up)
                    door.IsOpen = true;
            }

            if (Session.FindDeclaredThing(CorridorLeftGateName) is Prop leftGate)
            {
                leftGate.Unload();
                leftGate.DefaultImageName = CorridorLeftGateImageName;
                AddGate(leftGate, RoomNode.Definition.LeftGatePosition);
            }

            if (Session.FindDeclaredThing(CorridorRightGateName) is Prop rightGate)
            {
                rightGate.Unload();
                rightGate.DefaultImageName = CorridorRightGateImageName;
                AddGate(rightGate, RoomNode.Definition.RightGatePosition);
            }

            if (Session.FindDeclaredThing(CorridorLeftWallName) is Prop leftWall)
            {
                leftWall.Unload();
                leftWall.Atlas = Atlas;
                Children.Add(leftWall);
            }

            if (Session.FindDeclaredThing(CorridorRightWallName) is Prop rightWall)
            {
                rightWall.Unload();
                rightWall.Atlas = Atlas;
                Children.Add(rightWall);
            }

            if (Session.FindDeclaredThing(CorridorLever) is Prop lever)
            {
                lever.Unload();
                if (RoomNode.Definition.LeverPosition.HasValue)
                    lever.Position = RoomNode.Definition.LeverPosition.Value;
                Children.Add(lever);
            }

            // Boss
            //if (Session.RunCount > 0 || Session.CurrentRun?.CorridorIndex > 0)
            {
                if (RoomNode.Definition.BossPosition != null)
                {
                    if (SpawnBoss() is Actor boss)
                        Session.Bosses.Add(boss);

                    if (Session.Boss != null && RoomNode.Definition.BossPosition.HasValue)
                        Session.Boss.Position = RoomNode.Definition.BossPosition.Value;
                }
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (closedDoorTimer >= 0)
            {
                closedDoorTimer -= gameTime.ElapsedGameTime.Milliseconds;
                if (closedDoorTimer < 0)
                    CloseCorridorDoorsCore();
            }
        }

        // SpawnBoss
        private Actor? SpawnBoss(Vector2? position = null)
        {
            if (Session.CurrentRun == null)
                return null;

            var candidates = GetCandidateDefinitions<ActorDefinition, Actor>(
                ActorDefinition.Definitions.All,
                def => def.SpawnLocation == SpawnLocation.Gate
            );

            if (candidates.Count == 0)
                return null;

            // Pick
            var chanceTable = new ChanceTable();
            foreach (var c in candidates)
            {
                var finalWeight = AdjustWeight(Session.CurrentRun.Intensity, RoomNode.Definition.Difficulty, c.Difficulty, c.SpawnWeight);
                chanceTable.Add(c.Name, finalWeight);
            }

            if (chanceTable.GetValue() is not ChanceTableItem chanceTableItem)
                return null;

            if (ActorDefinition.Definitions.Find(chanceTableItem.Name) is not ActorDefinition chosen)
                return null;

            var instance = CreateThingClone(chosen.Name);

            if (position.HasValue)
            {
                instance.Position = position.Value;
                Children.Add(instance);
            }

            // Log spawn
            Session.CurrentRun.Spawns.Increment(chosen.Name);

            return instance as Actor;
        }

        #endregion

        // AddCorridorExit
        public void AddCorridorExit()
        {
            if (Session.GetEntity<Prop>(CorridorExitName) is Prop exit)
            {
                Session.StopCountdown();
                exit.ApproachPosition = RoomNode.Definition.ExitApproachPosition;
                exit.Hotspot.SetVertices(RoomNode.Definition.ExitHotspot);
                Children.Add(exit);

                Session.HUD.Message.Show(MessageKind.PathCleared);

                if (Session.ScriptLibrary.FindRoutine("CorridorRightGate-MoveUp") is Script script)
                    Session.ScriptProcessor.StartScript(script);
            }
        }

        // CloseCorridorDoor
        public void CloseCorridorDoor()
        {
            foreach (var door in Children.OfType<RideDoor>())
            {
                door.AllowInteraction = false;
            }

            closedDoorTimer = 1000;
        }
    }
}
