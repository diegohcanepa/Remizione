using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// ProjectileDescriptor
    /// </summary>
    public sealed class ProjectileDescriptor
    {
        private readonly List<EffectDescriptor> effectDescriptors = [];

        // Constructor
        public ProjectileDescriptor(JsonElement element)
        {
            this.Gravity = element.GetFloat("gravity", 0);
            this.ImageName = element.GetString("imageName", string.Empty);
            this.InitialYVelocity = element.GetFloat("initialYVelocity", 0);
            this.RicochetSound = element.GetObject("ricochetSound", Sound.Get);
            this.Speed = element.GetFloat("speed", 0);
            this.Trajectory = element.GetEnum("trajectory", ProjectileTrajectoryType.Linear);

            // Effects
            if (element.TryGetProperty("effects", out JsonElement effectsArray))
            {
                foreach (var effectJson in effectsArray.EnumerateArray())
                {
                    effectDescriptors.Add(new(effectJson));
                }
            }

            EffectDescriptors = effectDescriptors.AsReadOnly();
        }

        // EffectDescriptors
        public ReadOnlyCollection<EffectDescriptor> EffectDescriptors { get; }

        // Gravity
        public float Gravity { get; }

        // ImageName
        public string ImageName { get; }

        // InitialYVelocity
        public float InitialYVelocity { get; }

        // RicochetSound
        public Sound? RicochetSound { get; }

        // Speed
        public float Speed { get; }

        // Trajectory
        public ProjectileTrajectoryType Trajectory { get; }
    }
}
