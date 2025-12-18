using Engendro;

namespace ScaryCastle.Menus
{
    /// <summary>
    /// PlayStation5ControlsScene
    /// </summary>
    public sealed partial class PlayStation5ControlsScene : ControlsCoreScene
    {
        // Constructor
        public PlayStation5ControlsScene(ScaryCastleGame game)
            : base(game, Atlases.Menu.ControlsPlayStation5, .35f, -22, false)
        {
            SetLabel(LabelName.Inventory, 350, 108.5f, RectanglePoint.Left);
            SetLabel(LabelName.Run, 350, 121, RectanglePoint.Left);
            SetLabel(LabelName.Interact, 350, 134, RectanglePoint.Left);
            SetLabel(LabelName.Movement, 211, 176, RectanglePoint.Top);
            SetLabel(LabelName.Menu, 350, 97, RectanglePoint.Left);
            SetLabel(LabelName.Kick, 285, 185, RectanglePoint.Bottom);
        }
    }
}