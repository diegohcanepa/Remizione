using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework.Input;

namespace Remizione
{
    /// <summary>
    /// InputBindings
    /// </summary>
    internal static class InputBindings
    {
        // GetButtonA()
        private static Buttons GetButtonA()
        {
            return EngendroGame.RunningPlatform == RunningPlatform.NintendoSwitch ? Buttons.B : Buttons.A;
        }

        // GetButtonB()
        private static Buttons GetButtonB()
        {
            return EngendroGame.RunningPlatform == RunningPlatform.NintendoSwitch ? Buttons.A : Buttons.B;
        }

        // Menu
        internal static readonly InputBinding Console = InputManager.AddBinding(nameof(Console), null, Keys.Tab);
        internal static readonly InputBinding InGameMenu = InputManager.AddBinding(nameof(InGameMenu), Buttons.Start, Keys.Tab);
        internal static readonly InputBinding ClickMenuItem = InputManager.AddBinding(nameof(ClickMenuItem), GetButtonA(), Keys.E, Keys.Enter);

        // Misc
        internal static readonly InputBinding Back = InputManager.AddBinding(nameof(Back), GetButtonB(), Keys.X);
        internal static readonly InputBinding Continue = InputManager.AddBinding(nameof(Continue), GetButtonA(), Keys.E);
        internal static readonly InputBinding Inventory = InputManager.AddBinding(nameof(Inventory), Buttons.Y, Keys.I);
        internal static readonly InputBinding Select = InputManager.AddBinding(nameof(Select), GetButtonA(), Keys.E, Keys.Enter);
        internal static readonly InputBinding SelectDialogOption = InputManager.AddBinding(nameof(SelectDialogOption), Buttons.A, Keys.E, Keys.Enter);
        internal static readonly InputBinding SpeechText = InputManager.AddBinding(nameof(SpeechText), Buttons.A, Keys.E, Keys.Enter);

        // Generic options
        internal static readonly InputBinding SelectDown = InputManager.AddBinding(nameof(SelectDown), Buttons.DPadDown, Keys.Down);
        internal static readonly InputBinding SelectLeft = InputManager.AddBinding(nameof(SelectLeft), Buttons.DPadLeft, Keys.Left);
        internal static readonly InputBinding SelectRight = InputManager.AddBinding(nameof(SelectRight), Buttons.DPadRight, Keys.Right);
        internal static readonly InputBinding SelectUp = InputManager.AddBinding(nameof(SelectUp), Buttons.DPadUp, Keys.Up);
    }
}
