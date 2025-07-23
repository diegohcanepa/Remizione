using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;

namespace Remizione
{
    /// <summary>
    /// ProceduralRoom
    /// </summary>
    public sealed class ProceduralRoom : GameRoom
    {
        #region Private fields

        private const string ProcStates = "ProcStates";

        private RoomGrid? decorationGrid;
        private RoomGrid? mainGrid;
        private readonly Dictionary<string, PlacementData> placementDataDictionary = [];
        private readonly List<GameThing> proceduralThings = [];
        private readonly Random random;
        private readonly int randomSeed;

        #endregion

        #region Constructor

        // Constructor
        public ProceduralRoom(GameSession session, string name)
            : base(session, name)
        {
            LightingSystem = true;

            this.ProceduralThings = new(proceduralThings);
            this.randomSeed = GetSeed(Session.RandomSeed, Session.Level);
            this.random = new Random(randomSeed);
        }

        #endregion

        #region Private members

        // CreateDynamicThing
        private GameThing CreateDynamicThing(string staticName)
        {
            if (Session.CreateDynamicThing(staticName, $"{staticName}*{Name}_{proceduralThings.Count}") is not GameThing result)
                throw new InvalidOperationException($"Failed to create dynamic thing '{staticName}'.");

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

            Size sizeInCells = thing.GetRequiredGridSpace(RoomGrid.CellSize);

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
        private void DistributeRandomly(GameThing thing, PlacementData placementData)
        {
            if (mainGrid == null || decorationGrid == null)
                return;

            var targetGrid = thing.IsWalkAreaHole ? mainGrid : decorationGrid;
            Size sizeInCells = thing.GetRequiredGridSpace(RoomGrid.CellSize);
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
                        PlaceDynamicThing(thing, finalCol, finalRow);
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
            Size sizeInCells = thing.GetRequiredGridSpace(RoomGrid.CellSize);
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

            h ^= (uint)salt * 0x9E3779B9; // golden number (Knuth)
            h ^= h >> 16;
            h *= 0x85EBCA6B;
            h ^= h >> 13;
            h *= 0xC2B2AE35;
            h ^= h >> 16;

            return (int)h;
        }

        // GetStaticThings
        private IEnumerable<GameThing> GetStaticThings(PlacementPhase phase)
        {
            foreach (var entity in Session.Entities)
            {
                if (entity is not GameThing gameThing)
                    continue;

                if (placementDataDictionary.TryGetValue(gameThing.StaticName, out var placementData))
                {
                    if (placementData.Phase == phase)
                        yield return gameThing;
                }
            }
        }

        // PlaceDynamicThing
        private void PlaceDynamicThing(GameThing thing, int col, int row)
        {
            if (mainGrid == null || decorationGrid == null)
                return;

            var targetGrid = thing.IsWalkAreaHole ? mainGrid : decorationGrid;
            var instance = CreateDynamicThing(thing.StaticName);
            instance.Position = targetGrid.GetPosition(col, row);
            instance.Y += instance.BoundingBox.Height;
            instance.X += instance.BoundingBox.Width / 2;
            proceduralThings.Add(instance);
            Children.Add(instance);
        }

        // Populate
        private void Populate()
        {
            foreach (var phase in Enum.GetValues<PlacementPhase>())
            {
                if (phase == PlacementPhase.None)
                    continue;

                foreach (var thing in GetStaticThings(phase))
                {
                    if (thing.WorldVersion > Session.WorldVersion)
                        continue;

                    if (!placementDataDictionary.TryGetValue(thing.StaticName, out var placementData))
                        continue;

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

        #endregion

        #region Protected members

        /*
        // OnInitialize
        protected override void OnInitialize()
        {
            base.OnInitialize();

            if (Session.IsNewSession)
            {
                var initialBlock = WorldManager.AddBlock(new(WorldManager.GridSize / 2), Session.WorldVersion, !PreserveFirstBlock);
                if (initialBlock.Light != null)
                    initialBlock.LightPosition = new Vector2(120, 50);
                WorldManager.Blocks[0].Expand(EngendroAdventure.Direction.Up);
            }
            else
            {
                for (var i = 0; i < worldBlockData.Count; i++)
                {
                    var populate = i > 0 || !PreserveFirstBlock;
                    WorldManager.AddBlock(worldBlockData[i].gridPosition, worldBlockData[i].worldVersion, populate);

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

                WorldManager.Blocks[0].Expand(EngendroAdventure.Direction.Up);

                worldBlockData.Clear();
            }

            Regenerate();
        }
        */

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();

            this.decorationGrid = new RoomGrid("Decoration", Width, Height);
            this.mainGrid = new RoomGrid("Main", Width, Height);

            Populate();
        }

        // OnRead
        protected override void OnRead(XmlAttributeCollection attributes)
        {
            if (attributes[ProcStates]?.Value is string stateData)
            {
                var thingStates = new Dictionary<string, int>();
                var states = stateData.Split(',');
                foreach (var state in states)
                {
                    var values = state.Split('=');
                    thingStates[values[0]] = int.Parse(values[1]);
                }
            }
        }

        // OnWrite
        protected override void OnWrite(XmlWriter output)
        {
            var stateData = new List<string>();

            // Collect state data for procedural things
            stateData.Clear();
            foreach (var thing in ProceduralThings)
            {
                if (thing.StateID != 0)
                    stateData.Add($"{thing.Name}={thing.StateID}");
            }

            if (stateData.Count > 0)
            {
                var value = string.Join(",", stateData);
                output.WriteAttributeString(ProcStates, value);
            }
        }

        #endregion

        #region Internal members

        // AddPlacementData
        internal void AddPlacementData(string staticName, PlacementData placementData)
        {
            if (placementDataDictionary.ContainsKey(staticName))
                throw new InvalidOperationException($"Placement info with name '{staticName}' already exists.");

            placementDataDictionary.Add(staticName, placementData);
        }

        #endregion

        // CanPlaceDynamicPropAt
        public bool CanPlaceDynamicPropAt(IsometricProp prop, Vector2 position)
        {
            if (prop.Collider != null)
            {
                var box = new RectangleF(position, prop.Collider.BoundingRectangleF.Size);

                for (int i = 0; i < CulledThings.Count; i++)
                {
                    if (CulledThings[i] == prop)
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

        // PlaceDynamicProp
        public IsometricProp? PlaceDynamicProp(IsometricProp prop, Vector2 position)
        {
            IsometricProp? result = null;

            if (CanPlaceDynamicPropAt(prop, position))
            {
                result = Session.CreateDynamicThing(prop.StaticName, string.Empty) as IsometricProp;
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

        // ProceduralThings
        public ReadOnlyCollection<GameThing> ProceduralThings { get; }
    }
}
