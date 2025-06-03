using Engendro;
using EngendroAdventure;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// WorldBlock
    /// </summary>
    public class WorldBlock : Prop
    {
        #region Private fields

        private readonly bool[] borders = new bool[4];
        private readonly ImageSprite[] borderImages;
        private readonly WorldBlockGrid decorationGrid;
        private readonly WorldBlockGrid mainGrid;
        private readonly List<GameThing> proceduralThings = [];
        private readonly Random random;
        private readonly int randomSeed;
        private readonly List<WorldBlockTag> tags = [];

        #endregion

        #region Constructor

        // Constructor
        public WorldBlock(WorldManager manager, Point worldGridPosition, int worldVersion, Rectangle? reservedSpace = null)
            : base(manager.Session, string.Empty)
        {
            this.WorldVersion = worldVersion;
            this.decorationGrid = new WorldBlockGrid("Decoration", manager.BlockSize);
            this.mainGrid = new WorldBlockGrid("Main", manager.BlockSize);
            this.ProceduralThings = new(proceduralThings);
            this.Tags = new(tags);

            AssignTags();

            Index = manager.Blocks.Count;
            Manager = manager;
            WorldGridPosition = worldGridPosition;
            PivotOrigin = RectanglePoint.LeftTop;
            RenderLayer = RenderLayer.Background;
            DepthOffset = -1000;
            Position = new(worldGridPosition.X * manager.BlockSize.Width, worldGridPosition.Y * manager.BlockSize.Height);
            DefaultImageName = "TerrainBlockDefault";

            this.randomSeed = GetSeed(manager.Session.RandomSeed, Index);
            this.random = new Random(randomSeed);

            Light = new Light(Manager.Session.Game, "GlobalLight")
            {
                Color = Color.White,
                PivotOrigin = RectanglePoint.Middle,
                Scale = new(10, 6)
            };

            LightPosition = Screen.Center;

            borderImages = new ImageSprite[4];

            // Top
            borderImages[0] = new ImageSprite(Game)
            {
                Image = Atlases.Environment.GetImage($"{DefaultImageName}Top"),
                PivotOrigin = RectanglePoint.Bottom
            };

            // Right
            borderImages[1] = new ImageSprite(Game)
            {
                Image = Atlases.Environment.GetImage($"{DefaultImageName}Right"),
                PivotOrigin = RectanglePoint.Left
            };

            // Bottom
            borderImages[2] = new ImageSprite(Game)
            {
                Image = Atlases.Environment.GetImage($"{DefaultImageName}Bottom"),
                PivotOrigin = RectanglePoint.Top
            };

            // Left
            borderImages[3] = new ImageSprite(Game)
            {
                Image = Atlases.Environment.GetImage($"{DefaultImageName}Left"),
                PivotOrigin = RectanglePoint.Right
            };

            if (reservedSpace.HasValue)
            {
                decorationGrid.ReserveSpace(reservedSpace.Value);
                mainGrid.ReserveSpace(reservedSpace.Value);
            }

            Populate();
        }

        #endregion

        #region Private members

        // AssignTags
        private void AssignTags()
        {
        }

        // CreateDynamicThing
        private GameThing CreateDynamicThing(string staticName)
        {
            if (Session.CreateDynamicThing(staticName, $"{staticName}*{Index}_{proceduralThings.Count}") is not GameThing result)
                throw new InvalidOperationException($"Failed to create dynamic thing '{staticName}'.");

            result.WorldBlockOrigin = this;

            return result;
        }

        // DistributeClumped
        private void DistributeClumped(GameThing thing)
        {
            if (thing.InstancesPerBlock.IsEmpty)
                return;

            var targetGrid = thing.IsWalkAreaHole ? mainGrid : decorationGrid;
            int totalCount = random.Next(thing.InstancesPerBlock.Minimum, thing.InstancesPerBlock.Maximum + 1);
            int clumpSize = 3 + random.Next(3);
            int clumpCount = (totalCount + clumpSize - 1) / clumpSize;

            Size sizeInCells = thing.GetRequiredGridSpace(WorldBlockGrid.CellSize);

            for (int i = 0; i < clumpCount; i++)
            {
                if (!targetGrid.TryReserveSpace(sizeInCells, out int baseCol, out int baseRow))
                    break;

                PlaceDynamicThing(thing, baseCol, baseRow);

                for (int j = 0; j < clumpSize - 1; j++)
                {
                    int offsetCol = baseCol + random.Next(-1, 2);
                    int offsetRow = baseRow + random.Next(-1, 2);

                    if (targetGrid.TryReserveSpace(sizeInCells, out int col, out int row, offsetCol, offsetRow))
                        PlaceDynamicThing(thing, col, row);
                }
            }
        }

        // DistributeRandomly
        private void DistributeRandomly(GameThing thing)
        {
            if (thing.InstancesPerBlock.IsEmpty)
                return;

            var targetGrid = thing.IsWalkAreaHole ? mainGrid : decorationGrid;
            Size sizeInCells = thing.GetRequiredGridSpace(WorldBlockGrid.CellSize);
            var count = random.Next(thing.InstancesPerBlock.Minimum, thing.InstancesPerBlock.Maximum + 1);

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
                        PlaceDynamicThing(thing, finalCol, finalRow);
                        placed = true;
                    }
                }
            }
        }

        // DistributeWithNoiseMap
        private void DistributeWithNoiseMap(GameThing thing, int seed)
        {
            if (thing.InstancesPerBlock.IsEmpty)
                return;

            var targetGrid = thing.IsWalkAreaHole ? mainGrid : decorationGrid;
            Size sizeInCells = thing.GetRequiredGridSpace(WorldBlockGrid.CellSize);
            float noiseThreshold = 0.2f;
            int attempts = 100;

            for (int i = 0; i < attempts; i++)
            {
                if (!targetGrid.TryReserveSpace(sizeInCells, out int col, out int row))
                    break;

                float noise = GetNoise(col, row, seed);
                if (noise > noiseThreshold)
                    continue;

                PlaceDynamicThing(thing, col, row);
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

            h ^= (uint)salt * 0x9E3779B9; // número dorado (Knuth)
            h ^= h >> 16;
            h *= 0x85EBCA6B;
            h ^= h >> 13;
            h *= 0xC2B2AE35;
            h ^= h >> 16;

            return (int)h;
        }

        // PlaceDynamicThing
        private void PlaceDynamicThing(GameThing thing, int col, int row)
        {
            var targetGrid = thing.IsWalkAreaHole ? mainGrid : decorationGrid;
            var instance = CreateDynamicThing(thing.StaticName);
            Vector2 worldPosition = targetGrid.GetWorldPosition(col, row);
            instance.Position = worldPosition + Position;
            instance.Y += instance.BoundingBox.Height;
            instance.X += instance.BoundingBox.Width / 2;
            proceduralThings.Add(instance);
        }

        // Populate
        private void Populate()
        {
            foreach (var phase in Enum.GetValues<PlacementPhase>())
            {
                if (phase == PlacementPhase.None)
                    continue;

                foreach (var thing in Session.GetStaticThings(phase))
                {
                    if (thing.WorldVersion > WorldVersion)
                        continue;

                    if (!thing.IsAvailable(this, random))
                        continue;

                    switch (thing.DistributionStrategy)
                    {
                        // RandomCell
                        case PlacementDistributionStrategy.Random:
                            DistributeRandomly(thing);
                            break;

                        // Clump
                        case PlacementDistributionStrategy.Clump:
                            DistributeClumped(thing);
                            break;

                        // NoiseMap
                        case PlacementDistributionStrategy.NoiseMap:
                            DistributeWithNoiseMap(thing, randomSeed);
                            break;
                    }
                }
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            for (int i = 0; i < borderImages.Length; i++)
            {
                if (borders[i])
                    borderImages[i].Draw(gameTime);
            }
        }

        // OnInvalidate
        protected override void OnInvalidate()
        {
            base.OnInvalidate();

            borders[0] = GetNeighbor(EngendroAdventure.Direction.Up) == null;
            borderImages[0].Position = BoundingBox.GetPoint(RectanglePoint.Top);

            borders[1] = GetNeighbor(EngendroAdventure.Direction.Right) == null;
            borderImages[1].Position = BoundingBox.GetPoint(RectanglePoint.Right);

            borders[2] = GetNeighbor(EngendroAdventure.Direction.Down) == null;
            borderImages[2].Position = BoundingBox.GetPoint(RectanglePoint.Bottom);

            borders[3] = GetNeighbor(EngendroAdventure.Direction.Left) == null;
            borderImages[3].Position = BoundingBox.GetPoint(RectanglePoint.Left);
        }

        #endregion

        // Expand
        public WorldBlock? Expand(Direction direction)
        {
            if (GetNeighbor(direction) != null)
                return null;

            var newCell = WorldGridPosition;

            if (direction == EngendroAdventure.Direction.Down)
                newCell.Y += 1;

            else if (direction == EngendroAdventure.Direction.Left)
                newCell.X -= 1;

            else if (direction == EngendroAdventure.Direction.Right)
                newCell.X += 1;

            else if (direction == EngendroAdventure.Direction.Up)
                newCell.Y -= 1;

            return Manager.AddBlock(newCell, Session.WorldVersion);
        }

        // GetNeighbor
        public WorldBlock? GetNeighbor(Direction direction)
        {
            var pos = WorldGridPosition;

            if (direction == EngendroAdventure.Direction.Down)
            {
                pos.Y += 1;
                return Manager.GetBlockFromGrid(pos);
            }

            if (direction == EngendroAdventure.Direction.Left)
            {
                pos.X -= 1;
                return Manager.GetBlockFromGrid(pos);
            }

            if (direction == EngendroAdventure.Direction.Right)
            {
                pos.X += 1;
                return Manager.GetBlockFromGrid(pos);
            }

            if (direction == EngendroAdventure.Direction.Up)
            {
                pos.Y -= 1;
                return Manager.GetBlockFromGrid(pos);
            }

            return null;
        }

        // Index
        public int Index { get; }

        // Manager
        public WorldManager Manager { get; }

        // ProceduralThings
        public ReadOnlyCollection<GameThing> ProceduralThings { get; }

        // Tags
        public ReadOnlyCollection<WorldBlockTag> Tags { get; }

        // ToString
        public override string ToString() => $"{nameof(WorldBlock)} ({WorldGridPosition.X},{WorldGridPosition.Y})";

        // WorldGridPosition
        public Point WorldGridPosition { get; }
    }
}
