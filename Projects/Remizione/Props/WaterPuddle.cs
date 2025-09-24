using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// WaterPuddle
    /// </summary>
    public class WaterPuddle : IsometricProp
    {
        // Constructor
        public WaterPuddle(GameSession session, string name)
            : base(session, name)
        {
            RenderLayer = RenderLayer.Background;
            TerrainParticleColor = new Color(37, 63, 75);
            TerrainSound = Sound.Find(SoundNames.FootstepWater);
        }
    }
}
