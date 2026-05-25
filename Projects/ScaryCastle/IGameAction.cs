using Engendro.Audio;
using System.Collections.ObjectModel;

namespace ScaryCastle
{
    /// <summary>
    /// IGameAction
    /// </summary>
    public interface IGameAction
    {
        // AnimationName
        string AnimationName { get; }

        // AreaOfEffect
        int AreaOfEffect { get; }

        // Consume
        void Consume();

        // EffectDescriptors
        ReadOnlyCollection<EffectDescriptor> EffectDescriptors { get; }

        // InPlaceEffectType
        InPlaceEffectType InPlaceEffectType { get; }

        // Projectile
        ProjectileDescriptor? Projectile { get; }

        // SoundStart
        Sound? SoundStart { get; }

        // SoundTrigger
        Sound? SoundTrigger { get; }

        // UsageScope
        ItemUsageScope UsageScope { get; }
    }
}
