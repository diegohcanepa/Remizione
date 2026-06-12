using Engendro;

namespace ScaryCastle
{
    /// <summary>
    /// RedPill
    /// </summary>
    public sealed class RedPill : Pill
    {
        // Constructor
        public RedPill(GameSession session, string name)
            : base(session, name)
        {
            DefaultImageName = nameof(RedPill);
            DisplayNameKey = $"Item.{nameof(RedPill)}.Description";
        }
    }
}
