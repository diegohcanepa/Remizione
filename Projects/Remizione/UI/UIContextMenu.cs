using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// UIContextMenu
    /// </summary>
    public sealed class UIContextMenu<T> : GameObject, IInputHandler
    {
        #region Private fields

        private readonly List<UIContextMenuOption<T>> optionList = [];
        private readonly ImageSprite optionSelector;
        private Vector2 optionTextScale = ScaleInfo.ContextMenu.Option;
        private Vector2 position;
        private int selectedIndex = -1;
        private readonly StickInputController stick = new(GamePadThumbStick.Left) { AutoRepeatRate = 200 };

        #endregion

        #region Constructor

        // Constructor
        public UIContextMenu(EngendroGame game, Font? font = null)
            : base(game)
        {
            this.Font = font ?? Fonts.Common;

            // Option Selector
            optionSelector = new ImageSprite(game, Atlases.UI.ContextMenuOptionSelector)
            {
                PivotOrigin = RectanglePoint.Right
            };

            this.Options = new ReadOnlyCollection<UIContextMenuOption<T>>(optionList);
        }

        #endregion

        #region Private members

        // Invalidate
        private void Invalidate()
        {
            int NewLine(RectangleF bbox)
            {
                Width = (int)MathHelper.Max(Width, bbox.Width).Round(0);
                Height += (int)bbox.Height.Round(0) - 1;
                return (int)bbox.Height.Round(0) - 1;
            }

            Height = 0;
            Width = 0;
            var pos = Position;

            for (var i = 0; i < optionList.Count; i++)
            {
                var option = optionList[i];
                option.IsSelected = false;
                option.Position = pos;
                pos.Y += NewLine(option.BoundingBox);
            }

            BoundingBox = new RectangleF(Position.X, Position.Y, Width, Height);

            if (SelectedIndex != -1)
            {
                optionList[SelectedIndex].IsSelected = true;
                optionSelector.Position = optionList[SelectedIndex].BoundingBox.GetPoint(RectanglePoint.Left, -1, -.75f);
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp, RemizioneGame.Effects.ColorReduction.Effect);

            if (ShowSelector && optionList.Count > 0 && SelectInputBinding == null)
                optionSelector.Draw(gameTime);

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
                HoveredOption = GetOptionAt(InputManager.DefaultPlayer.Mouse.VirtualPosition);

                if (HoveredOption != null)
                {
                    if (HoveredOption.Index != SelectedIndex)
                    {
                        SelectedIndex = HoveredOption.Index;
                        Sound.Play(SoundNames.UIHover);
                        HoveredOption.Shake();
                    }
                }
            }

            stick.Stick = GamePadThumbStick.Left;
            stick.Update(gameTime);
            optionSelector.Update(gameTime);
            optionSelector.Color = OptionSelectedColor;

            for (var i = 0; i < optionList.Count; i++)
            {
                optionList[i].Update(gameTime);
            }
        }

        #endregion

        // AddOption
        public UIContextMenuOption<T> AddOption(T key, string text, AtlasImage? icon = null)
        {
            UIContextMenuOption<T> result = new(this, key, text, icon);
            optionList.Add(result);

            if (SelectedIndex == -1)
                SelectedIndex = 0;

            Invalidate();

            return result;
        }

        // BoundingBox
        public RectangleF BoundingBox { get; private set; }

        // Clear
        public void Clear()
        {
            optionList.Clear();
            SelectedIndex = -1;
        }

        // Font
        public Font Font { get; }

        // GetOptionAt
        public UIContextMenuOption<T>? GetOptionAt(Vector2 position)
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

        // Height
        public int Height { get; private set; }

        // HoveredOption
        public UIContextMenuOption<T>? HoveredOption { get; private set; }

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

            Sound.Play(SoundNames.UIHover);

            SelectedOption?.Shake();

            return true;
        }

        // OptionColor
        public Color OptionColor { get; set; } = ColorPalette.Text.Default;

        // Options
        public ReadOnlyCollection<UIContextMenuOption<T>> Options { get; }

        // OptionSelectedColor
        public Color OptionSelectedColor { get; set; } = ColorPalette.Text.Highlight;

        // OptionSelectorImage
        public AtlasImage? OptionSelectorImage
        {
            get => optionSelector.Image;
            set => optionSelector.Image = value;
        }

        // OptionTextScale
        public Vector2 OptionTextScale
        {
            get => optionTextScale;
            set
            {
                if (value != optionTextScale)
                {
                    optionTextScale = value;
                    Invalidate();
                }
            }
        }

        // Position
        public Vector2 Position
        {
            get => position;
            set
            {
                if (value != position)
                {
                    position = value;
                    Invalidate();
                }
            }
        }

        // Previous
        public bool Previous()
        {
            if (optionList.Count <= 1)
                return false;

            if (SelectedIndex == 0)
                SelectedIndex = optionList.Count - 1;
            else
                SelectedIndex--;

            Sound.Play(SoundNames.UIHover);

            SelectedOption?.Shake();

            return true;
        }

        // SelectedIndex
        public int SelectedIndex
        {
            get => selectedIndex;
            set
            {
                if (value != selectedIndex)
                {
                    this.selectedIndex = value;
                    Invalidate();
                }
            }
        }

        // SelectedOption
        public UIContextMenuOption<T>? SelectedOption => selectedIndex == -1 ? null : optionList[selectedIndex];

        // SelectInputBinding
        public InputBinding? SelectInputBinding { get; set; }

        // ShowSelector
        public bool ShowSelector { get; set; } = true;

        // Width
        public int Width { get; private set; }

        // X
        public float X
        {
            get => position.X;
            set
            {
                if (value != position.X)
                {
                    this.position.X = value;
                    Invalidate();
                }
            }
        }

        // Y
        public float Y
        {
            get => position.Y;
            set
            {
                if (value != position.Y)
                {
                    this.position.Y = value;
                    Invalidate();
                }
            }
        }
    }
}
