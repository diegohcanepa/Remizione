using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// PlaceholderOverride
    /// </summary>
    public sealed class PlaceholderOverride
    {
        // Constructor
        public PlaceholderOverride(string name, float? fillChance, Tags? allowTags, PlaceholderTarget? target)
        {
            this.Name = name;
            this.FillChance = fillChance;
            this.AllowTags = allowTags;
            this.Target = target;
        }

        // AllowTags
        public Tags? AllowTags { get; set; }

        // Apply
        public void Apply(Placeholder placeholder)
        {
            if (FillChance.HasValue)
                placeholder.FillChance = FillChance.Value;

            if (AllowTags != null)
                placeholder.AllowTags = AllowTags;

            if (Target.HasValue)
                placeholder.Target = Target.Value;
        }

        // FillChance
        public float? FillChance { get; set; }

        // Name
        public string Name { get; }

        // Target
        public PlaceholderTarget? Target { get; set; }
    }
}
