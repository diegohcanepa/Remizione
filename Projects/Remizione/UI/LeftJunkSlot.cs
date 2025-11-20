using Engendro;

namespace Remizione
{
    /// <summary>
    /// JunkSlot
    /// </summary>
    public sealed class LeftJunkSlot : EquipmentSlot
    {
        // Constructor
        public LeftJunkSlot(GameSession session)
            : base(session, Screen.HUDArea.GetPoint(RectanglePoint.LeftBottom, 2, -2), ItemCategory.Junk, InputBindings.UseLeftJunkItem)
        {
        }
    }
}
