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
            DefaultLight = this[nameof(DefaultLight)];
            DustParticles = CreateReadOnlyCollection("DustParticle", 1, 1);
            FireflyParticles = CreateReadOnlyCollection("FireflyParticle", 1, 2);
            LightningLight = this[nameof(LightningLight)];
            MoveDestinationMark = this[nameof(MoveDestinationMark)];
            RainParticles = CreateReadOnlyCollection("RainParticle", 1, 5);
        }

        // DefaultLight
        public AtlasImage DefaultLight { get; }

        // DustParticles
        public ReadOnlyCollection<AtlasImage> DustParticles { get; }

        // FireflyParticles
        public ReadOnlyCollection<AtlasImage> FireflyParticles { get; }

        // LightningLight
        public AtlasImage LightningLight { get; }

        // MoveDestinationMark
        public AtlasImage MoveDestinationMark { get; }

        // RainParticles
        public ReadOnlyCollection<AtlasImage> RainParticles { get; }
    }
}
