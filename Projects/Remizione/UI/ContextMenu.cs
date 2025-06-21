using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// ContextMenu
    /// </summary>
    public sealed class ContextMenu<TKey> : GameObject, IInputHandler
    {
        #region Private fields

        private readonly List<RectangleF> boundingBoxes = [];
        private readonly List<ContextMenuOption<TKey>> optionList = [];
        private readonly StickInputController stick = new(GamePadThumbStick.Left) { AutoRepeatRate = 200 };

        #endregion

        #region Constructor

        // Constructor
        public ContextMenu(EngendroGame game, Camera camera, Font? font = null)
            : base(game)
        {
            this.Camera = camera;
            this.Font = font ?? Fonts.CommonOutline;
            this.Options = new ReadOnlyCollection<ContextMenuOption<TKey>>(optionList);
        }

        #endregion

        #region Private members

        // HandleMouseInput
        private bool HandleMouseInput()
        {
            if (InputManager.DefaultPlayer.LastInputMethod != InputMethod.Mouse)
                return false;

            // Right button
            if (InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
            {
                SelectedOption = null;
                Hide();
                return true;
            }

            // Left button
            if (InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
            {
                if (GetOptionAt(InputManager.DefaultPlayer.Mouse.VirtualPosition) is ContextMenuOption<TKey> option)
                {
                    SelectedOption = option;
                    Hide();
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Camera);
            Game.Shapes.DrawRectangle(BoundingBox, Color.Black);
            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(Camera, SamplerState.LinearClamp);

            for (var i = 0; i < optionList.Count; i++)
            {
                if (optionList[i] == SelectedOption)
                    Game.Shapes.DrawRectangle(boundingBoxes[i], ColorPalette.ContextMenu.OptionBack);

                optionList[i].Draw(gameTime);
            }

            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (InputManager.DefaultPlayer.LastInputMethod == InputMethod.Mouse)
            {
                if (GetOptionAt(InputManager.DefaultPlayer.Mouse.WorldPosition(Camera)) is ContextMenuOption<TKey> option)
                {
                    SelectedOption = option;
                    if (InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
                        Hide();
                }
                else
                    SelectedOption = null;
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
        public ContextMenuOption<TKey> AddOption(TKey key, string text)
        {
            ContextMenuOption<TKey> result = new(this, key, text);
            optionList.Add(result);
            boundingBoxes.Add(RectangleF.Empty);
            return result;
        }

        // BoundingBox
        public RectangleF BoundingBox { get; private set; }

        // Camera
        public Camera Camera { get; }

        // Clear
        public void Clear()
        {
            optionList.Clear();
            SelectedOption = null;
        }

        // Font
        public Font Font { get; }

        // GetOptionAt
        public ContextMenuOption<TKey>? GetOptionAt(Vector2 position)
        {
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
            if (HandleMouseInput())
                return HandleInputResult.Handled;
            else
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

        // Options
        public ReadOnlyCollection<ContextMenuOption<TKey>> Options { get; }

        // SelectedOption
        public ContextMenuOption<TKey>? SelectedOption { get; private set; }

        // Show
        public void Show(Vector2 position, bool fromBottom)
        {
            InputManager.DefaultPlayer.Reset();

            IsVisible = true;

            optionList.Sort((a, b) => a.ToString().CompareTo(b.ToString()));

            // Calculate width
            Width = 0;
            for (var i = 0; i < optionList.Count; i++)
            {
                if (optionList[i].TextBoundingBox.Width > Width)
                    Width = optionList[i].TextBoundingBox.Width;
            }

            // Calculate height
            Height = 0;
            for (var i = 0; i < optionList.Count; i++)
            {
                Height += optionList[i].TextBoundingBox.Height;
            }

            if (fromBottom)
            {
                position.X -= Width / 2;
                position.Y -= Height + 2;
            }

            var pos = position;
            for (var i = 0; i < optionList.Count; i++)
            {
                var option = optionList[i];
                option.Position = pos;
                boundingBoxes[i] = new(pos.X - 2, pos.Y - 1, Width + 4, option.TextBoundingBox.Height + 1);

                pos.Y += option.TextBoundingBox.Height;
            }

            BoundingBox = new RectangleF(position.X - 2, position.Y - 2, Width + 4, Height + 4);
        }

        // TextScale
        public Vector2 TextScale { get; set; } = ScaleInfo.Text.Large;

        // Width
        public float Width { get; private set; }
    }
}
