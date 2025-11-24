namespace Remizione
{
    /// <summary>
    /// WaterPuddle
    /// </summary>
    public class WaterPuddle : Prop
    {
        // Constructor
        public WaterPuddle(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Environment;
            CollisionDetection = false;
            PlacementPhase = PlacementPhase.Object;
            RenderLayer = RenderLayer.Background;
            //TerrainParticleColor = new Color(75, 133, 150);
            //TerrainSound = Sound.Find(SoundNames.FootstepWater);
        }
    }
}
