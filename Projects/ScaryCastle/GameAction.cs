using Engendro.Audio;
using System;
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
        private readonly Action? onConsume;

        // Constructor
        public GameAction(string animationName, ItemUsageScope usageScope, IList<EffectDescriptor> effectDescriptors, Action? onConsume = null)
        {
            this.AnimationName = animationName;
            this.UsageScope = usageScope;
            this.EffectDescriptors = new(effectDescriptors);
            this.onConsume = onConsume;
        }

        #region IGameAction explicit members

        // OnConsume
        void IGameAction.Consume() => onConsume?.Invoke();

        #endregion

        // AnimationName
        public string AnimationName { get; }

        // Apply
        public static void Apply(IGameAction gameAction, GameThing source, GameThing? target, EffectContext context)
        {
            // Play sound
            if (gameAction.Sound != null)
                source.PlaySound(gameAction.Sound);

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

        // AreaOfEffect
        public int AreaOfEffect { get; init; }

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
