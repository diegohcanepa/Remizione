using Microsoft.Xna.Framework;

namespace Remizione
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
        public float Timer { get; private set; }

        // Update
        public void Update(GameTime gameTime)
        {
            if (Timer >= 0)
            {
                Timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (Timer >= Definition.Cooldown)
                {
                    if (manager.Session.Player is Actor player)
                        EffectDescriptor.Apply(Definition.EffectDescriptors, player, player, EffectContext.RunModifier);

                    ResetTimer();
                }
            }
        }
    }
}
