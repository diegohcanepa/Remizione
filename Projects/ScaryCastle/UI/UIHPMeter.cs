using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// UIHPMeter
    /// </summary>
    public sealed class UIHPMeter : UIStatMeter
    {
        // Constructor
        public UIHPMeter(Vector2 margin)
            : base(margin, Atlases.UI.HPEmpty, Atlases.UI.HPHalf, Atlases.UI.HPFull)
        {
        }

        // GetStatMaxValue
        protected override int GetStatMaxValue()
        {
            return Actor == null ? 0 : Actor.MaxHP;
        }

        // GetStatValue
        protected override int GetStatValue()
        {
            return Actor == null ? 0 : Actor.HP;
        }
    }
}
