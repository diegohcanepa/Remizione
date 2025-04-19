using Engendro;
using EngendroAdventure;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;

namespace Remizione
{
    /// <summary>
    /// WorldBlock
    /// </summary>
    public class WorldBlock : Prop
    {
        private readonly WorldBlockGrid grid;
        private readonly List<Prop> props = [];
        private readonly List<WorldBlockTag> tags = [];

        #region Constructor

        // Constructor
        public WorldBlock(WorldManager manager, Point worldGridPosition, bool isNew)
            : base(manager.Session, string.Empty)
        {
            this.grid = new WorldBlockGrid(manager.BlockSize);
            this.Props = new(props);
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

            if (isNew)
                Populate();
        }

        #endregion

        #region Private fields

        // AssignTags
        private void AssignTags()
        {
        }

        // CreateDynamicProp
        private Prop CreateDynamicProp(string staticName)
        {
            if (Session.CreateDynamicThing(staticName, $"{staticName}*{Index}_{props.Count}") is not Prop result)
                throw new InvalidOperationException($"Failed to create dynamic prop '{staticName}'.");

            return result;
        }

        // DistributeClumped
        private void DistributeClumped(Prop prop)
        {
            if (prop.InstancesPerBlock.IsEmpty)
                return;

            int totalCount = prop.InstancesPerBlock.Random();
            int clumpSize = 3 + Session.Random.Next(3);
            int clumpCount = (totalCount + clumpSize - 1) / clumpSize;

            Size sizeInCells = prop.GetRequiredGridSpace(WorldBlockGrid.CellSize);

            for (int i = 0; i < clumpCount; i++)
            {
                if (!grid.TryReserveSpace(sizeInCells, out int baseCol, out int baseRow))
                    break;

                PlaceDynamicProp(prop, baseCol, baseRow);

                for (int j = 0; j < clumpSize - 1; j++)
                {
                    int offsetCol = baseCol + Session.Random.Next(-1, 2);
                    int offsetRow = baseRow + Session.Random.Next(-1, 2);

                    if (grid.TryReserveSpace(sizeInCells, out int col, out int row, offsetCol, offsetRow))
                        PlaceDynamicProp(prop, col, row);
                }
            }
        }

        // DistributeRandomly
        private void DistributeRandomly(Prop prop)
        {
            if (prop.InstancesPerBlock.IsEmpty)
                return;

            Size sizeInCells = prop.GetRequiredGridSpace(WorldBlockGrid.CellSize);

            for (int i = 0; i < prop.InstancesPerBlock.Random(); i++)
            {
                // Intentos limitados para evitar bucles infinitos si no hay espacio
                int maxAttempts = 20 + (sizeInCells.Width * sizeInCells.Height) * 2;
                bool placed = false;

                for (int attempt = 0; attempt < maxAttempts && !placed; attempt++)
                {
                    int col = Session.Random.Next(grid.ColCount - sizeInCells.Width + 1);
                    int row = Session.Random.Next(grid.RowCount - sizeInCells.Height + 1);

                    if (grid.TryReserveSpace(sizeInCells, out int finalCol, out int finalRow, col, row))
                    {
                        PlaceDynamicProp(prop, finalCol, finalRow);
                        placed = true;
                    }
                }
            }
        }

        // DistributeWithNoiseMap
        private void DistributeWithNoiseMap(Prop prop, int seed)
        {
            if (prop.InstancesPerBlock.IsEmpty)
                return;

            Size sizeInCells = prop.GetRequiredGridSpace(WorldBlockGrid.CellSize);
            float noiseThreshold = 0.2f;
            int attempts = 100;

            for (int i = 0; i < attempts; i++)
            {
                if (!grid.TryReserveSpace(sizeInCells, out int col, out int row))
                    break;

                float noise = GetNoise(col, row, seed);
                if (noise > noiseThreshold)
                    continue;

                PlaceDynamicProp(prop, col, row);
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

        // PlaceDynamicProp
        private void PlaceDynamicProp(Prop prop, int col, int row)
        {
            var instance = CreateDynamicProp(prop.StaticName);
            Vector2 worldPosition = grid.GetWorldPosition(col, row);
            instance.Position = worldPosition + Position;
            instance.Y += instance.BoundingBox.Height;
            instance.X += instance.BoundingBox.Width / 2;
            props.Add(instance);
        }

        // Populate
        private void Populate()
        {
            foreach (var phase in Enum.GetValues<PropInstantiationPhase>())
            {
                if (phase == PropInstantiationPhase.None)
                    continue;

                foreach (var prop in Session.GetStaticProps(phase))
                {
                    if (!prop.IsAvailable(this))
                        continue;

                    switch (prop.DistributionStrategy)
                    {
                        // RandomCell
                        case PropDistributionStrategy.RandomCell:
                            DistributeRandomly(prop);
                            break;

                        // Clump
                        case PropDistributionStrategy.Clump:
                            DistributeClumped(prop);
                            break;

                        // NoiseMap
                        case PropDistributionStrategy.NoiseMap:
                            DistributeWithNoiseMap(prop, Session.RandomSeed);
                            break;
                    }
                }
            }
        }

        #endregion

        #region Protected members

        // OnRead
        protected override void OnRead(XmlAttributeCollection attributes)
        {
        }

        // OnWrite
        protected override void OnWrite(XmlWriter output)
        {
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

            return Manager.AddBlock(newCell, true);
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

        // Props
        public ReadOnlyCollection<Prop> Props { get; }

        // Tags
        public ReadOnlyCollection<WorldBlockTag> Tags { get; }

        // WorldGridPosition
        public Point WorldGridPosition { get; }
    }
}
