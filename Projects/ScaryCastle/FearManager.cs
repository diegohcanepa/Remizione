using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    // FearManager
    // Encapsula la lógica de miedo y la cuenta regresiva de muerte (síncope)
    public sealed class FearManager
    {
        private readonly GameSession session;

        // Constructor
        public FearManager(GameSession session)
        {
            this.session = session;
            Reset(0);
        }

        // AddFear
        public void AddFear(int amount)
        {
            if (amount == 0)
                return;

            int previousFear = CurrentFear;
            CurrentFear = Math.Clamp(CurrentFear + amount, 0, MaximumFear);

            // Si el miedo bajó, detenemos la amenaza de muerte inmediatamente
            if (CurrentFear < previousFear)
            {
                RemainingDeathTime = GameSettings.DeathCoooldown;
                IsDeadByFear = false;
            }
        }

        // CurrentFear
        public int CurrentFear { get; private set; }

        // IsDeadByFear
        public bool IsDeadByFear { get; private set; }

        // MaximumFear
        public int MaximumFear { get; private set; }

        // RemainingDeathTime
        public float RemainingDeathTime { get; private set; }

        // Reset
        public void Reset(int maximumFear)
        {
            CurrentFear = 0;
            IsDeadByFear = false;
            MaximumFear = maximumFear;
            RemainingDeathTime = GameSettings.DeathCoooldown;
        }

        // Update
        public void Update(GameTime gameTime)
        {
            if (IsDeadByFear || CurrentFear < MaximumFear || session.IsAwaiting)
                return;

            // Reducción del timer basada en segundos para evitar errores de precisión de int
            float elapsed = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            RemainingDeathTime -= elapsed;

            if (RemainingDeathTime <= 0)
            {
                RemainingDeathTime = 0;
                IsDeadByFear = true;
            }
        }
    }
}