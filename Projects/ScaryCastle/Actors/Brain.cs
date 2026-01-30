namespace ScaryCastle
{
    /// <summary>
    /// Brain
    /// </summary>
    public class Brain
    {
        // Constructor
        public Brain(Actor owner)
        {
            this.Owner = owner;
        }

        // PickCard
        public virtual Card? PickCard(Arena arena)
        {
            return null;
        }

        // Owner
        public Actor Owner { get; }
    }
}
