using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione.UI
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
                if (GetOptionAt(InputManager.DefaultPlayer.Mouse.WorldPosition(Camera)) is ContextMenuOption<TKey> option)
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
        public ContextMenuOption<TKey> AddOption(TKey key, string text)
        {
            ContextMenuOption<TKey> result = new(this, optionList.Count, key, text);
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
        public ContextMenuOption<TKey>? GetOptionAt(Vector2 position)
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

        // Options
        public ReadOnlyCollection<ContextMenuOption<TKey>> Options { get; }

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
        public ContextMenuOption<TKey>? SelectedOption => SelectedIndex == -1 ? null : optionList[SelectedIndex];

        // Show
        public void Show(Vector2 position, bool fromBottom)
        {
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
                boundingBoxes[i] = new(pos.X, pos.Y, Width, option.TextBoundingBox.Height);
                pos.Y += option.TextBoundingBox.Height;
            }

            BoundingBox = new RectangleF(position.X, position.Y, Width, Height);
        }

        // TextScale
        public Vector2 TextScale { get; set; } = ScaleInfo.Text.Large;

        // Width
        public float Width { get; private set; }
    }
}
