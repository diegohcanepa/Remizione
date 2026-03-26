using Engendro;
using Microsoft.Xna.Framework;
using System.Configuration;

namespace ScaryCastle
{
    /// <summary>
    /// UIFaithMeter
    /// </summary>
    public sealed class UIFaithMeter : UIStatMeter
    {
        // Constructor
        public UIFaithMeter(Vector2 margin)
            : base(margin, Atlases.UI.FaithEmpty, Atlases.UI.FaithHalf, Atlases.UI.FaithFull)
        {
        }

        // GetStatMaxValue
        protected override int GetStatMaxValue()
        {
            return Actor == null ? 0 : Actor.MaxFaith;
        }

        // GetStatValue
        protected override int GetStatValue()
        {
            return Actor == null ? 0 : Actor.Faith;
        }
    }
}
