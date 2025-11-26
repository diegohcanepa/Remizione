using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// ProceduralRoom
    /// </summary>
    public abstract class ProceduralRoom : GameRoom
    {
        #region Private fields

        private int instanceCount;
        private readonly List<Placeholder> placeholders = [];
        private readonly int randomSeed;

        #endregion

        #region Constructor

        // Constructor
        protected ProceduralRoom(GameSession session, string name, RoomGraph roomGraph)
            : base(session, name)
        {
            this.RoomGraph = roomGraph;

            AllowGlobalLight = true;
            LightingSystem = true;
            UnloadMode = Adberration.UnloadMode.Manual;

            int salt = roomGraph.Id;
            this.randomSeed = RandomHelper.GetSeed(Session.Seed, salt);
            this.Placeholders = new ReadOnlyCollection<Placeholder>(placeholders);
            this.Random = new Random(randomSeed);
        }

        #endregion

        #region Private members

        // CreateRuntimeThingCloneCore
        private GameThing CreateRuntimeThingCloneCore(string staticName)
        {
            if (Session.CreateRuntimeThingClone(staticName, $"{staticName}*{RoomGraph.Id}_{Name}_{instanceCount}") is not GameThing result)
                throw new InvalidOperationException($"Failed to create runtime clone from'{staticName}'.");

            instanceCount++;

            return result;
        }

        // GetStaticThings
        private List<GameThing> GetStaticThings(Placeholder placeholder)
        {
            var result = new List<GameThing>();

            foreach (var staticThing in Session.StaticThings)
            {
                // Match placeholder type?
                if (staticThing.PlaceholderType != placeholder.PlaceholderType)
                    continue;

                // Match placeholder size?
                if (placeholder.Size != PlaceholderSize.Any && staticThing.PlaceholderSize != placeholder.Size)
                    continue;
                
                result.Add(staticThing);
            }

            return result;
        }

        // PopulateThing
        private void PopulateThing(GameThing thing, PlacementData placementData)
        {
            var count = Random.Next(placementData.Rolls.Minimum, placementData.Rolls.Maximum + 1);

            /*
            for (int i = 0; i < count; i++)
            {
                SpawnThing(thing, finalCol, finalRow, placementData);
                placementData.LogSpawn(thing.StaticName);
                if (!placementData.CanSpawn(thing.StaticName))
                    return;
            }
            */
        }

        // SpawnThing
        private void SpawnThing(GameThing thing, int col, int row, PlacementData placementData)
        {
            /*
            var sizeInCells = grid.GetRequiredGridSpace(thing);
            var ltPos = grid.GetPixelArea(col, row).GetPoint(RectanglePoint.LeftTop) + new Vector2(.5f);
            var rect = new RectangleF(ltPos.X, ltPos.Y, sizeInCells.Width * grid.CellSize, sizeInCells.Height * grid.CellSize);
            var instance = CreateRuntimeThingCloneCore(thing.StaticName);
            instance.Position = rect.GetPoint(RectanglePoint.Bottom);
            Children.Add(instance);

            if (placementData.MaximumPerRun > 0)
                RunManager.LogSpawn(thing.StaticName);
            */
        }

        #endregion

        #region Protected members

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();

            CustomWidth = (int)BoundingBox.Width;
            CustomHeight = (int)BoundingBox.Height;

            OnPopulating();
            Populate();
            OnPopulated();
        }

        // OnPopulating
        protected virtual void OnPopulating()
        {
        }

        // OnPopulated
        protected virtual void OnPopulated()
        {
        }

        // Populate
        private void Populate()
        {
            var data = Session.PlacementDataPool.GetRoomPlacementData(RoomGraph.RoomStyle);
            if (data.Count == 0)
                return;

            var placeholders = new List<Placeholder>(Placeholders);
            placeholders.Shuffle(Random);

            // Placeholders
            foreach (var placeholder in placeholders)
            {
                if (placeholder.Used)
                    continue;

                var staticThings = GetStaticThings(placeholder);
                staticThings.Shuffle(Random);

                // Filtered things
                foreach (var thing in staticThings)
                {
                    var placementDataList = data.GetList(thing.StaticName);
                    if (placementDataList == null)
                        continue;

                    for (var i = 0; i < placementDataList.Count; i++)
                    {
                        var placementData = placementDataList[i];

                        if (!placementData.IsAvailable(this, thing, Random))
                            continue;

                        var instance = CreateRuntimeThingCloneCore(thing.StaticName);
                        instance.Position = placeholder.Polygon.BoundingRectangleF.GetPoint(RectanglePoint.Bottom);
                        Children.Add(instance);
                        placeholder.Used = true;
                        break;
                    }
                }
            }
        }

        // Random
        protected Random Random { get; }

        #endregion

        // AddPlaceholder
        public Placeholder AddPlaceholder(string name, PlaceholderType type, PlaceholderSize size, bool flipImage, string vertices)
        {
            var result = new Placeholder(name, type, size, flipImage, ReadOnlyPolygon.GetVertices(vertices));
            placeholders.Add(result);
            return result;
        }

        // CanPlaceThingAt
        public bool CanPlaceThingAt(GameThing thing, Vector2 position)
        {
            if (!thing.Collider.IsEmpty)
            {
                var box = new RectangleF(position, thing.Collider.BoundingRectangleF.Size);

                for (int i = 0; i < CulledThings.Count; i++)
                {
                    if (CulledThings[i] == thing)
                        continue;

                    if (CulledThings[i] is IHoleArea holeArea)
                    {
                        if (holeArea.Polygon.BoundingRectangleF.Intersects(box))
                            return false;
                    }
                }
            }

            return true;
        }

        // CreateRuntimeClone
        public GameThing? CreateRuntimeClone(string staticName)
        {
            return CreateRuntimeThingCloneCore(staticName);
        }

        // Placeholders
        public ReadOnlyCollection<Placeholder> Placeholders { get; }

        // PlaceRuntimeCloneAt
        public GameThing? PlaceRuntimeCloneAt(GameThing thing, Vector2 position)
        {
            GameThing? result = null;

            if (CanPlaceThingAt(thing, position))
            {
                result = Session.CreateRuntimeThingClone(thing.StaticName, string.Empty) as GameThing;
                if (result != null)
                {
                    result.Position = position;
                    Children.Add(result);
                }
                else
                    return null;
            }

            return result;
        }

        // RoomGraph
        public RoomGraph RoomGraph { get; }

        // ToString
        public override string ToString() => $"ProcRoom_{RoomGraph.Id}";
    }
}
