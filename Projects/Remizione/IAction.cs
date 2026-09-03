using Engendro;
using Engendro.Audio;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// IAction
    /// </summary>
    public interface IAction
    {
        // ActionKind
        ActionKind ActionKind { get; }

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

        // MissChance
        Ratio MissChance { get; }

        // Projectile
        ProjectileDescriptor? Projectile { get; }

        // SoundStart
        Sound? SoundStart { get; }

        // SoundTrigger
        Sound? SoundTrigger { get; }

        // StaminaCost
        int StaminaCost { get; }
    }
}
