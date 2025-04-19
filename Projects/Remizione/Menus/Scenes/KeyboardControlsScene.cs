using Engendro;

namespace Remizione.Menus
{
    /// <summary>
    /// KeyboardControlsScene
    /// </summary>
    public sealed partial class KeyboardControlsScene : ControlsCoreScene
    {
        // Constructor
        public KeyboardControlsScene(RemizioneGame game)
            : base(game, Atlases.Menu.ControlsKeyboard, .4f, -12, true)
        {
            SetLabel(LabelName.Movement, 140, 109, RectanglePoint.Right);
            SetLabel(LabelName.MovementAlt, 332, 109, RectanglePoint.Left);
            SetLabel(LabelName.Interact, 140, 127, RectanglePoint.Right);
            SetLabel(LabelName.Run, 332, 127, RectanglePoint.Left);
            SetLabel(LabelName.Kick, 332, 144, RectanglePoint.Left);
            SetLabel(LabelName.Inventory, 240, 162, RectanglePoint.Top);
        }
    }
}