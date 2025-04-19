using EngendroAdventure;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// WorldBlock
    /// </summary>
    public class WorldBlock : Prop
    {
        private readonly WorldBlockGrid grid;
        private readonly List<Prop> props = [];

        // Constructor
        public WorldBlock(WorldManager manager, Point worldGridPosition)
            : base(manager.Session, string.Empty)
        {
            Index = manager.Blocks.Count;
            RandomSeed = GetSeed(manager.RandomSeed, Index);
            Atlas = Atlases.Environment;
            Manager = manager;
            WorldGridPosition = worldGridPosition;
            PivotOrigin = Engendro.RectanglePoint.LeftTop;
            RenderLayer = RenderLayer.Background;
            Position = new(worldGridPosition.X * BlockWidth, worldGridPosition.Y * BlockHeight);
            DefaultImageName = "TerrainBlockDefault";
            this.grid = new WorldBlockGrid(this);

            this.Props = new(props);

            if (Session.CreateDynamicThing("CrossLargeA") is Prop prop)
            {
                prop.Position = BoundingBox.Center;
                props.Add(prop);
            }
        }

        #region Private fields

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

        #endregion

        // BlockHeight
        public const int BlockHeight = 135;

        // BlockWidth
        public const int BlockWidth = 240;

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

            return Manager.AddBlock(newCell);
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

        // RandomSeed
        public int RandomSeed { get; }

        // WorldGridPosition
        public Point WorldGridPosition { get; }
    }
}
