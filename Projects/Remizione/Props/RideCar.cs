namespace Remizione
{
    /// <summary>
    /// RideCar
    /// </summary>
    public sealed class RideCar : Prop
    {
        // Constructor
        public RideCar(GameSession session, string name)
            : base(session, name)
        {
            HitEffect = HitEffect.Shake;
        }
    }
}
