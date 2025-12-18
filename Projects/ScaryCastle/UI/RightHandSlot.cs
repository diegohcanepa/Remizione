using Engendro;

namespace ScaryCastle
{
    /// <summary>
    /// RightHandSlot
    /// </summary>
    public sealed class RightHandSlot : EquipmentSlot
    {
        // Constructor
        public RightHandSlot(GameSession session)
            : base(session, Screen.HUDArea.GetPoint(RectanglePoint.LeftBottom, 17, -2), ItemCategory.RightHand, InputBindings.UseRightHandItem)
        {
        }
    }
}
