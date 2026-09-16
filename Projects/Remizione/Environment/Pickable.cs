namespace Remizione
{
    /// <summary>
    /// Pickable
    /// </summary>
    public abstract class Pickable : Prop
    {
        // Constructor
        protected Pickable(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Props;
            ApproachBehavior = ApproachBehavior.ClosestSide;
            DepthOffset = 20;
            IgnoreWalkArea = false;
            Verb = Verb.PickUp;
        }
    }
}
