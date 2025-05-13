using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione.UI
{
    /// <summary>
    /// InGameMenu
    /// </summary>
    public sealed class InGameMenu<TKey> : GameObject, IInputHandler where TKey : Enum
    {
        #region Private fields

        private readonly List<InGameMenuOption<TKey>> optionList = [];
        private InGameMenuOption<TKey>? selectedOption;

        #endregion

        #region Constructor

        // Constructor
        public InGameMenu(EngendroGame game)
            : base(game)
        {
            this.Options = new ReadOnlyCollection<InGameMenuOption<TKey>>(optionList);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);

            for (var i = 0; i < optionList.Count; i++)
            {
                optionList[i].Draw(gameTime);
            }
            
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (InputManager.DefaultPlayer.LastInputMethod == InputMethod.Mouse)
                HoveredOption = GetOptionAt(InputManager.DefaultPlayer.Mouse.VirtualPosition);

            for (var i = 0; i < optionList.Count; i++)
            {
                optionList[i].Update(gameTime);
            }
        }

        #endregion

        // AddOption
        public InGameMenuOption<TKey> AddOption(TKey key, string text, Action action)
        {
            InGameMenuOption<TKey> result = new(this, key, text, action);
            optionList.Add(result);
            return result;
        }

        // CanHandleInput
        public bool CanHandleInput => true;

        // GetOptionAt
        public InGameMenuOption<TKey>? GetOptionAt(Vector2 position)
        {
            for (var i = 0; i < optionList.Count; i++)
            {
                if (optionList[i].BoundingBox.Contains(position))
                    return optionList[i];
            }

            return null;
        }

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            if (!CanHandleInput)
                return HandleInputResult.Unhandled;

            if (HoveredOption != null && InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
            {
                SelectedOption = HoveredOption;
                SelectedOption.Action.Invoke();
                return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
        }

        // Hide
        public void Hide()
        {
            IsVisible = false;
        }

        // HoveredOption
        public InGameMenuOption<TKey>? HoveredOption { get; private set; }

        // IsActiveInGameLoop
        public override bool IsActiveInGameLoop => IsVisible;

        // IsVisible
        public bool IsVisible { get; private set; }

        // Options
        public ReadOnlyCollection<InGameMenuOption<TKey>> Options { get; }

        // Position
        public Vector2 Position { get; private set; }

        // SelectedOption
        public InGameMenuOption<TKey>? SelectedOption
        {
            get => selectedOption;
            set
            {
                if (value != selectedOption)
                {
                    if (selectedOption != null && value != null)
                        Sound.Play(SoundNames.MenuSelect);

                    selectedOption = value;
                }
            }
        }

        // Show
        public void Show(Vector2 position)
        {
            IsVisible = true;
            Position = position;

            var pos = position;
            for (var i = 0; i < optionList.Count; i++)
            {
                var option = optionList[i];
                option.Position = pos;
                pos.Y += option.BoundingBox.Height + 2;
            }
        }
    }
}
