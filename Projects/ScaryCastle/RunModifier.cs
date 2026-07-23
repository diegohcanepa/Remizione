using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// RunModifier
    /// </summary>
    public sealed class RunModifier
    {
        private readonly RunModifierManager manager;

        // Constructor
        public RunModifier(RunModifierManager manager, RunModifierDefinition definition)
        {
            this.manager = manager;
            this.Definition = definition;
        }

        // Definition
        public RunModifierDefinition Definition { get; }

        // Name
        public string Name => Definition.Name;

        // ResetTimer
        public void ResetTimer()
        {
            Timer = 0;
        }

        // Timer
        public int Timer { get; private set; }

        // Update
        public void Update(GameTime gameTime)
        {
            if (Timer >= 0)
            {
                Timer += gameTime.ElapsedGameTime.Milliseconds;
                if (Timer >= Definition.Cooldown)
                {
                    ResetTimer();
                }
            }
        }
    }
}
