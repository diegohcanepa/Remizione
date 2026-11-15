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

        // AllowSelfAction
        bool AllowSelfAction { get; }

        // AnimationName
        string AnimationName { get; }

        // AreaOfEffect
        int AreaOfEffect { get; }

        // ApplyUsage
        ApplyUsageOutcome ApplyUsage(Actor actor);

        // EffectDescriptors
        ReadOnlyCollection<EffectDescriptor> EffectDescriptors { get; }

        // HPCost
        int HPCost { get; }

        // ImpactEffectType
        ImpactEffectType ImpactEffectType { get; }

        // MissChance
        Ratio MissChance { get; }

        // Name
        string Name { get; }

        // ProjectileImageName
        string ProjectileImageName { get; }

        // SoundStart
        Sound? SoundStart { get; }

        // SoundTrigger
        Sound? SoundTrigger { get; }

        // Spawn
        bool Spawn { get; }
    }
}
