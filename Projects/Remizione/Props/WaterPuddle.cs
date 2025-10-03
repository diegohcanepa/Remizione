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
            CollisionDetection = false;
            PlacementPhase = PlacementPhase.NaturalObject;
            RenderLayer = RenderLayer.Background;
            //TerrainParticleColor = new Color(75, 133, 150);
            //TerrainSound = Sound.Find(SoundNames.FootstepWater);
        }
    }
}
