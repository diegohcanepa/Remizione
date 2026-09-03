using Engendro;

namespace ScaryCastle.Menus
{
    /// <summary>
    /// NintendoSwitchControlsScene
    /// </summary>
    public sealed partial class NintendoSwitchControlsScene : ControlsCoreScene
    {
        // Constructor
        public NintendoSwitchControlsScene(ScaryCastleGame game)
            : base(game, Atlases.Menu.ControlsNintendoSwitch, .35f, -22, false)
        {
            SetLabel(LabelName.Inventory, 312, 74, RectanglePoint.Left);
            SetLabel(LabelName.Run, 312, 83, RectanglePoint.Left);
            SetLabel(LabelName.Interact, 312, 93, RectanglePoint.Left);
            SetLabel(LabelName.Movement, 176, 83, RectanglePoint.Right);
            SetLabel(LabelName.Kick, 312, 103, RectanglePoint.Left);
            SetLabel(LabelName.Menu, 312, 65, RectanglePoint.Left);
        }
    }
}