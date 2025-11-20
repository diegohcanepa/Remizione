using Engendro;

namespace Remizione
{
    /// <summary>
    /// TrinketSlot
    /// </summary>
    public sealed class TrinketSlot : EquipmentSlot
    {
        // Constructor
        public TrinketSlot(GameSession session)
            : base(session, Screen.HUDArea.GetPoint(RectanglePoint.LeftTop, 2, -2), ItemCategory.Trinkets, null)
        {
        }
    }
}
