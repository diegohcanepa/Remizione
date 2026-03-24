using Engendro;
using System;

namespace ScaryCastle
{
    /// <summary>
    ///  Run
    /// </summary>
    public sealed class Run : IDisposable
    {
        private readonly GameSession session;

        // Constructor
        public Run(GameSession session, int maxFloors)
        {
            this.session = session;
            this.MaxFloors = maxFloors;
        }

        // CleanUpCurrentFloor
        private void CleanUpCurrentFloor()
        {
            if (CurrentFloor == null)
                return;

            foreach (var r in CurrentFloor.RoomGraphs)
            {
                r.RideRoom?.Children.Clear();
            }

            session.CleanUpRuntimeEntities();

            CurrentFloor = null;
        }

        // CurrentFloor
        public Floor? CurrentFloor { get; private set; }

        // CurrentFloorIndex
        public int CurrentFloorIndex => CurrentFloor?.Index ?? 0;

        // Dispose
        public void Dispose()
        {
            if (IsDisposed)
                return;

            CleanUpCurrentFloor();
            IsDisposed = true;
        }

        // HasContent
        public bool HasContent => CurrentFloor?.RoomGraphs.Count > 0;

        // Intensity
        public float Intensity
        {
            get
            {
                if (MaxFloors <= 1)
                    return 0;
                
                float progress = (float)CurrentFloorIndex / MaxFloors;
                
                return (float)Math.Pow(Math.Clamp(progress, 0f, 1f), 1.2f);
            }
        }

        // IsDisposed
        public bool IsDisposed { get; private set; }

        // LoadNextFloor
        public bool LoadNextFloor(Tags pools)
        {
            ObjectDisposedException.ThrowIf(IsDisposed, this);

            if (CurrentFloorIndex == MaxFloors)
                return false;

            var nextIndex = CurrentFloorIndex + 1;

            CleanUpCurrentFloor();

            var builder = new FloorBuilder();
            CurrentFloor = builder.Build(session, nextIndex, pools, Spawns);

            // Commit floor spawns
            foreach (var name in builder.Spawns.GetNames())
            {
                int count = builder.Spawns.GetCount(name);
                for (int i = 0; i < count; i++)
                {
                    Spawns.Increment(name);
                }
            }

            // Build rooms
            foreach (var r in CurrentFloor.RoomGraphs)
            {
                r.RideRoom = RideRoom.CreateInstance(session, r);
            }

            foreach (var r in CurrentFloor.RoomGraphs)
            {
                r.RideRoom.Load();
            }

            return true;
        }

        // MaxFloors
        public int MaxFloors { get; }

        // Spawns
        public CounterBank Spawns { get; } = new();
    }
}