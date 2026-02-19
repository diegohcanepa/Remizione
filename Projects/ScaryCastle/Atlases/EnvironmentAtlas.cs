using Engendro;
using System.Collections.ObjectModel;

namespace ScaryCastle
{
    /// <summary>
    /// EnvironmentAtlas
    /// </summary>
    public sealed partial class EnvironmentAtlas : Atlas
    {
        // Constructor
        public EnvironmentAtlas(EngendroGame game)
            : base(game.Content, "Environment", ContentManagerExtension.EncodePath(ContentFolder.Atlases, "Environment"), false)
        {
            DefaultLight = this[nameof(DefaultLight)];
            DustParticles = CreateReadOnlyCollection("DustParticle", 1, 1);
            DropMark = this[nameof(DropMark)];
            FireflyParticles = CreateReadOnlyCollection("FireflyParticle", 1, 2);
            Guts = CreateReadOnlyCollection("Gut", 1, 12);
            GutStains = CreateReadOnlyCollection("GutStain", 1, 2);
            GlobalLight = this[nameof(GlobalLight)];
            LightningLight = this[nameof(LightningLight)];
            Sack = this[nameof(Sack)];
        }

        // DefaultLight
        public AtlasImage DefaultLight { get; }

        // DustParticles
        public ReadOnlyCollection<AtlasImage> DustParticles { get; }

        // DropMark
        public AtlasImage DropMark { get; }

        // FireflyParticles
        public ReadOnlyCollection<AtlasImage> FireflyParticles { get; }

        // GlobalLight
        public AtlasImage GlobalLight { get; }

        // Guts
        public ReadOnlyCollection<AtlasImage> Guts { get; }

        // GutStains
        public ReadOnlyCollection<AtlasImage> GutStains { get; }

        // LightningLight
        public AtlasImage LightningLight { get; }

        // Sack
        public AtlasImage Sack { get; }
    }
}
