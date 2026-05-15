using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// WaterPuddle
    /// </summary>
    public sealed class WaterPuddle : Prop
    {
        // Constructor
        public WaterPuddle(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Props;
            CollisionDetection = false;
            RenderLayer = RenderLayer.Background;
            TerrainParticleColor = new(75, 133, 150);
            TerrainSound = Sound.Find(SoundNames.FootstepWater);
        }
    }
}
