using Engendro;

namespace Remizione
{
    /// <summary>
    /// ThingieSlot
    /// </summary>
    public sealed class ThingieSlot : EquipmentSlot
    {
        // Constructor
        public ThingieSlot(GameSession session)
            : base(session, Screen.HUDArea.GetPoint(RectanglePoint.LeftBottom, 22, -2), InventoryCategory.Thingies, InputBindings.UseThingieItem, false)
        {
        }
    }
}
