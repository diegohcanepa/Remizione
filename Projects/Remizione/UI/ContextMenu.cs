using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione.UI
{
    /// <summary>
    /// ContextMenu
    /// </summary>
    public sealed class ContextMenu : GameObject, IInputHandler
    {
        #region Private fields

        private readonly List<RectangleF> boundingBoxes = new();
        private readonly List<ContextMenuOption<string>> optionList = [];
        private Vector2 optionTextScale = ScaleInfo.ContextMenu.Option;
        private Vector2 position;
        private readonly StickInputController stick = new(GamePadThumbStick.Left) { AutoRepeatRate = 200 };

        #endregion

        #region Constructor

        // Constructor
        public ContextMenu(EngendroGame game, Camera camera, Font? font = null)
            : base(game)
        {
            this.Camera = camera;
            this.Font = font ?? Fonts.Main;
            this.Options = new ReadOnlyCollection<ContextMenuOption<string>>(optionList);
        }

        #endregion

        #region Private members

        // Invalidate
        private void Invalidate()
        {
            if (!IsVisible)
                return;    
            
            // Get maximum width
            Width = 0;
            for (var i = 0; i < optionList.Count; i++)
            {
                if (optionList[i].TextBoundingBox.Width > Width)
                    Width = optionList[i].TextBoundingBox.Width;
            }

            Height = 0;

            var pos = Position;

            for (var i = 0; i < optionList.Count; i++)
            {

                var option = optionList[i];
                option.Position = pos;
                boundingBoxes[i] = new(pos.X, pos.Y, Width, option.TextBoundingBox.Height);
                pos.Y += option.TextBoundingBox.Height;
                Height += option.TextBoundingBox.Height;
            }

            BoundingBox = new RectangleF(Position.X, Position.Y, Width, Height);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Camera, SamplerState.PointClamp, RemizioneGame.Effects.ColorReduction.Effect);

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
            {
                if (GetOptionAt(InputManager.DefaultPlayer.Mouse.WorldPosition(Camera)) is ContextMenuOption<string> option)
                {
                    SelectedIndex = option.Index;

                    if (InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
                        Hide();
                }
                else
                    SelectedIndex = -1;
            }

            stick.Stick = GamePadThumbStick.Left;
            stick.Update(gameTime);

            for (var i = 0; i < optionList.Count; i++)
            {
                optionList[i].Update(gameTime);
            }
        }

        #endregion

        // IsActiveInGameLoop
        public override bool IsActiveInGameLoop => IsVisible;

        // AddOption
        public ContextMenuOption<string> AddOption(string key, string text)
        {
            ContextMenuOption<string> result = new(this, optionList.Count, key, text);
            optionList.Add(result);
            boundingBoxes.Add(RectangleF.Empty);
            return result;
        }

        // BoundingBox
        public RectangleF BoundingBox { get; private set; }

        // Camera
        public Camera Camera { get; }

        // CanHandleInput
        public bool CanHandleInput => true;

        // Clear
        public void Clear()
        {
            optionList.Clear();
            SelectedIndex = -1;
        }

        // Font
        public Font Font { get; }

        // GetOptionAt
        public ContextMenuOption<string>? GetOptionAt(Vector2 position)
        {
            if (!BoundingBox.Contains(position))
                return null;

            for (var i = 0; i < optionList.Count; i++)
            {
                if (boundingBoxes[i].Contains(position))
                    return optionList[i];
            }

            return null;
        }

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            if (!CanHandleInput)
                return HandleInputResult.Unhandled;

            if (optionList.Count > 1)
            {
                // Previous option
                if (InputBindings.SelectUp.IsPressed(0) || stick.IsUp(0))
                {
                    Previous();
                    return HandleInputResult.Handled;
                }

                // Next option
                else if (InputBindings.SelectDown.IsPressed(0) || stick.IsDown(0))
                {
                    Next();
                    return HandleInputResult.Handled;
                }
            }

            return HandleInputResult.Unhandled;
        }

        // HasOptions
        public bool HasOptions => optionList.Count > 0;

        // Height
        public float Height { get; private set; }

        // Hide
        public void Hide()
        {
            IsVisible = false;
        }

        // IsVisible
        public bool IsVisible { get; private set; }

        // Last
        public void Last()
        {
            if (optionList.Count > 0)
                SelectedIndex = optionList.Count - 1;
        }

        // Next
        public bool Next()
        {
            if (optionList.Count <= 1)
                return false;

            if (SelectedIndex == optionList.Count - 1)
                SelectedIndex = 0;
            else
                SelectedIndex++;

            return true;
        }

        // OptionCount
        public int OptionCount => optionList.Count;

        // Options
        public ReadOnlyCollection<ContextMenuOption<string>> Options { get; }

        // Position
        public Vector2 Position { get; private set; }

        // Previous
        public bool Previous()
        {
            if (optionList.Count <= 1)
                return false;

            if (SelectedIndex == 0)
                SelectedIndex = optionList.Count - 1;
            else
                SelectedIndex--;

            return true;
        }

        // SelectedIndex
        public int SelectedIndex { get; set; } = -1;

        // SelectedOption
        public ContextMenuOption<string>? SelectedOption => SelectedIndex == -1 ? null : optionList[SelectedIndex];

        // Show
        public void Show(Vector2 position)
        {
            IsVisible = true;
            this.Position = position;
            Invalidate();
        }

        // TextScale
        public Vector2 TextScale { get; set; } = ScaleInfo.Text.Large;

        // Width
        public float Width { get; private set; }
    }
}
