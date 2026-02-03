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

        // Owner
        public Actor Owner { get; }
    }
}
