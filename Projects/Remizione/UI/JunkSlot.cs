using Engendro;

namespace Remizione
{
    /// <summary>
    /// JunkSlot
    /// </summary>
    public sealed class JunkSlot : EquipmentSlot
    {
        // Constructor
        public JunkSlot(GameSession session)
            : base(session, Screen.HUDArea.GetPoint(RectanglePoint.LeftBottom, 7, -2), InventoryCategory.Junk, InputBindings.UseJunkItem, true)
        {
        }
    }
}
