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
        public Run(GameSession session, int maxStages)
        {
            this.session = session;
            this.MaxStages = maxStages;
        }

        // CleanUpCurrentStage
        private void CleanUpCurrentStage()
        {
            if (CurrentStage == null)
                return;

            foreach (var r in CurrentStage.RoomGraphs)
            {
                r.RideRoom?.Children.Clear();
            }

            session.CleanUpRuntimeEntities();

            CurrentStage = null;
        }

        // CurrentStage
        public Stage? CurrentStage { get; private set; }

        // CurrentStageIndex
        public int CurrentStageIndex => CurrentStage?.Index ?? 0;

        // Dispose
        public void Dispose()
        {
            if (IsDisposed)
                return;

            CleanUpCurrentStage();
            IsDisposed = true;
        }

        // HasContent
        public bool HasContent => CurrentStage?.RoomGraphs.Count > 0;

        // Intensity
        public float Intensity
        {
            get
            {
                if (MaxStages <= 1)
                    return 0;
                
                float progress = (float)CurrentStageIndex / MaxStages;
                
                return (float)Math.Pow(Math.Clamp(progress, 0f, 1f), 1.2f);
            }
        }

        // IsDisposed
        public bool IsDisposed { get; private set; }

        // LoadNextStage
        public bool LoadNextStage(Tags pools)
        {
            ObjectDisposedException.ThrowIf(IsDisposed, this);

            if (CurrentStageIndex == MaxStages)
                return false;

            var nextIndex = CurrentStageIndex + 1;

            CleanUpCurrentStage();

            var builder = new StageBuilder();
            CurrentStage = builder.Build(session, nextIndex, pools, Spawns);

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
            foreach (var r in CurrentStage.RoomGraphs)
            {
                r.RideRoom = RideRoom.CreateInstance(session, r);
            }

            foreach (var r in CurrentStage.RoomGraphs)
            {
                r.RideRoom.Load();
            }

            return true;
        }

        // MaxFloors
        public int MaxStages { get; }

        // Spawns
        public CounterBank Spawns { get; } = new();
    }
}