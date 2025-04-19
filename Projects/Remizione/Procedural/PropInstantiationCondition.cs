namespace Remizione
{
    /// <summary>
    /// PropInstantiationCondition
    /// </summary>
    public abstract class PropInstantiationCondition
    {
        // IsAvailable
        public abstract bool IsAvailable(Prop prop, WorldBlock block);
    }
}
