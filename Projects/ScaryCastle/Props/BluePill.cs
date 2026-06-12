using Engendro;

namespace ScaryCastle
{
    /// <summary>
    /// BluePill
    /// </summary>
    public sealed class BluePill : Pill
    {
        // Constructor
        public BluePill(GameSession session, string name)
            : base(session, name)
        {
            DefaultImageName = nameof(BluePill);
            DisplayNameKey = $"Item.{nameof(BluePill)}.Description";
        }
    }
}
