using Engendro.Audio;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ScaryCastle
{
    /// <summary>
    /// GameAction
    /// </summary>
    public sealed class GameAction : IGameAction
    {
        // Constructor
        public GameAction(string animationName, ItemUsageScope usageScope, IList<EffectDescriptor> effectDescriptors)
        {
            this.AnimationName = animationName;
            this.UsageScope = usageScope;
            this.EffectDescriptors = new(effectDescriptors);
        }

        // AnimationName
        public string AnimationName { get; }

        // Apply
        public static void Apply(IGameAction gameAction, GameThing source, GameThing? target, EffectContext context)
        {
            // Play sound
            if (gameAction.Sound != null)
                source.PlaySound(gameAction.Sound);

            if (gameAction.AreaRange == 0)
            {
                EffectDescriptor.Apply(gameAction.EffectDescriptors, source, target, context);
            }
            else if (source.Room is GameRoom room)
            {
                foreach (var potentialTarget in room.Children.OfType<GameThing>())
                {
                    if (source.DistanceTo(potentialTarget) < gameAction.AreaRange)
                        EffectDescriptor.Apply(gameAction.EffectDescriptors, source, potentialTarget, context);
                }
            }
        }

        // AreaRange
        public int AreaRange { get; init; }

        // EffectDescriptors
        public ReadOnlyCollection<EffectDescriptor> EffectDescriptors { get; }

        // InPlaceEffectType
        public InPlaceEffectType InPlaceEffectType { get; init; }

        // Projectile
        public ProjectileDescriptor? Projectile { get; init; }

        // Sound
        public Sound? Sound { get; init; }

        // UsageScope
        public ItemUsageScope UsageScope { get; }
    }
}
