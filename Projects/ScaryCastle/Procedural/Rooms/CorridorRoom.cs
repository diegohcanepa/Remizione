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

        // RollGateEvent
        private static GateEventType RollGateEvent(Difficulty roomDiff)
        {
            // Tiramos un dado del 1 al 100
            int roll = Random.Shared.Next(1, 101);

            return roomDiff switch
            {
                // Easy: 50% Malo, 35% Nada, 15% Bueno
                Difficulty.Easy => roll <= 50 ? GateEventType.Bad :
                                   roll <= 85 ? GateEventType.Nothing : GateEventType.Good,

                // Normal: 70% Malo, 22% Nada, 8% Bueno
                Difficulty.Normal => roll <= 70 ? GateEventType.Bad :
                                     roll <= 92 ? GateEventType.Nothing : GateEventType.Good,

                // Hard: 85% Malo, 13% Nada, 2% Bueno (Casi imposible)
                Difficulty.Hard => roll <= 85 ? GateEventType.Bad :
                                   roll <= 98 ? GateEventType.Nothing : GateEventType.Good,

                _ => GateEventType.Nothing
            };
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

            Session.GateEvent = GateEventType.Nothing;

            // Boss
            //if (Session.RunCount > 0 || Session.CurrentRun?.CorridorIndex > 0)
            {
                if (RoomNode.Definition.BossPosition != null)
                {
                    Session.Bosses.Clear();
                    Session.GateEvent = RollGateEvent(RoomNode.Definition.Difficulty);

                    if (SpawnBoss(Session.GateEvent) is Actor boss)
                    {
                        Session.Bosses.Add(boss);

                        if (RoomNode.Definition.BossPosition.HasValue)
                            boss.Position = RoomNode.Definition.BossPosition.Value;
                    }
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
        private Actor? SpawnBoss(GateEventType eventType)
        {
            if (Session.CurrentRun == null)
                return null;

            // Si el dado dijo que no sale nada, salimos rápido
            if (eventType == GateEventType.Nothing)
                return null;

            // 2. Recolectamos candidatos usando tu GetCandidateDefinitions (que ya filtra progreso, etc.)
            var candidates = GetCandidateDefinitions<ActorDefinition, Actor>(
                ActorDefinition.Definitions.All,
                def => def.SpawnLocation == SpawnLocation.Gate
            );

            if (candidates.Count == 0)
                return null;

            // 3. Filtramos por facción en base al evento que elegimos, y guardamos un fallback
            var chanceTable = new ChanceTable();
            ActorDefinition? fallbackBadBoss = null;

            for (int i = 0; i < candidates.Count; i++)
            {
                var c = candidates[i];
                bool isFriendly = c.Faction is Faction.Good; // O tu propiedad equivalente de facción

                // Si buscamos algo bueno y es malo, o viceversa, lo ignoramos
                if (eventType == GateEventType.Good && !isFriendly)
                    continue;

                if (eventType == GateEventType.Bad && isFriendly)
                    continue;

                // Guardamos el bicho malo de menor progreso por si la tabla de chances se queda vacía
                if (eventType == GateEventType.Bad && (fallbackBadBoss == null || c.MinProgress < fallbackBadBoss.MinProgress))
                    fallbackBadBoss = c;

                var finalWeight = AdjustWeight(Session.CurrentRun.Intensity, RoomNode.Definition.Difficulty, c.Difficulty, c.SpawnWeight);
                chanceTable.Add(c.Name, finalWeight);
            }

            // 4. Selección final
            ActorDefinition? chosenDef = null;

            if (chanceTable.Count > 0 && chanceTable.GetValue() is ChanceTableItem chanceTableItem)
            {
                chosenDef = ActorDefinition.Definitions.Find(chanceTableItem.Name);
            }
            else if (eventType == GateEventType.Bad)
            {
                // Si el pool dinámico falló pero el juego exige un Boss Malo, aplicamos el fallback obligatorio
                chosenDef = fallbackBadBoss;
            }

            // Si no se pudo seleccionar nada (o era un evento Good y no había aliados disponibles), abrimos limpio
            if (chosenDef == null)
                return null;

            // 5. Instanciación (Respetando tu arquitectura exacta de clonación)
            var instance = CreateThingClone<Actor>(chosenDef.Name);

            // Log spawn global
            Session.CurrentRun.Spawns.Increment(chosenDef.Name);

            return instance;
        }

        #endregion

        // AddCorridorExit
        public void AddCorridorExit()
        {
            if (Session.GetEntity<Prop>(CorridorExitName) is Prop exit)
            {
                exit.ApproachPosition = RoomNode.Definition.ExitApproachPosition;
                exit.Hotspot.SetVertices(RoomNode.Definition.ExitHotspot);
                Children.Add(exit);

                Session.TextHUD.Message.Show(MessageKind.PathCleared);

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
