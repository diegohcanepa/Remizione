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
    /// ItemMenu
    /// </summary>
    public sealed class ItemMenu : GameObject, IInputHandler
    {
        #region Private fields

        private readonly ImageSprite container;
        private readonly ImageSprite containerSelection;
        private readonly List<ItemMenuOption> optionList = [];
        private ItemMenuOption? selectedOption;
        private Action<ItemMenuOption?>? selectedOptionChanged;
        private readonly StickInputController stick = new(GamePadThumbStick.Left) { AutoRepeatRate = 200 };
        private readonly TextSprite titleText;

        #endregion

        #region Constructor

        // Constructor
        public ItemMenu(EngendroGame game, Action<ItemMenuOption?>? selectedOptionChanged)
            : base(game)
        {
            this.selectedOptionChanged = selectedOptionChanged;
            this.Font = Fonts.CommonOutline;
            this.Options = new ReadOnlyCollection<ItemMenuOption>(optionList);

            // Container
            this.container = new(game, Atlases.UI.ItemMenuContainer)
            {
                PivotOrigin = RectanglePoint.Top,
                Scale = ScaleInfo.UIElement.Medium
            };

            // ContainerSelection
            this.containerSelection = new(game, Atlases.UI.ItemMenuContainerSelection)
            {
                Opacity = .2f,
                PivotOrigin = RectanglePoint.Middle,
                Scale = ScaleInfo.UIElement.Medium
            };

            // Title
            this.titleText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Bottom,
                Scale = ScaleInfo.Text.VeryLarge
            };
        }

        #endregion

        #region Private members

        // LayoutOptions
        private void LayoutOptions()
        {
            var pos = container.BoundingBox.GetPoint(RectanglePoint.Top, 0, 9);

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
            Game.SpriteBatch.Begin(Game.Camera);
            container.Draw(gameTime);

            if (SelectedOption != null)
            {
                containerSelection.Position = SelectedOption.Position;
                containerSelection.Draw(gameTime);
            }

            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);

            for (var i = 0; i < optionList.Count; i++)
            {
                optionList[i].Draw(gameTime);
            }
            
            titleText.Draw(gameTime);
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
        public ItemMenuOption AddOption(Item item)
        {
            ItemMenuOption result = new(this, item);
            optionList.Add(result);
            optionList.Sort((a, b) => a.ToString().CompareTo(b.ToString()));

            return result;
        }

        // BoundingBox
        public RectangleF BoundingBox => container.BoundingBox;

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
        public ItemMenuOption? GetOptionAt(Vector2 position)
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
                return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
        }

        // HasOptions
        public bool HasOptions => optionList.Count > 0;

        // Hide
        public void Hide()
        {
            IsVisible = false;
        }

        // HoveredOption
        public ItemMenuOption? HoveredOption { get; private set; }

        // IsActiveInGameLoop
        public override bool IsActiveInGameLoop => IsVisible;

        // IsVisible
        public bool IsVisible { get; private set; }

        // Options
        public ReadOnlyCollection<ItemMenuOption> Options { get; }

        // RemoveSelectedOption
        public void RemoveSelectedOption()
        {
            if (SelectedOption != null)
            {
                optionList.Remove(SelectedOption);
                SelectedOption = null;
                LayoutOptions();
            }
        }

        // Select
        public void Select(Item item)
        {
            for (var i = 0; i < optionList.Count; i++)
            {
                if (optionList[i].Item == item)
                {
                    SelectedOption = optionList[i];
                    return;
                }
            }
        }

        // SelectedOption
        public ItemMenuOption? SelectedOption
        {
            get => selectedOption;
            set
            {
                if (value != selectedOption)
                {
                    if (selectedOption != null && value != null)
                        Sound.Play(SoundNames.MenuSelect);

                    selectedOption = value;
                    selectedOptionChanged?.Invoke(selectedOption);
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

        // Show
        public void Show(Vector2 position, string title)
        {
            IsVisible = true;

            container.Position = position;

            LayoutOptions();

            titleText.Text = title;
            titleText.Position = container.BoundingBox.GetPoint(RectanglePoint.Top, 0, 2);
        }

        // TextScale
        public Vector2 TextScale { get; set; } = ScaleInfo.Text.Large;

        // Title
        public string? Title
        {
            get => titleText.Text;
            set => titleText.Text = value;
        }

        // VerticalSpacing
        public float VerticalSpacing { get; set; } = 1;
    }
}
