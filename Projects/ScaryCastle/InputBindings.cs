using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework.Input;

namespace ScaryCastle
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
        internal static readonly InputBinding SignIn = InputManager.AddBinding(nameof(SignIn), Buttons.X, Keys.X);

        // Misc
        internal static readonly InputBinding Back = InputManager.AddBinding(nameof(Back), GetButtonB(), Keys.X);
        internal static readonly InputBinding ConsumeItem = InputManager.AddBinding(nameof(ConsumeItem), Buttons.A, Keys.E);
        internal static readonly InputBinding Continue = InputManager.AddBinding(nameof(Continue), GetButtonA(), Keys.E);
        internal static readonly InputBinding Close = InputManager.AddBinding(nameof(Close), GetButtonB(), Keys.X);
        internal static readonly InputBinding Drop = InputManager.AddBinding(nameof(Drop), Buttons.Y, Keys.Delete);
        internal static readonly InputBinding EquipItem = InputManager.AddBinding(nameof(EquipItem), Buttons.A, Keys.E);
        internal static readonly InputBinding Exit = InputManager.AddBinding(nameof(Exit), GetButtonB(), Keys.Escape);
        internal static readonly InputBinding Interact = InputManager.AddBinding(nameof(Interact), Buttons.A, Keys.E, Keys.Enter);
        internal static readonly InputBinding Inventory = InputManager.AddBinding(nameof(Inventory), Buttons.Y, Keys.I);
        internal static readonly InputBinding Select = InputManager.AddBinding(nameof(Select), GetButtonA(), Keys.E, Keys.Enter);
        internal static readonly InputBinding SelectDialogOption = InputManager.AddBinding(nameof(SelectDialogOption), Buttons.A, Keys.E, Keys.Enter);
        internal static readonly InputBinding SpeechBubble = InputManager.AddBinding(nameof(SpeechBubble), Buttons.A, Keys.E, Keys.Enter);
        internal static readonly InputBinding UseFriendlyItem = InputManager.AddBinding(nameof(UseFriendlyItem), Buttons.A, Keys.E, Keys.Enter);
        internal static readonly InputBinding UseItem = InputManager.AddBinding(nameof(UseItem), Buttons.X, Keys.LeftControl, Keys.RightControl);

        // Keyboard movement
        internal static readonly InputBinding KeyboardMoveDown = InputManager.AddBinding(nameof(KeyboardMoveDown), 0, Keys.S);
        internal static readonly InputBinding KeyboardMoveLeft = InputManager.AddBinding(nameof(KeyboardMoveLeft), 0, Keys.A);
        internal static readonly InputBinding KeyboardMoveRight = InputManager.AddBinding(nameof(KeyboardMoveRight), 0, Keys.D);
        internal static readonly InputBinding KeyboardMoveUp = InputManager.AddBinding(nameof(KeyboardMoveUp), 0, Keys.W);

        // Generic options
        internal static readonly InputBinding NextTab = InputManager.AddBinding(nameof(NextTab), Buttons.RightShoulder, Keys.Right);
        internal static readonly InputBinding PreviousTab = InputManager.AddBinding(nameof(PreviousTab), Buttons.LeftShoulder, ModifiersKey.Shift, Keys.Left);
        internal static readonly InputBinding SelectDown = InputManager.AddBinding(nameof(SelectDown), Buttons.DPadDown, Keys.Down);
        internal static readonly InputBinding SelectLeft = InputManager.AddBinding(nameof(SelectLeft), Buttons.DPadLeft, Keys.Left);
        internal static readonly InputBinding SelectRight = InputManager.AddBinding(nameof(SelectRight), Buttons.DPadRight, Keys.Right);
        internal static readonly InputBinding SelectUp = InputManager.AddBinding(nameof(SelectUp), Buttons.DPadUp, Keys.Up);

        internal static readonly InputBinding NextPlayer = InputManager.AddBinding(nameof(NextPlayer), Buttons.RightShoulder, Keys.Tab);
        internal static readonly InputBinding PreviousPlayer = InputManager.AddBinding(nameof(PreviousPlayer), Buttons.LeftShoulder, ModifiersKey.Shift, Keys.Tab);
    }
}
