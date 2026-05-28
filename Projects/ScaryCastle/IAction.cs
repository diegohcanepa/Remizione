using Engendro.Audio;
using System.Collections.ObjectModel;

namespace ScaryCastle
{
    /// <summary>
    /// IAction
    /// </summary>
    public interface IAction
    {
        // AnimationName
        string AnimationName { get; }

        // AreaOfEffect
        int AreaOfEffect { get; }

        // Consume
        void Consume(Actor actor);

        // EffectDescriptors
        ReadOnlyCollection<EffectDescriptor> EffectDescriptors { get; }

        // EnergyCost
        int EnergyCost { get; }

        // InPlaceEffectType
        InPlaceEffectType InPlaceEffectType { get; }

        // Projectile
        ProjectileDescriptor? Projectile { get; }

        // SoundStart
        Sound? SoundStart { get; }

        // SoundTrigger
        Sound? SoundTrigger { get; }

        // UsageMode
        ItemUsageMode UsageMode { get; }
    }
}
