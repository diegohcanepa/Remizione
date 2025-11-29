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
            : base(session, Screen.HUDArea.GetPoint(RectanglePoint.LeftTop, 2, -2), ItemCategory.Gadget, null)
        {
        }
    }
}
