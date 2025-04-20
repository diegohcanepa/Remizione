namespace Remizione
{
    /// <summary>
    /// PlacementCondition
    /// </summary>
    public abstract class PlacementCondition
    {
        // IsAvailable
        public abstract bool IsAvailable(GameThing thing, WorldBlock block);
    }
}
