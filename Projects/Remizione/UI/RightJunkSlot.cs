using Engendro;

namespace Remizione
{
    /// <summary>
    /// RightJunkSlot
    /// </summary>
    public sealed class RightJunkSlot : EquipmentSlot
    {
        // Constructor
        public RightJunkSlot(GameSession session)
            : base(session, Screen.HUDArea.GetPoint(RectanglePoint.LeftBottom, 17, -2), ItemCategory.Junk, InputBindings.UseRightJunkItem)
        {
        }
    }
}
