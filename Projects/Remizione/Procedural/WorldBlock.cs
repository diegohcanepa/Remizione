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
        private readonly WorldBlockGrid grid;
        private readonly List<Prop> props = [];
        private readonly List<WorldBlockTag> tags = [];

        #region Constructor

        // Constructor
        public WorldBlock(WorldManager manager, Point worldGridPosition)
            : base(manager.Session, string.Empty)
        {
            this.grid = new WorldBlockGrid(this);
            this.Props = new(props);
            this.Tags = new(tags);

            AssignTags();

            Index = manager.Blocks.Count;
            Atlas = Atlases.Environment;
            Manager = manager;
            WorldGridPosition = worldGridPosition;
            PivotOrigin = Engendro.RectanglePoint.LeftTop;
            RenderLayer = RenderLayer.Background;
            Position = new(worldGridPosition.X * BlockWidth, worldGridPosition.Y * BlockHeight);
            DefaultImageName = "TerrainBlockDefault";
          

            if (CreateDynamicProp("CrossLargeA") is Prop prop)
            {
                prop.Position = BoundingBox.Center;
                props.Add(prop);
            }
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
            var result = Session.CreateDynamicThing(staticName, $"{staticName}*{Index}_{props.Count}") as Prop;
            if (result == null)
                throw new InvalidOperationException($"Failed to create dynamic prop '{staticName}'.");

            return result;
        }

        // GetAvailableProps
        private List<Prop> GetAvailableProps()
        {
            var result = new List<Prop>();
            
            for (int i = 0; i < Session.AllStaticProps.Count; i++)
            {
                if (Session.AllStaticProps[i].IsAvailable(this))
                    result.Add(Session.AllStaticProps[i]);
            }
            return result;
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

        // Tags
        public ReadOnlyCollection<WorldBlockTag> Tags { get; }

        // WorldGridPosition
        public Point WorldGridPosition { get; }
    }
}
