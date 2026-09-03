using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// Run
    /// </summary>
    public sealed class Run
    {
        // Constructor
        public Run(GameSession session, int seed, RunDefinition definition)
        {
            this.Session = session;
            this.Seed = seed;
            this.Definition = definition;

            this.LootGenerator = new(this);

            this.MasterRunRng = new(seed);
            this.VolatileRng = new(seed);

            this.PlayerInventory = new ItemContainer(session);
            this.PocketItems = new PocketItemManager(session);
            this.Modifiers = new RunModifierManager(session);
        }

        // Definition
        public RunDefinition Definition { get; }

        // FloorDescriptor
        public FloorDescriptor? FloorDescriptor { get; private set; }

        // FloorIndex
        public int FloorIndex { get; private set; } = -1;

        // LootGenerator
        public LootGenerator LootGenerator { get; }

        // MasterRunRng
        public Random MasterRunRng { get; }

        // Modifiers
        public RunModifierManager Modifiers { get; }

        // PlayerInventory
        public ItemContainer PlayerInventory { get; }

        // PocketItems
        public PocketItemManager PocketItems { get; }

        // Progress
        public float Progress => (FloorIndex + 1f) / Definition.Floors.Count;

        // Seed
        public int Seed { get; }

        // Session
        public GameSession Session { get; }

        // Spawns
        public CounterBank Spawns { get; } = new();

        // Traits
        public TraitCollection Traits { get; } = [];

        // TryGenerateNextFloor
        public bool TryGenerateNextFloor(out ProceduralRoom? startRoom)
        {
            if (FloorIndex >= Definition.Floors.Count - 1)
            {
                startRoom = null;
                return false;
            }

            FloorIndex++;
            FloorDescriptor = Definition.Floors[FloorIndex];

            var floorGenerator = new FloorGenerator();
            var floorLayout = floorGenerator.Generate(this, FloorDescriptor);
            startRoom = floorLayout.StartNode.Room;
            return true;
        }

        // Update
        public void Update(GameTime gameTime)
        {
            Modifiers.Update(gameTime);
        }

        // VolatileRng
        public Random VolatileRng { get; }
    }
}