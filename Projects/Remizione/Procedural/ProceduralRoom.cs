using Adberration;
using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// ProceduralRoom
    /// </summary>
    public sealed class ProceduralRoom : GameRoom
    {
        #region Private fields

        private ProceduralRoomGrid? decorationGrid;
        private int instanceCount;
        private ProceduralRoomGrid? mainGrid;
        private readonly Dictionary<string, List<PlacementData>> placementDataDictionary = [];
        private readonly Random random;
        private readonly int randomSeed;
        private readonly ImageSprite terrainBlock;
        private int terrainCols;
        private int terrainRows;

        #endregion

        #region Constructor

        // Constructor
        public ProceduralRoom(GameSession session, string name)
            : base(session, name)
        {
            LightingSystem = true;

            this.randomSeed = GetSeed(Session.RandomSeed, Session.Stage);
            this.random = new Random(randomSeed);
            this.terrainBlock = new ImageSprite(session.Game);
        }

        #endregion

        #region Private members

        // CreateRuntimeCloneCore
        private GameThing CreateRuntimeCloneCore(string staticName)
        {
            if (Session.CreateRuntimeClone(staticName, $"{staticName}*{Name}_{instanceCount}") is not GameThing result)
                throw new InvalidOperationException($"Failed to create runtime clone from'{staticName}'.");

            instanceCount++;

            return result;
        }

        // DistributeClumped
        private void DistributeClumped(GameThing thing, PlacementData placementData)
        {
            if (mainGrid == null || decorationGrid == null)
                return;

            var targetGrid = thing.IsWalkAreaHole ? mainGrid : decorationGrid;
            int totalCount = random.Next(placementData.Instances.Minimum, placementData.Instances.Maximum + 1);
            int clumpSize = 3 + random.Next(3);
            int clumpCount = (totalCount + clumpSize - 1) / clumpSize;

            Size sizeInCells = thing.GetRequiredGridSpace(ProceduralRoomGrid.CellSize);

            for (int i = 0; i < clumpCount; i++)
            {
                if (!targetGrid.TryReserveSpace(sizeInCells, out int baseCol, out int baseRow))
                    break;

                PlaceRuntimeThing(thing, baseCol, baseRow);

                for (int j = 0; j < clumpSize - 1; j++)
                {
                    int offsetCol = baseCol + random.Next(-1, 2);
                    int offsetRow = baseRow + random.Next(-1, 2);

                    if (targetGrid.TryReserveSpace(sizeInCells, out int col, out int row, offsetCol, offsetRow))
                        PlaceRuntimeThing(thing, col, row);
                }
            }
        }

        // DistributeRandomly
        private void DistributeRandomly(GameThing thing, PlacementData placementData)
        {
            if (mainGrid == null || decorationGrid == null)
                return;

            var targetGrid = thing.IsWalkAreaHole ? mainGrid : decorationGrid;
            Size sizeInCells = thing.GetRequiredGridSpace(ProceduralRoomGrid.CellSize);
            var count = random.Next(placementData.Instances.Minimum, placementData.Instances.Maximum + 1);

            for (int i = 0; i < count; i++)
            {
                // Intentos limitados para evitar bucles infinitos si no hay espacio
                int maxAttempts = 20 + (sizeInCells.Width * sizeInCells.Height) * 2;
                bool placed = false;

                for (int attempt = 0; attempt < maxAttempts && !placed; attempt++)
                {
                    int col = random.Next(targetGrid.ColCount - sizeInCells.Width + 1);
                    int row = random.Next(targetGrid.RowCount - sizeInCells.Height + 1);

                    if (targetGrid.TryReserveSpace(sizeInCells, out int finalCol, out int finalRow, col, row))
                    {
                        PlaceRuntimeThing(thing, finalCol, finalRow);
                        placed = true;
                    }
                }
            }
        }

        // DistributeWithNoiseMap
        private void DistributeWithNoiseMap(GameThing thing, PlacementData placementData, int seed)
        {
            if (mainGrid == null || decorationGrid == null)
                return;

            var targetGrid = thing.IsWalkAreaHole ? mainGrid : decorationGrid;
            Size sizeInCells = thing.GetRequiredGridSpace(ProceduralRoomGrid.CellSize);
            float noiseThreshold = 0.2f;
            int attempts = 100;

            for (int i = 0; i < attempts; i++)
            {
                if (!targetGrid.TryReserveSpace(sizeInCells, out int col, out int row))
                    break;

                float noise = GetNoise(col, row, seed);
                if (noise > noiseThreshold)
                    continue;

                PlaceRuntimeThing(thing, col, row);
            }
        }

        // GetNoise
        private static float GetNoise(int col, int row, int seed)
        {
            unchecked
            {
                int hash = seed;
                hash = (hash * 397) ^ col;
                hash = (hash * 397) ^ row;

                // Mezcla adicional para mayor dispersión
                hash ^= (hash >> 13);
                hash *= 0x5bd1e995;
                hash ^= (hash >> 15);

                // Normaliza a 0..1
                uint uhash = (uint)hash;
                return (uhash & 0xFFFFFF) / (float)0xFFFFFF;
            }
        }

        // GetSeed
        private static int GetSeed(int seed, int salt)
        {
            uint h = (uint)seed;

            h ^= (uint)salt * 0x9E3779B9; // golden number (Knuth)
            h ^= h >> 16;
            h *= 0x85EBCA6B;
            h ^= h >> 13;
            h *= 0xC2B2AE35;
            h ^= h >> 16;

            return (int)h;
        }

        // GetStaticThings
        private List<GameThing> GetStaticThings(PlacementPhase phase)
        {
            var result = new List<GameThing>();

            for (var i = 0; i < Session.StaticThings.Count; i++)
            {
                if (Session.StaticThings[i].PlacementPhase == phase)
                    result.Add(Session.StaticThings[i]);
            }

            return result;
        }

        // PlaceRuntimeThing
        private void PlaceRuntimeThing(GameThing thing, int col, int row)
        {
            if (mainGrid == null || decorationGrid == null)
                return;

            var instance = CreateRuntimeCloneCore(thing.StaticName);
            instance.Position = ProceduralRoomGrid.GetPosition(col, row);
            instance.Y += instance.BoundingBox.Height;
            instance.X += instance.BoundingBox.Width / 2;
            Children.Add(instance);

            RequiredPower += thing.PowerBonus;
        }

        // Populate
        private void Populate()
        {
            RequiredPower = 0;

            // Entrance rail
            if (Session.GetEntity<GameThing>("EntranceRail") is GameThing entranceRail)
            {
                var sizeInCells = entranceRail.GetRequiredGridSpace(ProceduralRoomGrid.CellSize);
                if (mainGrid != null && mainGrid.TryReserveSpace(sizeInCells, out int col, out int row))
                    Children.Add(entranceRail);
            }

            foreach (var phase in Enum.GetValues<PlacementPhase>())
            {
                if (phase == PlacementPhase.None)
                    continue;

                var list = GetStaticThings(phase);

                foreach (var thing in list)
                {
                    if (!placementDataDictionary.TryGetValue(thing.StaticName, out var placementDataList))
                        continue;

                    for (var i = 0; i < placementDataList.Count; i++)
                    {
                        var placementData = placementDataList[i];

                        if (!placementData.IsAvailable(thing, random))
                            continue;

                        switch (placementData.DistributionStrategy)
                        {
                            // RandomCell
                            case PlacementDistributionStrategy.Random:
                                DistributeRandomly(thing, placementData);
                                break;

                            // Clump
                            case PlacementDistributionStrategy.Clump:
                                DistributeClumped(thing, placementData);
                                break;

                            // NoiseMap 
                            case PlacementDistributionStrategy.NoiseMap:
                                DistributeWithNoiseMap(thing, placementData, randomSeed);
                                break;
                        }
                    }
                }
            }

            if (RequiredPower > 0)
                RequiredPower = Math.Max(RequiredPower, 1);

            SetupGhostCars();
        }

        // SetupGhostCars
        private void SetupGhostCars()
        {
            var childList = new List<Thing>(Children);

            foreach (var thing in childList)
            {
                if (thing is RoomConnector roomConnector)
                {
                    var staticName = roomConnector.NW ? "OutgoingGhostCarNW" : "OutgoingGhostCarNE";

                    if (CreateRuntimeCloneCore(staticName) is not OutgoingGhostCar car)
                        throw new InvalidOperationException("Failed to create GhostCar instance.");

                    Children.Add(car);
                    roomConnector.GhostCar = car;
                    car.Position = roomConnector.BoundingBox.GetPoint(RectanglePoint.RightBottom) + roomConnector.GhostCarOffset;
                }
            }
        }

        #endregion

        #region Protected members

        // OnDrawCustomBackground
        protected override void OnDrawCustomBackground(GameTime gameTime)
        {
            terrainBlock.Position = Vector2.Zero;

            for (var i = 0; i < terrainRows; i++)
            {
                for (var j = 0; j < terrainCols; j++)
                {
                    terrainBlock.Draw(gameTime);
                    terrainBlock.X += terrainBlock.BoundingBox.Width;
                }

                terrainBlock.X = 0;
                terrainBlock.Y += terrainBlock.BoundingBox.Height;
            }
        }

        // OnLoad
        protected override void OnLoad()
        {
            AudioManager.Music.PlayTag("PilgrimPath");
            Session.Environment.GlobalLight.Scale = new(2, 1.4f);

            const int walkAreaMargin = 15;

            terrainCols = random.Next(TerrainColRange.Minimum, TerrainColRange.Maximum + 1);
            terrainRows = terrainCols == 1 ? 1 : random.Next(TerrainRowRange.Minimum, TerrainRowRange.Maximum + 1);

            CustomWidth = Screen.NativeWidth * (terrainCols <= 0 ? 1 : terrainCols);
            CustomHeight = Screen.NativeHeight * (terrainRows <= 0 ? 1 : terrainRows);

            this.decorationGrid = new ProceduralRoomGrid("Decoration", CustomWidth, CustomHeight);
            this.mainGrid = new ProceduralRoomGrid("Main", CustomWidth, CustomHeight);

            ClearWalkAreas();

            Vector2[] vertices = [new(walkAreaMargin, walkAreaMargin),
                                  new(CustomWidth - walkAreaMargin, walkAreaMargin),
                                  new(CustomWidth - walkAreaMargin, CustomHeight - walkAreaMargin),
                                  new(walkAreaMargin, CustomHeight - walkAreaMargin)
                                 ];

            AddWalkArea("<Default>", vertices);

            base.OnLoad();

            terrainBlock.Image = Atlas?.GetImage("TerrainBlock");

            Populate();
        }

        // OnUnload
        protected override void OnUnload()
        {
            base.OnUnload();
            Children.Clear();
            Session.CleanUpRuntimeEntities();
        }

        #endregion

        #region Internal members

        // AddPlacementData
        internal void AddPlacementData(string staticName, PlacementData placementData)
        {
            if (placementDataDictionary.TryGetValue(staticName, out var existingList))
                existingList.Add(placementData);
            else
                placementDataDictionary.Add(staticName, [placementData]);
        }

        #endregion

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
            return CreateRuntimeCloneCore(staticName);
        }

        // PlaceRuntimeCloneAt
        public GameThing? PlaceRuntimeCloneAt(GameThing thing, Vector2 position)
        {
            GameThing? result = null;

            if (CanPlaceThingAt(thing, position))
            {
                result = Session.CreateRuntimeClone(thing.StaticName, string.Empty) as GameThing;
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

        // RequiredPower
        public int RequiredPower { get; private set; }

        // TerrainColRange
        [ScriptProperty(CodingContext.Declaration)]
        public Int32Range TerrainColRange { get; set; } = new(2, 3);

        // TerrainRowRange
        [ScriptProperty(CodingContext.Declaration)]
        public Int32Range TerrainRowRange { get; set; } = new(1, 2);
    }
}
