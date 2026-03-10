namespace ScaryCastle
{
    /// <summary>
    /// Skeleton
    /// </summary>
    public sealed class Skeleton : ProceduralActor
    {
        // Constructor
        public Skeleton(GameSession session, string name)
            : base(session, name)
        {
            BodySize = BodySize.Medium;
            AttackRange = 10;
            Guts = 0;
            Sensor.ViewAngle = 100;

            BodyMachine.AddState(new BodyCloseAttackState());
        }
    }
}
