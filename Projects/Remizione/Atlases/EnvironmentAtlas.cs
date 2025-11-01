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
        public EnvironmentAtlas(EngendroGame game)
            : base(game.Content, "Environment", ContentHelper.EncodePath(ContentFolder.Atlases, "Environment"), false)
        {
            CraftingMark = this[nameof(CraftingMark)];
            DefaultLight = this[nameof(DefaultLight)];
            DustParticles = CreateReadOnlyCollection("DustParticle", 1, 1);
            FireflyParticles = CreateReadOnlyCollection("FireflyParticle", 1, 2);
            Guts = CreateReadOnlyCollection("Gut", 1, 12);
            GutStains = CreateReadOnlyCollection("GutStain", 1, 2);
            GlobalLight = this[nameof(GlobalLight)];
            LightningLight = this[nameof(LightningLight)];
            Ticket = this[nameof(Ticket)];
        }

        // CraftingMark
        public AtlasImage CraftingMark { get; }

        // DefaultLight
        public AtlasImage DefaultLight { get; }

        // DustParticles
        public ReadOnlyCollection<AtlasImage> DustParticles { get; }

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

        // Ticket
        public AtlasImage Ticket { get; }
    }
}
