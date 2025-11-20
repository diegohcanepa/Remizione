using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// ProceduralRoom
    /// </summary>
    public abstract class ProceduralRoom : GameRoom
    {
        #region Private fields

        private readonly TextSprite cellLabel;
        private int instanceCount;
        private bool populated;
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

            int salt = roomGraph.Id;
            this.randomSeed = RandomHelper.GetSeed(Session.Seed, salt);
            this.Random = new Random(randomSeed);

            this.cellLabel = new(Game, Fonts.Common)
            {
                Scale = new(.04f)
            };
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

        // DistributeClumped
        private void DistributeClumped(ProceduralRoomGrid grid, GameThing thing, PlacementData placementData)
        {
            int totalCount = Random.Next(placementData.Rolls.Minimum, placementData.Rolls.Maximum + 1);
            int clumpSize = 3 + Random.Next(3);
            int clumpCount = (totalCount + clumpSize - 1) / clumpSize;
            Size sizeInCells = grid.GetRequiredGridSpace(thing);

            for (int i = 0; i < clumpCount; i++)
            {
                if (!grid.TryReserveSpace(thing.StaticName, sizeInCells, out int baseCol, out int baseRow))
                    break;

                SpawnThing(grid, thing, baseCol, baseRow, placementData);
                placementData.LogSpawn(thing.StaticName);
                if (!placementData.CanSpawn(thing.StaticName))
                    return;

                for (int j = 0; j < clumpSize - 1; j++)
                {
                    int offsetCol = baseCol + Random.Next(-1, 2);
                    int offsetRow = baseRow + Random.Next(-1, 2);

                    if (grid.TryReserveSpace(thing.StaticName, sizeInCells, out int col, out int row, offsetCol, offsetRow))
                    {
                        SpawnThing(grid, thing, col, row, placementData);
                        placementData.LogSpawn(thing.StaticName);
                        if (!placementData.CanSpawn(thing.StaticName))
                            return;
                    }
                }
            }
        }

        // DistributeRandomly
        private void DistributeRandomly(ProceduralRoomGrid grid, GameThing thing, PlacementData placementData)
        {
            Size sizeInCells = grid.GetRequiredGridSpace(thing);
            var count = Random.Next(placementData.Rolls.Minimum, placementData.Rolls.Maximum + 1);

            for (int i = 0; i < count; i++)
            {
                // Intentos limitados para evitar bucles infinitos si no hay espacio
                int maxAttempts = 20 + (sizeInCells.Width * sizeInCells.Height) * 2;
                bool placed = false;

                for (int attempt = 0; attempt < maxAttempts && !placed; attempt++)
                {
                    int col = Random.Next(grid.ColCount - sizeInCells.Width + 1);
                    int row = Random.Next(grid.RowCount - sizeInCells.Height + 1);

                    if (grid.TryReserveSpace(thing.StaticName, sizeInCells, out int finalCol, out int finalRow, col, row))
                    {
                        SpawnThing(grid, thing, finalCol, finalRow, placementData);
                        placed = true;
                        placementData.LogSpawn(thing.StaticName);
                        if (!placementData.CanSpawn(thing.StaticName))
                            return;
                    }
                }
            }
        }

        // DistributeWithNoiseMap
        private void DistributeWithNoiseMap(ProceduralRoomGrid grid, GameThing thing, PlacementData placementData, int seed)
        {
            Size sizeInCells = grid.GetRequiredGridSpace(thing);
            float noiseThreshold = 0.2f;
            int attempts = 100;

            for (int i = 0; i < attempts; i++)
            {
                if (!grid.TryReserveSpace(thing.StaticName, sizeInCells, out int col, out int row))
                    break;

                float noise = GetNoise(col, row, seed);
                if (noise > noiseThreshold)
                    continue;

                SpawnThing(grid, thing, col, row, placementData);
                placementData.LogSpawn(thing.StaticName);
                if (!placementData.CanSpawn(thing.StaticName))
                    return;
            }
        }

        // DrawGrid
        private void DrawGrid(GameTime gameTime)
        {
            for (var col = 0; col < Grid.ColCount; col++)
            {
                for (var row = 0; row < Grid.RowCount; row++)
                {
                    var pos = Grid.GetPixelArea(col, row).GetPoint(RectanglePoint.LeftTop);
                    var rect = new RectangleF(pos.X + 1, pos.Y + 1, Grid.CellSize, Grid.CellSize);
                    rect.Inflate(-1, -1);
                    var color = (Grid.IsCellFree(col, row) ? Color.Green : Color.Red) * .1f;

                    Game.Shapes.DrawRectangle(rect, color);

                    cellLabel.Text = Grid.GetCellLabel(col, row);
                    cellLabel.Position = pos + new Vector2(2);
                    cellLabel.Draw(gameTime);
                }
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

        // SpawnThing
        private void SpawnThing(ProceduralRoomGrid grid, GameThing thing, int col, int row, PlacementData placementData)
        {
            var sizeInCells = grid.GetRequiredGridSpace(thing);
            var ltPos = grid.GetPixelArea(col, row).GetPoint(RectanglePoint.LeftTop) + new Vector2(.5f);
            var rect = new RectangleF(ltPos.X, ltPos.Y, sizeInCells.Width * grid.CellSize, sizeInCells.Height * grid.CellSize);
            var instance = CreateRuntimeThingCloneCore(thing.StaticName);
            instance.Position = rect.GetPoint(RectanglePoint.Bottom);
            Children.Add(instance);

            if (placementData.MaximumPerRun > 0)
                RunManager.LogSpawn(thing.StaticName);
        }

        #endregion

        #region Protected members

        // Grid
        protected ProceduralRoomGrid Grid { get; } = new("Grid");

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);
            Game.SpriteBatch.Begin(Session.Camera);

            if (ShowGrid)
                DrawGrid(gameTime);

            Game.SpriteBatch.End();
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();

            if (!populated)
            {
                populated = true;

                CustomWidth = (int)BoundingBox.Width;
                CustomHeight = (int)BoundingBox.Height;
                Grid.Resize(CustomWidth, CustomHeight);

                OnSetupWalkArea();

                if (WalkArea != null)
                {
                    for (var row = 0; row < Grid.RowCount; row++)
                    {
                        for (var col = 0; col < Grid.ColCount; col++)
                        {
                            var cellArea = Grid.GetPixelArea(row, col);

                            if (!WalkArea.Polygon.BoundingRectangleF.Contains(cellArea))
                                Grid.MarkOccupied(string.Empty, col, row);
                        }
                    }
                }

                OnPopulating();
                Populate();
                OnPopulated();
            }
        }

        // OnPopulating
        protected virtual void OnPopulating()
        {
        }

        // OnPopulated
        protected virtual void OnPopulated()
        {
        }

        // OnSetupWalkArea
        protected virtual void OnSetupWalkArea()
        {
        }

        // Populate
        private void Populate()
        {
            var data = Session.PlacementDataPool.GetRoomPlacementData(RoomGraph.RoomKind);
            if (data.Count == 0)
                return;

            foreach (var phase in Enum.GetValues<PlacementPhase>())
            {
                if (phase == PlacementPhase.None)
                    continue;

                var staticThings = GetStaticThings(phase);
                staticThings.Shuffle(Random);

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

                        placementData.ResetSpawnCount();

                        switch (placementData.DistributionStrategy)
                        {
                            // Random
                            case PlacementDistributionStrategy.Random:
                                DistributeRandomly(Grid, thing, placementData);
                                break;

                            // Clump
                            case PlacementDistributionStrategy.Clump:
                                DistributeClumped(Grid, thing, placementData);
                                break;

                            // NoiseMap 
                            case PlacementDistributionStrategy.NoiseMap:
                                DistributeWithNoiseMap(Grid, thing, placementData, randomSeed);
                                break;
                        }
                    }
                }
            }
        }

        // Random
        protected Random Random { get; }

        // RequiresPersistence
        protected sealed override bool RequiresPersistence => false;

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
            return CreateRuntimeThingCloneCore(staticName);
        }

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

        // ShowGrid
        public static bool ShowGrid { get; set; }

        // ToString
        public override string ToString() => $"ProcRoom_{RoomGraph.Id}";
    }
}
