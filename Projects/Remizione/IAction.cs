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

        // HPCost
        int HPCost { get; }

        // ImpactEffectType
        ImpactEffectType ImpactEffectType { get; }

        // MissChance
        Ratio MissChance { get; }

        // ProjectileImageName
        string ProjectileImageName { get; }

        // SoundStart
        Sound? SoundStart { get; }

        // SoundTrigger
        Sound? SoundTrigger { get; }
    }
}
