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

        // Attacks
        internal static readonly InputBinding CloseAttack = InputManager.AddBinding(nameof(CloseAttack), Buttons.A);

        // Menu
        internal static readonly InputBinding Console = InputManager.AddBinding(nameof(Console), null, Keys.Tab);
        internal static readonly InputBinding DeleteSlot = InputManager.AddBinding(nameof(DeleteSlot), Buttons.Y, Keys.F8);
        internal static readonly InputBinding InGameMenu = InputManager.AddBinding(nameof(InGameMenu), Buttons.Start, Keys.Tab);
        internal static readonly InputBinding NextMenuItem = InputManager.AddBinding(nameof(NextMenuItem), Buttons.DPadDown, Keys.S, Keys.Down);
        internal static readonly InputBinding NextMenuItemValue = InputManager.AddBinding(nameof(NextMenuItemValue), Buttons.DPadRight, Keys.D, Keys.Right);
        internal static readonly InputBinding PreviousMenuItem = InputManager.AddBinding(nameof(PreviousMenuItem), Buttons.DPadUp, Keys.W, Keys.Up);
        internal static readonly InputBinding PreviousMenuItemValue = InputManager.AddBinding(nameof(PreviousMenuItemValue), Buttons.DPadLeft, Keys.A, Keys.Left);
        internal static readonly InputBinding ClickMenuItem = InputManager.AddBinding(nameof(ClickMenuItem), GetButtonA(), Keys.E, Keys.Enter);
        internal static readonly InputBinding SignIn = InputManager.AddBinding(nameof(SignIn), Buttons.X, Keys.X);

        // Misc
        internal static readonly InputBinding Back = InputManager.AddBinding(nameof(Back), GetButtonB(), Keys.Escape);
        internal static readonly InputBinding Continue = InputManager.AddBinding(nameof(Continue), GetButtonA(), Keys.E);
        internal static readonly InputBinding Close = InputManager.AddBinding(nameof(Close), GetButtonB(), Keys.Escape);
        internal static readonly InputBinding Exit = InputManager.AddBinding(nameof(Exit), GetButtonB(), Keys.Escape);
        internal static readonly InputBinding Info = InputManager.AddBinding(nameof(Info), Buttons.Y, Keys.I, Keys.Enter);
        internal static readonly InputBinding Interact = InputManager.AddBinding(nameof(Interact), Buttons.Y, Keys.E, Keys.Enter);
        internal static readonly InputBinding Inventory = InputManager.AddBinding(nameof(Inventory), Buttons.RightShoulder, Keys.I);
        internal static readonly InputBinding ItemAction = InputManager.AddBinding(nameof(ItemAction), Buttons.X, Keys.E);
        internal static readonly InputBinding NextDialog = InputManager.AddBinding(nameof(NextDialog), Buttons.Y, Keys.E, Keys.Enter);
        internal static readonly InputBinding Sacrifice = InputManager.AddBinding(nameof(Sacrifice), Buttons.X, Keys.S);
        internal static readonly InputBinding Select = InputManager.AddBinding(nameof(Select), GetButtonA(), Keys.E, Keys.Enter);
        internal static readonly InputBinding SelectDialogOption = InputManager.AddBinding(nameof(SelectDialogOption), Buttons.A, Keys.E, Keys.Enter);
        internal static readonly InputBinding SpeechBubble = InputManager.AddBinding(nameof(SpeechBubble), Buttons.Y, Keys.E, Keys.Enter);
        internal static readonly InputBinding UseItem = InputManager.AddBinding(nameof(UseItem), Buttons.X, Keys.Z);

        // Keyboard movement
        internal static readonly InputBinding KeyboardMoveDown = InputManager.AddBinding(nameof(KeyboardMoveDown), 0, Keys.S);
        internal static readonly InputBinding KeyboardMoveLeft = InputManager.AddBinding(nameof(KeyboardMoveLeft), 0, Keys.A);
        internal static readonly InputBinding KeyboardMoveRight = InputManager.AddBinding(nameof(KeyboardMoveRight), 0, Keys.D);
        internal static readonly InputBinding KeyboardMoveUp = InputManager.AddBinding(nameof(KeyboardMoveUp), 0, Keys.W);

        // Generic options
        internal static readonly InputBinding NextTab = InputManager.AddBinding(nameof(NextTab), Buttons.RightShoulder, Keys.Tab);
        internal static readonly InputBinding PreviousTab = InputManager.AddBinding(nameof(PreviousTab), Buttons.LeftShoulder, ModifiersKey.Shift, Keys.Tab);
        internal static readonly InputBinding SelectDown = InputManager.AddBinding(nameof(SelectDown), Buttons.DPadDown, Keys.Down, Keys.S);
        internal static readonly InputBinding SelectLeft = InputManager.AddBinding(nameof(SelectLeft), Buttons.DPadLeft, Keys.Left, Keys.A);
        internal static readonly InputBinding SelectRight = InputManager.AddBinding(nameof(SelectRight), Buttons.DPadRight, Keys.Right, Keys.D);
        internal static readonly InputBinding SelectUp = InputManager.AddBinding(nameof(SelectUp), Buttons.DPadUp, Keys.Up, Keys.W);

        // Quick slot
        internal static readonly InputBinding QuickSlotNextWeaponItem = InputManager.AddBinding(nameof(QuickSlotNextWeaponItem), Buttons.DPadRight, Keys.Right);
        internal static readonly InputBinding QuickSlotPreviousWeaponItem = InputManager.AddBinding(nameof(QuickSlotPreviousWeaponItem), Buttons.DPadLeft, Keys.Left);
    }
}
