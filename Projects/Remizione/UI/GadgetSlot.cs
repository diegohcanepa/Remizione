using Engendro;

namespace Remizione
{
    /// <summary>
    /// GadgetSlot
    /// </summary>
    public sealed class GadgetSlot : EquipmentSlot
    {
        // Constructor
        public GadgetSlot(GameSession session)
            : base(session, Screen.HUDArea.GetPoint(RectanglePoint.LeftBottom, 22, -2), ItemCategory.Gadgets, InputBindings.UseGadgetItem)
        {
        }
    }
}
