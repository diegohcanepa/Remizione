using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ScaryCastle.Menus
{
    /// <summary>
    /// MessageBoxScene
    /// </summary>
    public partial class MessageBoxScene : Scene
    {
        #region Private fields

        private readonly Sprite bottomOrnament;
        private readonly Menu menu;
        private readonly TextSprite message;
        private readonly TextSprite subMessage;
        private readonly TextSprite title;
        private readonly Sprite topOrnament;

        #endregion

        #region Constructor

        // Constructor
        public MessageBoxScene(ScaryCastleGame game, string text)
            : this(game, text, MessageBoxOptions.Accept, null, MessageBoxOptions.Accept)
        {
        }

        // Constructor
        public MessageBoxScene(ScaryCastleGame game, MessageBoxOptions options, Action<MessageBoxOptions>? onSelect, MessageBoxOptions defaultOption)
            : this(game, string.Empty, options, onSelect, defaultOption)
        {
        }

        // Constructor
        public MessageBoxScene(ScaryCastleGame game, string text, MessageBoxOptions options, Action<MessageBoxOptions>? onSelect, MessageBoxOptions defaultOption)
        {
            this.Options = options;
            this.PausePreviousScenes = true;
            this.OnSelect = onSelect;

            BackgroundColor = Color.Black;

            // Message
            message = new(Fonts.Common)
            {
                Color = ColorPalette.TextWhite,
                MaximumWidth = 250,
                PivotOrigin = RectanglePoint.Top,
                Scale = ScaleInfo.MessageBoxMessage,
                Text = text
            };

            // SubMessage
            subMessage = new(Fonts.Common)
            {
                Color = ColorPalette.TextWhite,
                MaximumWidth = 200,
                PivotOrigin = RectanglePoint.Top,
                Scale = ScaleInfo.MessageBoxSubMessage
            };

            // Title
            title = new(Fonts.Common)
            {
                Color = ColorPalette.TextWhite,
                PivotOrigin = RectanglePoint.Bottom,
                Scale = ScaleInfo.MessageBoxTitle
            };

            // Top Ornament
            topOrnament = new(Atlases.Menu.MessageBoxOrnamentTop)
            {
                PivotOrigin = RectanglePoint.Bottom,
            };

            // Bottom Ornament
            bottomOrnament = new(Atlases.Menu.MessageBoxOrnamentBottom)
            {
                PivotOrigin = RectanglePoint.Top
            };

            // Menu
            menu = new Menu(game)
            {
            };

            MenuItem? defaultItem = null;

            // Accept
            if (options.HasFlag(MessageBoxOptions.Accept))
            {
                menu.AddItem(MenuItemName.Accept, () => SelectOption(MessageBoxOptions.Accept), null);
                if (defaultOption == MessageBoxOptions.Accept)
                {
                    defaultItem = menu.Items[^1];
                }
            }

            // Yes
            if (options.HasFlag(MessageBoxOptions.Yes))
            {
                menu.AddItem(MenuItemName.Yes, () => SelectOption(MessageBoxOptions.Yes), null);
                if (defaultOption == MessageBoxOptions.Yes)
                {
                    defaultItem = menu.Items[^1];
                }
            }

            // No
            if (options.HasFlag(MessageBoxOptions.No))
            {
                menu.AddItem(MenuItemName.No, () => SelectOption(MessageBoxOptions.No), null);
                if (defaultOption == MessageBoxOptions.No)
                {
                    defaultItem = menu.Items[^1];
                }
            }

            // Cancel
            if (options.HasFlag(MessageBoxOptions.Cancel))
            {
                menu.AddItem(MenuItemName.Cancel, () => SelectOption(MessageBoxOptions.Cancel), null);
                if (defaultOption == MessageBoxOptions.Cancel)
                {
                    defaultItem = menu.Items[^1];
                }
            }

            if (defaultItem != null)
            {
                menu.SelectedItem = defaultItem;
            }
        }

        #endregion

        #region Private members

        // Invalidate
        private void Invalidate()
        {
            message.Position = new Vector2(Screen.Center.X, 80);
            var y = message.BoundingBox.GetPoint(RectanglePoint.Bottom).Y;

            if (!subMessage.IsEmpty)
            {
                subMessage.Position = message.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, 2);
                y = subMessage.BoundingBox.GetPoint(RectanglePoint.Bottom).Y + 2;
            }

            menu.Position = new Vector2(Screen.Center.X, y + 10);

            topOrnament.Position = message.BoundingBox.GetPoint(RectanglePoint.Top, 0, -9);
            bottomOrnament.Position = menu.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, 9);

            if (!title.IsEmpty)
            {
                title.Position = topOrnament.BoundingBox.GetPoint(RectanglePoint.Top, 0, -3);
            }
        }

        // SelectOption
        private void SelectOption(MessageBoxOptions option)
        {
            if (AutoPopScene)
            {
                Game.SceneManager.Pop();
            }

            OnSelect?.Invoke(option);
        }

        #endregion

        #region Protected members

        // AllowBack
        protected bool AllowBack { get; set; } = true;

        // AutoPopScene
        protected bool AutoPopScene { get; set; } = true;

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            menu.Draw(gameTime);

            Game.SpriteBatch.Begin(Game.Camera);
            topOrnament.Draw(gameTime);
            bottomOrnament.Draw(gameTime);
            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp);
            title.Draw(gameTime);
            message.Draw(gameTime);
            subMessage.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput()
        {
            if (TransitionManager.CurrentTransition.IsRunning)
            {
                return HandleInputResult.Handled;
            }

            if (AllowBack && InputBindings.Back.IsPressed(0))
            {
                Game.SceneManager.Pop();
                return HandleInputResult.Handled;
            }

            return menu.HandleInput();
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            base.OnLoadContent();
            Invalidate();
        }

        // OnSelect
        protected Action<MessageBoxOptions>? OnSelect { get; set; }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            menu.Update(gameTime);
            title.Update(gameTime);
            message.Update(gameTime);
            subMessage.Update(gameTime);
        }

        #endregion

        // Message
        public string? Message
        {
            get => message.Text;
            set
            {
                message.Text = value;
                Invalidate();
            }
        }

        // MessageColor
        public Color MessageColor
        {
            get => message.Color;
            set => message.Color = value;
        }

        // Options
        public MessageBoxOptions Options { get; }

        // SubMessage
        public string? SubMessage
        {
            get => subMessage.Text;
            set
            {
                subMessage.Text = value;
                Invalidate();
            }
        }

        // SubMessageColor
        public Color SubMessageColor
        {
            get => subMessage.Color;
            set => subMessage.Color = value;
        }

        // Title
        public string? Title
        {
            get => title.Text;
            set
            {
                title.Text = value;
                Invalidate();
            }
        }

        // TitleColor
        public Color TitleColor
        {
            get => title.Color;
            set => title.Color = value;
        }
    }
}
