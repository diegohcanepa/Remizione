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
        public string AnimationName { get; }

        // AreaRange
        int AreaRange { get; }

        // EffectDescriptors
        ReadOnlyCollection<EffectDescriptor> EffectDescriptors { get; }

        // InPlaceEffectType
        InPlaceEffectType InPlaceEffectType { get; }

        // Projectile
        ProjectileDescriptor? Projectile { get; }

        // Sound
        Sound? Sound { get; }

        // UsageScope
        ItemUsageScope UsageScope { get; }
    }
}
