using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// RunState
    /// </summary>
    public sealed class RunState
    {
        // Constructor
        public RunState(GameSession session, int seed, RunDescriptor descriptor)
        {
            this.Session = session;
            this.Seed = seed;
            this.Descriptor = descriptor;

            this.LootGenerator = new(this);

            this.MasterRunRng = new(seed);
            this.VolatileRng = new(seed);

            this.PlayerInventory = new ItemContainer(session);
            this.PocketItems = new PocketItemManager(session);
            this.Modifiers = new RunModifierManager(session);
        }

        // Descriptor
        public RunDescriptor Descriptor { get; }

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

        // Seed
        public int Seed { get; }

        // Session
        public GameSession Session { get; }

        // Spawns
        public CounterBank Spawns { get; } = new();

        // Traits
        public PlayerTraits Traits { get; } = new();

        // TryGenerateNextFloor
        public bool TryGenerateNextFloor(out ProceduralRoom? startRoom)
        {
            if (FloorIndex >= Descriptor.Floors.Count - 1)
            {
                startRoom = null;
                return false;
            }

            FloorIndex++;
            FloorDescriptor = Descriptor.Floors[FloorIndex];

            var floorLayout = FloorGenerator.Generate(this, FloorDescriptor);
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