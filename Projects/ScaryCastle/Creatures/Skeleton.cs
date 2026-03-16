namespace ScaryCastle
{
    /// <summary>
    /// Skeleton
    /// </summary>
    public sealed class Skeleton : Actor
    {
        // Constructor
        public Skeleton(GameSession session, string name)
            : base(session, name)
        {
            BodySize = BodySize.Medium;
            Guts = 0;

            BodyMachine.AddState(new BodyCloseAttackState());
        }
    }
}
