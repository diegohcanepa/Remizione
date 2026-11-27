using Engendro;
using Engendro.Audio;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// PunchAction
    /// </summary>
    public sealed class PunchAction : IAction
    {
        private readonly List<EffectDescriptor> effectDescriptors = [];

        // Constructor
        public PunchAction()
        {
            var effectDesc = new EffectDescriptor()
            {
                Amount = new("1"),
                Context = EffectContext.Attack,
                EffectType = EffectType.Damage,
                Sound = Sound.Get(SoundNames.BareFist)
            };

            effectDescriptors.Add(effectDesc);
            EffectDescriptors = effectDescriptors.AsReadOnly();
        }

        public ActionKind ActionKind { get; } = ActionKind.Proximity;

        public bool AllowSelfAction => false;

        public string AnimationName { get; } = "Punch";

        public int AreaOfEffect => 0;

        public ReadOnlyCollection<EffectDescriptor> EffectDescriptors { get; }

        public int HPCost => '0';

        public ImpactEffectType ImpactEffectType => ImpactEffectType.None;

        public static PunchAction Instance = new();

        public Ratio MissChance => 0;

        public string Name => "Punch";

        public string ProjectileImageName => string.Empty;

        public Sound? SoundStart => null;

        public Sound? SoundTrigger => null;

        public bool Spawn => false;

        public ApplyUsageOutcome ApplyUsage(Actor actor) => ApplyUsageOutcome.None;
    }
}
