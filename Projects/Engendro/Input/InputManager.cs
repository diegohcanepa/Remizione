using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Engendro.Input
{
    /// <summary>
    /// InputManager
    /// </summary>
    public static class InputManager
    {
        #region Private fields

        private static readonly Dictionary<string, InputBinding> bindings = [];
        private static readonly List<PlayerInputManager> playerInputManagers = [];
        private static int suspendInterval;

        #endregion

        #region Constructor

        // Static constructor
        static InputManager()
        {
            for (int i = 0; i < MaximumPlayers; i++)
            {
                playerInputManagers.Add(new PlayerInputManager(i));
            }

            Players = new ReadOnlyCollection<PlayerInputManager>(playerInputManagers);
        }

        #endregion

        #region Internal members

        // Update
        internal static void Update(GameTime gameTime)
        {
            if (suspendInterval > 0)
                suspendInterval -= gameTime.ElapsedGameTime.Milliseconds;

            for (int i = 0; i < PlayerCount; i++)
            {
                playerInputManagers[i].Update(gameTime);
            }
        }

        #endregion

        // AddBinding
        public static InputBinding AddBinding(string name, Buttons? button, params Keys[] keys)
        {
            return AddBinding(name, button, ModifiersKey.None, keys);
        }

        // AddBinding
        public static InputBinding AddBinding(string name, Buttons? button, ModifiersKey modifiers, params Keys[] keys)
        {
            CodeContract.NotEmpty(name, nameof(name));

            InputBinding result = new(name, button, keys)
            {
                RequiresAlt = modifiers.HasFlag(ModifiersKey.Alt),
                RequiresControl = modifiers.HasFlag(ModifiersKey.Control),
                RequiresShift = modifiers.HasFlag(ModifiersKey.Shift)
            };

            bindings.Add(name, result);
            return result;
        }

        // AllowGamePad
        public static bool AllowGamePad { get; set; } = true;

        // AllowKeyboard
        public static bool AllowKeyboard { get; set; } = true;

        // AllowMouse
        public static bool AllowMouse { get; set; } = true;

        // AutoAssignGamepad
        public static bool AutoAssignGamepad { get; set; } = true;

        // DefaultPlayer
        public static PlayerInputManager DefaultPlayer => playerInputManagers[0];

        // GetBinding
        public static InputBinding? GetBinding(string name) => bindings.TryGetValue(name, out var value) ? value : null;

        // GetBindingNotNull
        public static InputBinding GetBindingNotNull(string name)
        {
            CodeContract.NotEmpty(name, nameof(name));

            if (bindings.TryGetValue(name, out var value))
                return value;

            throw new InvalidOperationException($"Input binding [{name}] not found.");
        }

        // IsSuspended
        public static bool IsSuspended => suspendInterval > 0;

        // MaximumPlayers
        public const int MaximumPlayers = 4;

        // PlayerCount
        public static int PlayerCount
        {
            get;
            set
            {
                CodeContract.ValidRange(value, 1, MaximumPlayers, nameof(value));
                field = value;
            }
        } = 1;

        // Players
        public static ReadOnlyCollection<PlayerInputManager> Players { get; }

        // Reset
        public static void Reset() => Suspend(50);

        // Suspend
        public static void Suspend(int duration)
        {
            suspendInterval = duration;
            for (int i = 0; i < playerInputManagers.Count; i++)
            {
                playerInputManagers[i].Reset();
            }
        }
    }
}
