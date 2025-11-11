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
            : base(session, Screen.HUDArea.GetPoint(RectanglePoint.LeftBottom, 2, -2), ItemCategory.Junk, InputBindings.UseJunkItem)
        {
        }
    }
}
