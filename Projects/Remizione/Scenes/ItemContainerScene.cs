using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Remizione.UI;

namespace Remizione
{
    /// <summary>
    /// ItemContainerScene
    /// </summary>
    public abstract class ItemContainerScene : Scene
    {
        #region Private fields

        private readonly UIControl actionButton;
        private readonly bool allowAction;
        private readonly bool allowDiscard;
        //private readonly UIControl closeButton;
        private readonly ImageSprite container;
        private readonly ImageSprite containerSelection;
        private readonly UIControl discardButton;
        private readonly UIInfoPanel infoPanel;
        private readonly ItemContainerCategory category;
        private readonly PopupMenu<Item> menu;

        #endregion

        #region Constructor

        // Constructor
        protected ItemContainerScene(RemizioneGame game, ItemContainerCategory category, bool allowDiscard, bool allowAction)
            : base(game, SceneSettings.None)
        {
            this.category = category;
            this.allowDiscard = allowDiscard;
            this.allowAction = allowAction;

            this.infoPanel = new(Game) { ShowGradient = true };

            // Container
            this.container = new(game, Atlases.UI.ItemMenuContainer)
            {
                PivotOrigin = RectanglePoint.Top,
                Position = new Vector2(Screen.Center.X, 14),
                Scale = ScaleInfo.UIElement.Medium
            };

            // ContainerSelection
            this.containerSelection = new(game, Atlases.UI.ItemMenuContainerSelection)
            {
                Opacity = .2f,
                PivotOrigin = RectanglePoint.Middle,
                Scale = ScaleInfo.UIElement.Medium
            };

            // Menu
            this.menu = new(Game, HorizontalAlignment.Center, true, container.BoundingBox)
            {
                Position = container.BoundingBox.GetPoint(RectanglePoint.Top, 0, 6),
                OnSelectionChanged = SelectedOptionChanged,
                TitlePosition = container.BoundingBox.GetPoint(RectanglePoint.Top),
            };

            // Action
            this.actionButton = new UIControl(game, InputBindings.Select)
            {
                PivotOrigin = RectanglePoint.RightBottom,
                TextColor = ColorPalette.Text.Default
            };

            /*
            // Close window
            this.closeButton = new UIControl(game)
            {
                DisplayMode = UIControlDisplayMode.ImageOnly,
                ImageName = "CloseWindowButton",
                PivotOrigin = RectanglePoint.LeftTop,
                Position = container.BoundingBox.GetPoint(RectanglePoint.RightTop, 2, 0),
                TextColor = ColorPalette.Text.Default
            };
            */

            // Discard
            this.discardButton = new UIControl(game, InputBindings.Select)
            {
                PivotOrigin = RectanglePoint.LeftBottom,
                Text = Localization.EncodeKey(ItemAction.Discard),
                TextColor = ColorPalette.Text.Default
            };
        }

        #endregion

        #region Private members

        // DiscardItem
        private void DiscardItem(Actor actor)
        {
            if (!allowDiscard)
                return;

            if (menu.SelectedOption?.LinkedObject is Item item)
            {
                actor.GetItemContainer(category).Remove(item.Name);
                menu.RemoveSelectedOption();
                InvalidateTitle();
                Sound.Play(SoundNames.MenuDiscardItem);
            }
        }

        // HandleMouseInput
        private bool HandleMouseInput()
        {
            if (InputManager.DefaultPlayer.LastInputMethod != InputMethod.Mouse)
                return false;

            if (InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
            {
                SceneController.Pop();
                return true;
            }

            if (InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
            {
                if (!menu.BoundingBox.Contains(InputManager.DefaultPlayer.Mouse.VirtualPosition))
                    SceneController.Pop();
                return true;
            }

            return false;
        }

        // InvalidateTitle
        private void InvalidateTitle()
        {
            var itemContainer = Actor?.GetItemContainer(category);
            if (itemContainer == null)
                return;

            var title = itemContainer.DisplayName;

            if (itemContainer.Capacity > 0)
                title += $" ({itemContainer.Items.Count} / {itemContainer.Capacity})";

            menu.Title = title;
        }

        // PerformItemAction
        private void PerformItemAction(Actor actor)
        {
            if (menu.SelectedOption?.LinkedObject is Item item && item.MetaItem.Action != ItemAction.None)
            {
                if (item.MetaItem.Action == ItemAction.Create)
                    actor.CreateItem(item);

                else if (item.MetaItem.Action == ItemAction.Use)
                    actor.UseItem(item);

                else
                    item.Use();

                SceneController.Pop();
            }
        }

        // SelectedOptionChanged
        private void SelectedOptionChanged(PopupMenuOption<Item>? option)
        {
            if (option != null)
            {
                infoPanel.Title = option.LinkedObject.GetLocalizedInfo();
                infoPanel.Text = option.LinkedObject.MetaItem.LocalizedDescription;
                actionButton.Text = Localization.EncodeKey(option.LinkedObject.MetaItem.Action);
            }
            else
                infoPanel.Text = null;

            actionButton.IsEnabled = option != null && option.LinkedObject.MetaItem.Action != ItemAction.None && option.LinkedObject.MeetUsageConditions();
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            container.Draw(gameTime);
            if (menu.SelectedOption != null)
            {
                containerSelection.Position = menu.SelectedOption.Position;
                containerSelection.Draw(gameTime);
            }
            Game.SpriteBatch.End();

            menu.Draw(gameTime);
            infoPanel.Draw(gameTime);

            if (menu.SelectedOption != null)
            {
                if (allowDiscard)
                    discardButton.Draw(gameTime);

                if (allowAction)
                    actionButton.Draw(gameTime);
            }

            //closeButton.Draw(gameTime);
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            if (Actor == null)
                return HandleInputResult.Unhandled;

            if (menu.HandleInput(gameTime) == HandleInputResult.Handled)
                return HandleInputResult.Handled;

            if (allowDiscard && discardButton.TestPressed(PlayerIndex.One))
                DiscardItem(Actor);

            if (allowAction && actionButton.TestPressed(PlayerIndex.One))
                PerformItemAction(Actor);

            if (HandleMouseInput())
                return HandleInputResult.Handled;

            return base.OnHandleInput(gameTime);
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            Sound.Play(SoundNames.InventoryOpen);

            menu.Clear();
            MouseCursor.Instance.State = MouseCursorState.Arrow;

            if (Actor?.GetItemContainer(category) is not ItemContainer itemContainer)
                return;

            for (int i = 0; i < itemContainer.Items.Count; i++)
            {
                menu.AddOption(itemContainer.Items[i]);
            }

            menu.SelectFirst();

            if (allowDiscard)
                discardButton.Position = container.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 5, -4);

            if (allowAction)
                actionButton.Position = container.BoundingBox.GetPoint(RectanglePoint.RightBottom, -5, -4);

            InvalidateTitle();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            menu.Update(gameTime);

            if (menu.SelectedOption != null)
            {
                if (allowDiscard)
                    discardButton.Update(gameTime);

                if (allowAction)
                    actionButton.Update(gameTime);
            }

            //closeButton.Update(gameTime);
        }

        #endregion

        // Actor
        public Actor? Actor { get; set; }
    }
}
