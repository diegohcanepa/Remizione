using Engendro;

namespace Remizione.Menus
{
    /// <summary>
    /// PlayStation4ControlsScene
    /// </summary>
    public sealed partial class PlayStation4ControlsScene : ControlsCoreScene
    {
        // Constructor
        public PlayStation4ControlsScene(RemizioneGame game)
            : base(game, Atlases.Menu.ControlsPlayStation4, .35f, -22, false)
        {
            SetLabel(LabelName.Inventory, 350, 108.5f, RectanglePoint.Left);
            SetLabel(LabelName.Run, 350, 121, RectanglePoint.Left);
            SetLabel(LabelName.Interact, 350, 134, RectanglePoint.Left);
            SetLabel(LabelName.Movement, 211, 176, RectanglePoint.Top);
            SetLabel(LabelName.Menu, 265, 66, RectanglePoint.Right);
            SetLabel(LabelName.Kick, 285, 186, RectanglePoint.Bottom);
        }
    }
}