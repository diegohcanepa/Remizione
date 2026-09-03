using Engendro;

namespace Remizione.Menus
{
    /// <summary>
    /// XboxOneControlsScene
    /// </summary>
    public sealed partial class XboxOneControlsScene : ControlsCoreScene
    {
        // Constructor
        public XboxOneControlsScene(RemizioneGame game)
            : base(game, Atlases.Menu.ControlsXboxOne, .35f, -16, false)
        {
            SetLabel(LabelName.Inventory, 358, 87, RectanglePoint.Left);
            SetLabel(LabelName.Run, 358, 99, RectanglePoint.Left);
            SetLabel(LabelName.Interact, 358, 112, RectanglePoint.Left);
            SetLabel(LabelName.Movement, 124, 95, RectanglePoint.Right);
            SetLabel(LabelName.Menu, 358, 170, RectanglePoint.Left);
            SetLabel(LabelName.Kick, 358, 74, RectanglePoint.Left);
        }
    }
}