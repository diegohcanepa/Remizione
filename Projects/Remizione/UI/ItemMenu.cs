using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        private readonly List<ItemMenuOption> optionList = [];
        private readonly StickInputController stick = new(GamePadThumbStick.Left) { AutoRepeatRate = 200 };
        private readonly TextSprite titleText;

        #endregion

        #region Constructor

        // Constructor
        public ItemMenu(EngendroGame game, Font? font = null)
            : base(game)
        {
            this.Font = font ?? Fonts.CommonOutline;
            this.Options = new ReadOnlyCollection<ItemMenuOption>(optionList);

            // Container
            this.container = new(game, Atlases.UI.ItemMenuContainer)
            {
                PivotOrigin = RectanglePoint.Top,
                Scale = new(.75f)
            };

            // Title
            this.titleText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Terra,
                PivotOrigin = RectanglePoint.Bottom,
                Scale = ScaleInfo.Text.Large
            };
        }

        #endregion

        #region Private members

        // LayoutOptions
        private void LayoutOptions()
        {
            var pos = container.BoundingBox.GetPoint(RectanglePoint.Top, 0, 8);

            for (var i = 0; i < optionList.Count; i++)
            {
                var option = optionList[i];
                option.Position = pos;
                pos.Y += option.TextBoundingBox.Height;
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            container.Draw(gameTime);
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
            {
                if (GetOptionAt(InputManager.DefaultPlayer.Mouse.VirtualPosition) is ItemMenuOption option)
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

            return HandleInputResult.Unhandled;
        }

        // HasOptions
        public bool HasOptions => optionList.Count > 0;

        // Hide
        public void Hide()
        {
            IsVisible = false;
        }

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
                LayoutOptions();
            }
        }

        // SelectedOption
        public ItemMenuOption? SelectedOption { get; private set; }

        // Show
        public void Show(Vector2 position, string title)
        {
            IsVisible = true;

            container.Position = position;

            LayoutOptions();

            titleText.Text = title;
            titleText.Position = container.BoundingBox.GetPoint(RectanglePoint.Top);
        }

        // TextScale
        public Vector2 TextScale { get; set; } = ScaleInfo.Text.Large;

        // Title
        public string? Title
        {
            get => titleText.Text;
            set => titleText.Text = value;
        }
    }
}
