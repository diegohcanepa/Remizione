using Engendro;

namespace Remizione
{
    /// <summary>
    /// LeftHandSlot
    /// </summary>
    public sealed class LeftHandSlot : EquipmentSlot
    {
        // Constructor
        public LeftHandSlot(GameSession session)
            : base(session, Screen.HUDArea.GetPoint(RectanglePoint.LeftBottom, 2, -2), ItemCategory.LeftHand, InputBindings.UseLeftHandItem)
        {
        }
    }
}
