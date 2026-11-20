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
        private Vector2 optionTextScale = ScaleInfo.ContextMenu.Option;
        private Vector2 position;
        private readonly StickInputController stick = new(GamePadThumbStick.Left) { AutoRepeatRate = 200 };

        #endregion

        #region Constructor

        // Constructor
        public UIContextMenu(Font? font = null)
        {
            this.Font = font ?? Fonts.Common;
            this.Options = new ReadOnlyCollection<UIContextMenuOption<T>>(optionList);
        }

        #endregion

        #region Private members

        // Refresh
        private void Refresh()
        {
            int NewLine(RectangleF bbox)
            {
                var vOffset = (int)bbox.Height.Round(0);
                Width = (int)MathHelper.Max(Width, bbox.Width).Round(0);
                Height += vOffset;
                return vOffset;
            }

            Height = 0;
            Width = 0;
            var pos = Position;

            for (var i = 0; i < optionList.Count; i++)
            {
                var option = optionList[i];
                option.IsSelected = false;
                option.Position = pos;
                pos.Y += NewLine(option.BoundingBox) + OptionSpacing;
            }

            BoundingBox = new RectangleF(Position.X, Position.Y, Width, Height);

            if (SelectedIndex != -1)
                optionList[SelectedIndex].IsSelected = true;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp, RemizioneGame.Effects.ColorReduction.Effect);

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
                        Sound.Play(SoundNames.ButtonHover);
                        HoveredOption.Shake();
                    }
                }
                else
                {
                    SelectedIndex = -1;
                }
            }

            stick.Stick = GamePadThumbStick.Left;
            stick.Update(gameTime);

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

            Refresh();

            return result;
        }

        // Alignment
        public HorizontalAlignment Alignment
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    Refresh();
                }
            }
        } = HorizontalAlignment.Left;

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

            Sound.Play(SoundNames.ButtonHover);

            SelectedOption?.Shake();

            return true;
        }

        // OptionColor
        public Color OptionColor { get; set; } = Colors.Palette.Text;

        // Options
        public ReadOnlyCollection<UIContextMenuOption<T>> Options { get; }

        // OptionSelectedColor
        public Color OptionSelectedColor { get; set; } = Colors.Palette.Text;

        // OptionSpacing
        public float OptionSpacing { get; set; }

        // OptionTextScale
        public Vector2 OptionTextScale
        {
            get => optionTextScale;
            set
            {
                if (value != optionTextScale)
                {
                    optionTextScale = value;
                    Refresh();
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
                    Refresh();
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

            Sound.Play(SoundNames.ButtonHover);

            SelectedOption?.Shake();

            return true;
        }

        // SelectedIndex
        public int SelectedIndex
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    Refresh();
                }
            }
        } = -1;

        // SelectInputBinding
        public InputBinding? SelectInputBinding { get; set; }

        // SelectedOption
        public UIContextMenuOption<T>? SelectedOption => SelectedIndex == -1 ? null : optionList[SelectedIndex];

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
                    position.X = value;
                    Refresh();
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
                    position.Y = value;
                    Refresh();
                }
            }
        }
    }
}
