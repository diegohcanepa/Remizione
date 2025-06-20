using Engendro;
using Engendro.Audio;
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
        private const string EmptyList = "[none]";
        private const string WorldBlocksAttributeName = "WorldBlocks";
        private readonly List<(Point gridPosition, int worldVersion, Dictionary<string, int> states)> worldBlockData = [];

        // Constructor
        public ProceduralRoom(GameSession session, string name)
            : base(session, name)
        {
            LightingSystem = true;
            WorldManager = new WorldManager(session, new Size(Screen.NativeWidth, Screen.NativeHeight), 111);
        }

        #region Private members

        // Regenerate
        private void Regenerate()
        {
            var removeList = new List<GameThing>();
            for (var i = 0; i < Children.Count; i++)
            {
                if (Children[i] is GameThing thing && thing.PlacementPhase != PlacementPhase.None)
                    removeList.Add(thing);
            }

            for (var i = 0; i < removeList.Count; i++)
            {
                removeList[i].Unparent();
            }

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
                
                foreach (var thing in WorldManager.Blocks[i].ProceduralThings)
                {
                    if (thing.StateID >= 0)
                        Children.Add(thing);
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

            WorldManager.BeginUpdate();
            if (Session.IsNewSession)
            {
                var initialBlock = WorldManager.AddBlock(new(WorldManager.GridSize / 2), Session.WorldVersion, FirstBlockReservedSpace);
                if (initialBlock.Light != null)
                    initialBlock.LightPosition = new Vector2(120,50);
            }
            else
            {
                for (var i = 0; i < worldBlockData.Count; i++)
                {
                    WorldManager.AddBlock(worldBlockData[i].gridPosition, worldBlockData[i].worldVersion);

                    foreach (var keyValue in worldBlockData[i].states)
                    {
                        var thing = Session.GetEntity<GameThing>(keyValue.Key);
                        if (thing != null)
                        {
                            thing.StateID = keyValue.Value;
                            if (thing.StateID < 0)
                                thing.Unparent();
                        }
                    }
                }

                worldBlockData.Clear();
            }
            WorldManager.EndUpdate();

            Regenerate();
        }

        // OnRead
        protected override void OnRead(XmlAttributeCollection attributes)
        {
            // World blocks
            if (attributes[WorldBlocksAttributeName]?.Value is string worldBlocksValue)
            {
                var list = worldBlocksValue.Split(';');

                foreach (var item in list)
                {
                    var blockData = item.Split(':');
                    var gridPosition = XmlConverterExtension.ToPoint(blockData[0]);
                    var worldVersion = int.Parse(blockData[1]);
                    var thingStates = new Dictionary<string, int>();

                    if (blockData[2] != EmptyList)
                    {
                        var states = blockData[2].Split(',');
                        foreach (var state in states)
                        {
                            var values = state.Split('=');
                            thingStates[values[0]] = int.Parse(values[1]);
                        }
                    }

                    worldBlockData.Add((gridPosition, worldVersion, thingStates));
                }
            }
        }

        // OnWrite
        protected override void OnWrite(XmlWriter output)
        {
            var data = new List<string>();
            var stateData = new List<string>();

            foreach (var block in WorldManager.Blocks)
            {
                // Collect state data for procedural things in block
                stateData.Clear();
                foreach (var thing in block.ProceduralThings)
                {
                    if (thing.StateID != 0)
                        stateData.Add($"{thing.Name}={thing.StateID}");
                }

                var stateDataValue = stateData.Count == 0 ? "[none]" : string.Join(",", stateData);
                var value = $"{block.WorldGridPosition.X},{block.WorldGridPosition.Y}:{block.WorldVersion}:{stateDataValue}";
                data.Add(value);
            }

            var attrValue = string.Join(";", data);
            output.WriteAttributeString(WorldBlocksAttributeName, attrValue);
        }

        #endregion

        // BlockSize
        [ScriptProperty]
        public Size BlockSize { get; set; } = new Size(Screen.NativeWidth, Screen.NativeHeight);

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

        // FirstBlockReservedSpace
        [ScriptProperty]
        public Rectangle FirstBlockReservedSpace { get; set; }

        // WorldManager
        public WorldManager WorldManager { get; }
    }
}
