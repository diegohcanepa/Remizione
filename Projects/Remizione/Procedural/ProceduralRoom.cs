using Engendro;
using Engendro.Audio;
using EngendroAdventure;
using EngendroAdventure.Scripting;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Xml;

namespace Remizione
{
    /// <summary>
    /// ProceduralRoom
    /// </summary>
    public sealed class ProceduralRoom : GameRoom
    {
        private enum AttributeName { WorldBlocks };
        private readonly List<(Point gridPosition, int worldVersion)> worldBlockData = [];

        // Constructor
        public ProceduralRoom(GameSession session, string name)
            : base(session, name)
        {
            AtlasName = string.Empty;
            LightingSystem = true;
            WorldManager = new WorldManager(session, new Size(Screen.NativeWidth, Screen.NativeHeight), 111);
        }

        #region Private members

        // Regenerate
        private void Regenerate()
        {
            Children.Clear();
            RemoveWalkArea("");

            CustomWidth = WorldManager.GridSize * WorldManager.BlockSize.Width;
            CustomHeight = WorldManager.GridSize * WorldManager.BlockSize.Height;

            // Define walk area
            var vertices = WorldManager.GetWalkareaVertices();
            AddWalkArea("", vertices);

            // Add existing blocks
            for (var i = 0; i < WorldManager.Blocks.Count; i++)
            {
                Children.Add(WorldManager.Blocks[i]);
                foreach (var prop in WorldManager.Blocks[i].Things)
                {
                    Children.Add(prop);
                }
            }

            if (Session.Player != null)
                Children.Add(Session.Player);
        }

        #endregion

        #region Protected members

        // OnInitialize
        protected override void OnInitialize()
        {
            base.OnInitialize();

            if (Session.IsNewSession)
            {
                WorldManager.BeginUpdate();
                var origin = WorldManager.AddBlock(new(WorldManager.GridSize / 2), Session.WorldVersion);

                origin.Expand(EngendroAdventure.Direction.Up);
                origin.Expand(EngendroAdventure.Direction.Down);
                origin.Expand(EngendroAdventure.Direction.Left);
                origin.Expand(EngendroAdventure.Direction.Right);
                WorldManager.EndUpdate();
                Regenerate();
            }
            else
            {
                WorldManager.BeginUpdate();
                for (var i = 0; i < worldBlockData.Count; i++)
                {
                    WorldManager.AddBlock(worldBlockData[i].gridPosition, worldBlockData[i].worldVersion);
                }
                WorldManager.EndUpdate();
            }

            Regenerate();
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();

            if (Session.Player != null)
            {
                Session.Player.Position = WorldManager.Blocks[0].BoundingBox.Center;
                Session.Camera.FollowTarget(Session.Player);
            }
        }

        // OnRead
        protected override void OnRead(XmlAttributeCollection attributes)
        {
            // World blocks
            if (attributes[AttributeName.WorldBlocks.ToString()]?.Value is string worldBlocksValue)
            {
                var list = worldBlocksValue.Split(';');

                foreach (var item in list)
                {
                    var blockData = item.Split(':');
                    var gridPosition = XmlConverterExtension.ToPoint(blockData[0]);
                    var worldVersion = int.Parse(blockData[1]);
                    worldBlockData.Add((gridPosition, worldVersion));
                }
            }
        }

        // OnWrite
        protected override void OnWrite(XmlWriter output)
        {
            var blockData = new List<string>();

            foreach (var block in WorldManager.Blocks)
            {
                var value = $"{block.WorldGridPosition.X},{block.WorldGridPosition.Y}:{block.WorldVersion}";
                blockData.Add(value);
            }

            var attrValue = string.Join(";", blockData);

            output.WriteAttributeString(AttributeName.WorldBlocks.ToString(), attrValue);
        }

        #endregion

        // Expand
        public bool Expand(Vector2 playerPosition, EngendroAdventure.Direction direction)
        {
            if (WorldManager.GetBlockFromScreen(playerPosition) is WorldBlock currentBlock)
            {
                WorldManager.BeginUpdate();
                currentBlock.Expand(direction);
                WorldManager.EndUpdate();
                Regenerate();

                return true;
            }

            return false;
        }

        // TerrainSound
        [ScriptProperty]
        public Sound? TerrainSound { get; set; }

        // WorldManager
        public WorldManager WorldManager { get; }
    }
}
