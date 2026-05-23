using Engendro.Audio;
using System.Collections.ObjectModel;

namespace ScaryCastle
{
    /// <summary>
    /// IGameAction
    /// </summary>
    public interface IGameAction
    {
        int AreaRange { get; }
        Sound? Sound { get; }
        ReadOnlyCollection<EffectDescriptor> EffectDescriptors { get; }
        InPlaceEffectType InPlaceEffectType { get; }
        ProjectileDescriptor? Projectile { get; }
        ItemUsageScope UsageScope { get; }
    }
}
