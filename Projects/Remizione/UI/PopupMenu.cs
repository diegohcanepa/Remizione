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
    /// PopupMenu
    /// </summary>
    public sealed class PopupMenu<TLinkedObject> : GameObject, IInputHandler where TLinkedObject : class
    {
        #region Private fields

        private readonly List<PopupMenuOption<TLinkedObject>> optionList = [];
        private Vector2 position;
        private PopupMenuOption<TLinkedObject>? selectedOption;
        private readonly bool sorted;
        private readonly StickInputController stick = new(GamePadThumbStick.Left) { AutoRepeatRate = 200 };
        private Vector2 textScale = ScaleInfo.Text.Large;
        private readonly TextSprite titleText;

        #endregion

        #region Constructor

        // Constructor
        public PopupMenu(EngendroGame game, HorizontalAlignment horizontalAlignment, bool sorted, RectangleF? boundingBox = null)
            : base(game)
        {
            this.sorted = sorted;
            this.HorizontalAlignment = horizontalAlignment;
            this.BoundingBox = boundingBox ?? RectangleF.Empty;
            this.Font = Fonts.CommonOutline;
            this.Options = new ReadOnlyCollection<PopupMenuOption<TLinkedObject>>(optionList);

            // Title
            this.titleText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Bottom,
                Scale = TextScale
            };
        }

        #endregion

        #region Private members

        // HandleMouseInput
        private bool HandleMouseInput()
        {
            if (HoveredOption != null && InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
            {
                SelectedOption = HoveredOption;
                SelectedOption.Execute();
                return true;
            }

            return false;
        }

        // Layout
        private void Layout()
        {
            var pos = Position;
            
            for (var i = 0; i < optionList.Count; i++)
            {
                var option = optionList[i];
                option.Position = pos;
                pos.Y += option.TextBoundingBox.Height + VerticalSpacing;
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);

            titleText.Draw(gameTime);

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

            stick.Stick = GamePadThumbStick.Left;
            stick.Update(gameTime);

            for (var i = 0; i < optionList.Count; i++)
            {
                optionList[i].Update(gameTime);
            }
        }

        #endregion

        // AddOption
        public PopupMenuOption<TLinkedObject> AddOption(TLinkedObject obj, Action? action = null)
        {
            PopupMenuOption<TLinkedObject> result = new(this, obj, action);
            optionList.Add(result);

            if (sorted)
                optionList.Sort((a, b) => a.ToString().CompareTo(b.ToString()));

            Layout();

            return result;
        }

        // BoundingBox
        public RectangleF BoundingBox { get; }

        // CanHandleInput
        public bool CanHandleInput => true;

        // Clear
        public void Clear()
        {
            optionList.Clear();
            HoveredOption = null;
            SelectedOption = null;
        }

        // Font
        public Font Font { get; }

        // GetOptionAt
        public PopupMenuOption<TLinkedObject>? GetOptionAt(Vector2 position)
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

            if (HandleMouseInput())
                return HandleInputResult.Handled;

            if (InputBindings.NextMenuItem.IsPressed(PlayerIndex.One) || stick.IsDown(0))
            {
                SelectNext();
                return HandleInputResult.Handled;
            }

            if (InputBindings.PreviousMenuItem.IsPressed(PlayerIndex.One) || stick.IsUp(0))
            {
                SelectPrevious();
                return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
        }

        // HasOptions
        public bool HasOptions => optionList.Count > 0;

        // HideSelectedOption
        public bool HideSelectedOption { get; set; }

        // HorizontalAlignment
        public HorizontalAlignment HorizontalAlignment { get; }

        // HoveredOption
        public PopupMenuOption<TLinkedObject>? HoveredOption { get; private set; }

        // OnPressed
        public Action<PopupMenuOption<TLinkedObject>>? OnPressed { get; set; }

        // OnSelectionChanged
        public Action<PopupMenuOption<TLinkedObject>?>? OnSelectionChanged { get; set; }

        // Options
        public ReadOnlyCollection<PopupMenuOption<TLinkedObject>> Options { get; }

        // Position
        public Vector2 Position
        {
            get => position;
            set
            {
                position = value;
                Layout();
            }
        }

        // RemoveSelectedOption
        public void RemoveSelectedOption()
        {
            if (SelectedOption != null)
            {
                var index = optionList.IndexOf(SelectedOption);
                optionList.Remove(SelectedOption);

                if (optionList.Count == 0)
                    SelectedOption = null;
                else if (index < optionList.Count)
                    Select(optionList[index].LinkedObject);
                else
                    SelectFirst();

                Layout();
            }
        }

        // Select
        public void Select(TLinkedObject obj)
        {
            for (var i = 0; i < optionList.Count; i++)
            {
                if (optionList[i].LinkedObject == obj)
                {
                    SelectedOption = optionList[i];
                    return;
                }
            }
        }

        // SelectFirst
        public void SelectFirst()
        {
            if (optionList.Count == 0)
                return;

            SelectedOption = optionList[0];
        }

        // SelectNext
        public void SelectNext()
        {
            if (optionList.Count == 0)
                return;

            if (SelectedOption == null)
            {
                SelectFirst();
                return;
            }

            var index = optionList.IndexOf(SelectedOption);
            index++;
            if (index == optionList.Count)
                index = 0;

            SelectedOption = optionList[index];
        }

        // SelectedOption
        public PopupMenuOption<TLinkedObject>? SelectedOption
        {
            get => selectedOption;
            set
            {
                if (value != selectedOption)
                {
                    if (selectedOption != null && value != null)
                        Sound.Play(SoundNames.MenuSelect);

                    selectedOption = value;
                    OnSelectionChanged?.Invoke(selectedOption);
                }
            }
        }

        // SelectOptionAt
        public PopupMenuOption<TLinkedObject>? SelectOptionAt(Vector2 position)
        {
            var option = GetOptionAt(position);
            if(option != null)
                SelectedOption = option;

            return option;
        }

        // SelectPrevious
        public void SelectPrevious()
        {
            if (optionList.Count == 0)
                return;

            if (SelectedOption == null)
            {
                SelectFirst();
                return;
            }

            var index = optionList.IndexOf(SelectedOption);
            index--;
            if (index == -1)
                index = optionList.Count - 1;

            SelectedOption = optionList[index];
        }

        // Sorted
        public bool Sorted { get; }

        // TextScale
        public Vector2 TextScale
        {
            get => textScale;
            set
            {
                if (value != textScale)
                {
                    textScale = value;

                    titleText.Scale = textScale;

                    for (var i = 0; i < optionList.Count; i++)
                    {
                        optionList[i].Invalidate();
                    }
                }
            }
        }

        // Title
        public string? Title
        {
            get => titleText.Text;
            set => titleText.Text = value;
        }

        // TitlePosition
        public Vector2 TitlePosition
        {
            get => titleText.Position;
            set => titleText.Position = value;
        }

        // VerticalSpacing
        public float VerticalSpacing { get; set; } = 1.5f;
    }
}
