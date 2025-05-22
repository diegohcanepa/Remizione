using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Remizione.UI;

namespace Remizione
{
    /// <summary>
    /// InventoryScene
    /// </summary>
    public sealed class InventoryScene : Scene
    {
        private readonly UIControl actionButton;
        private readonly ImageSprite container;
        private readonly ImageSprite containerSelection;
        private readonly UIControl discardButton;
        private readonly UIInfoPanel infoPanel;
        private readonly PopupMenu<Item> menu;

        #region Constructor

        // Constructor
        public InventoryScene(RemizioneGame game)
            : base(game, SceneSettings.None)
        {
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

            // Item action
            this.actionButton = new UIControl(game, InputBindings.Select)
            {
                PivotOrigin = RectanglePoint.RightBottom,
                TextColor = ColorPalette.Text.Default
            };

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
            if (menu.SelectedOption?.LinkedObject is Item item)
            {
                actor.Inventory.Remove(item.Name);
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
            var title = TextRepository.GetValue("Misc.Inventory");
            if (Actor != null)
                title += $" ({Actor.Inventory.Items.Count} / {Actor.InventoryCapacity})";

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
                discardButton.Draw(gameTime);
                actionButton.Draw(gameTime);
            }
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            if (Actor == null)
                return HandleInputResult.Unhandled;

            if (menu.HandleInput(gameTime) == HandleInputResult.Handled)
                return HandleInputResult.Handled;

            if (discardButton.TestPressed(PlayerIndex.One))
                DiscardItem(Actor);

            if (actionButton.TestPressed(PlayerIndex.One))
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

            if (Actor == null)
                return;

            for (int i = 0; i < Actor.Inventory.Items.Count; i++)
            {
                menu.AddOption(Actor.Inventory.Items[i]);
            }

            menu.SelectFirst();

            discardButton.Position = container.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 5, -4);
            actionButton.Position = container.BoundingBox.GetPoint(RectanglePoint.RightBottom, -5, -4);

            InvalidateTitle();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            menu.Update(gameTime);

            if (menu.SelectedOption != null)
            {
                discardButton.Update(gameTime);
                actionButton.Update(gameTime);
            }
        }

        #endregion

        // Actor
        public Actor? Actor { get; set; }
    }
}
