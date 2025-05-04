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
    /// ContextMenu
    /// </summary>
    public sealed class ContextMenu : GameObject, IInputHandler
    {
        #region Private fields

        private readonly List<ContextMenuOption> optionList = [];
        private readonly UIControl optionSelectorControl;
        private readonly ImageSprite optionSelector;
        private Vector2 optionTextScale = ScaleInfo.ContextMenu.Option;
        private Vector2 position;
        private int selectedIndex = -1;
        private readonly StickInputController stick = new(GamePadThumbStick.Left) { AutoRepeatRate = 200 };
        private readonly TextSprite titleSprite;

        #endregion

        #region Constructor

        // Constructor
        public ContextMenu(EngendroGame game, Camera camera, Font? font = null)
            : base(game)
        {
            this.Camera = camera;
            this.Font = font ?? Fonts.Main;

            // Title
            titleSprite = new TextSprite(game, Font)
            {
                Color = ColorPalette.ContextMenu.Title,
                PivotOrigin = RectanglePoint.LeftTop,
                Scale = ScaleInfo.ContextMenu.Title
            };

            // Option Selector
            optionSelector = new ImageSprite(game, Atlases.UI.ContextMenuOptionSelector)
            {
                PivotOrigin = RectanglePoint.Right
            };

            optionSelectorControl = new UIControl(Game, null)
            {
                DisplayMode = UIControlDisplayMode.ImageOnly,
                PivotOrigin = RectanglePoint.Right,
                Size = UIControlSize.Small
            };

            this.Options = new ReadOnlyCollection<ContextMenuOption>(optionList);
        }

        #endregion

        #region Private members

        // Invalidate
        private void Invalidate()
        {
            float NewLine(RectangleF bbox)
            {
                Width = Math.Max(Width, bbox.Width);
                Height += bbox.Height - 1;
                return bbox.Height - 1;
            }

            Height = 0;
            Width = 0;
            var pos = Position;

            if (!titleSprite.IsEmpty)
            {
                titleSprite.Position = pos;
                pos.Y += NewLine(titleSprite.BoundingBox);
            }

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
                optionSelector.Position = optionList[SelectedIndex].BoundingBox.GetPoint(RectanglePoint.Left, -1, -.5f);

                if (optionSelectorControl != null)
                    optionSelectorControl.Position = optionSelector.Position;
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Camera, SamplerState.PointClamp, RemizioneGame.Effects.ColorReduction.Effect);

            titleSprite.Draw(gameTime);

            if (ShowSelector && optionList.Count > 0 && SelectInputBinding == null)
                optionSelector.Draw(gameTime);

            for (var i = 0; i < optionList.Count; i++)
            {
                optionList[i].Draw(gameTime);
            }

            Game.SpriteBatch.End();

            if (optionList.Count > 0 && SelectInputBinding != null)
                optionSelectorControl.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (InputManager.DefaultPlayer.LastInputMethod == InputMethod.Mouse)
            {
                if (GetOptionAt(InputManager.DefaultPlayer.Mouse.WorldPosition(Camera)) is ContextMenuOption option)
                    SelectedIndex = option.Index;
            }

            stick.Stick = GamePadThumbStick.Left;
            stick.Update(gameTime);
            optionSelectorControl?.Update(gameTime);
            optionSelector.Update(gameTime);
            titleSprite.Update(gameTime);

            for (var i = 0; i < optionList.Count; i++)
            {
                optionList[i].Update(gameTime);
            }
        }

        #endregion

        // AddOption
        public ContextMenuOption AddOption(string key, string text, AtlasImage? icon = null)
        {
            ContextMenuOption result = new(this, optionList.Count, key, text, icon);
            optionList.Add(result);
            Invalidate();

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
            titleSprite.Text = null;
        }

        // Font
        public Font Font { get; }

        // GetOptionAt
        public ContextMenuOption? GetOptionAt(Vector2 position)
        {
            if (!BoundingBox.Contains(position))
                return null;

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

            Sound.Play(SoundNames.UINavigation);

            SelectedOption?.Shake();

            return true;
        }

        // OptionCount
        public int OptionCount => optionList.Count;

        // Options
        public ReadOnlyCollection<ContextMenuOption> Options { get; }

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

            Sound.Play(SoundNames.UINavigation);

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
        public ContextMenuOption? SelectedOption => selectedIndex == -1 ? null : optionList[selectedIndex];

        // SelectInputBinding
        public InputBinding? SelectInputBinding
        {
            get => optionSelectorControl.InputBinding;
            set => optionSelectorControl.InputBinding = value;
        }

        // ShowSelector
        public bool ShowSelector { get; set; } = true;

        // Title
        public string Title
        {
            get => titleSprite.Text ?? string.Empty;
            set
            {
                titleSprite.Text = value;
                Invalidate();
            }
        }

        // TitleBoundingBox
        public RectangleF TitleBoundingBox => titleSprite.BoundingBox;

        // TitleColor
        public Color TitleColor
        {
            get => titleSprite.Color;
            set => titleSprite.Color = value;
        }

        // TitleTextScale
        public Vector2 TitleTextScale
        {
            get => titleSprite.Scale;
            set
            {
                if (value != titleSprite.Scale)
                {
                    titleSprite.Scale = value;
                    Invalidate();
                }
            }
        }

        // Width
        public float Width { get; private set; }

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
