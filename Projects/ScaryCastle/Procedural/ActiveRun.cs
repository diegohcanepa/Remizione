using Engendro;
using System;

namespace ScaryCastle
{
    /// <summary>
    ///  ActiveRun
    /// </summary>
    public sealed class ActiveRun : IDisposable
    {
        private readonly GameSession session;

        // ActiveRun
        public ActiveRun(GameSession session)
        {
            this.session = session;
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

        // IsDisposed
        public bool IsDisposed { get; private set; }

        // LoadNextFloor
        public void LoadNextFloor(Tags pools)
        {
            ObjectDisposedException.ThrowIf(IsDisposed, this);

            var nextIndex = CurrentFloorIndex + 1;

            CleanUpCurrentFloor();

            var builder = new FloorBuilder();
            CurrentFloor = builder.Build(session, nextIndex, pools, RunSpawns);

            // Commit floor spawns
            foreach (var name in builder.Spawns.GetNames())
            {
                int count = builder.Spawns.GetCount(name);
                for (int i = 0; i < count; i++)
                {
                    RunSpawns.Increment(name);
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
        }

        // RunSpawns
        public MultiCounter RunSpawns { get; } = new();
    }
}