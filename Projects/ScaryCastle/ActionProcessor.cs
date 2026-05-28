using System.Linq;

namespace ScaryCastle
{
    /// <summary>
    /// ActionProcessor
    /// </summary>
    public static class ActionProcessor
    {
        // Apply
        public static void Apply(IAction gameAction, GameThing source, GameThing? target, EffectContext context)
        {
            // Play sound
            if (gameAction.SoundTrigger != null)
                source.PlaySound(gameAction.SoundTrigger);

            if (gameAction.AreaOfEffect == 0)
            {
                EffectDescriptor.Apply(gameAction.EffectDescriptors, source, target, context);
            }
            else if (source.Room is GameRoom room)
            {
                foreach (var potentialTarget in room.Children.OfType<GameThing>())
                {
                    if (source.DistanceTo(potentialTarget) < gameAction.AreaOfEffect)
                        EffectDescriptor.Apply(gameAction.EffectDescriptors, source, potentialTarget, context);
                }
            }
        }
    }
}
