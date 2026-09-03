using Engendro;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// EnvironmentAtlas
    /// </summary>
    public sealed partial class EnvironmentAtlas : Atlas
    {
        // Constructor
        public EnvironmentAtlas()
            : base(EngendroGame.Instance.Content, "Environment", ContentManagerExtension.EncodePath(ContentFolder.Atlases, "Environment"), false)
        {
            Bones = CreateReadOnlyCollection("Bone", 1, 5);
            DefaultLight = this[nameof(DefaultLight)];
            DustParticles = CreateReadOnlyCollection("DustParticle", 1, 1);
            FireflyParticles = CreateReadOnlyCollection("FireflyParticle", 1, 2);
            Guts = CreateReadOnlyCollection("Gut", 1, 12);
            GutStains = CreateReadOnlyCollection("GutStain", 1, 2);
            LightningLight = this[nameof(LightningLight)];
            ToxicGuts = CreateReadOnlyCollection("ToxicGut", 1, 12);
            ToxicGutStains = CreateReadOnlyCollection("ToxicGutStain", 1, 2);
        }

        // Bones
        public ReadOnlyCollection<AtlasImage> Bones { get; }

        // DefaultLight
        public AtlasImage DefaultLight { get; }

        // DustParticles
        public ReadOnlyCollection<AtlasImage> DustParticles { get; }

        // FireflyParticles
        public ReadOnlyCollection<AtlasImage> FireflyParticles { get; }

        // Guts
        public ReadOnlyCollection<AtlasImage> Guts { get; }

        // GutStains
        public ReadOnlyCollection<AtlasImage> GutStains { get; }

        // LightningLight
        public AtlasImage LightningLight { get; }

        // ToxicGuts
        public ReadOnlyCollection<AtlasImage> ToxicGuts { get; }

        // ToxicGutStains
        public ReadOnlyCollection<AtlasImage> ToxicGutStains { get; }
    }
}
