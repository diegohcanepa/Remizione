using ScaryCastle.Battle;

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

        // DecideIntent
        public virtual Intent? DecideIntent()
        {
            return null;
        }

        // Owner
        public Actor Owner { get; }
    }
}
