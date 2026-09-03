using Engendro;

namespace Remizione.Menus
{
    /// <summary>
    /// WindowsControlsScene
    /// </summary>
    public sealed partial class WindowsControlsScene : ControlsCoreScene
    {
        // Constructor
        public WindowsControlsScene(RemizioneGame game)
            : base(game, Atlases.Menu.ControlsWindows, .35f, 0, false)
        {
            SetLabel(LabelName.Inventory, 356, 102, RectanglePoint.Left);
            SetLabel(LabelName.Run, 356, 114, RectanglePoint.Left);
            SetLabel(LabelName.Interact, 356, 126.5f, RectanglePoint.Left);
            SetLabel(LabelName.Movement, 124, 113, RectanglePoint.Right);
            SetLabel(LabelName.Menu, 251, 186, RectanglePoint.Top);
            SetLabel(LabelName.Kick, 265, 62, RectanglePoint.Bottom);
        }
    }
}