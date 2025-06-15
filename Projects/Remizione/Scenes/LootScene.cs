using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Remizione.UI;

namespace Remizione
{
    /// <summary>
    /// LootScene
    /// </summary>
    public sealed class LootScene : Scene
    {
        #region Private fields

        private PopupMenu<Item> activeMenu;
        private readonly UIInfoPanel infoPanel;
        private const int margin = 22;
        private readonly UIControl moveButton;
        private readonly ImageSprite playerContainer;
        private readonly ImageSprite selectionContainer;
        private readonly PopupMenu<Item> playerMenu;
        private readonly GameSession session;
        private readonly ImageSprite targetContainer;
        private readonly PopupMenu<Item> targetMenu;

        #endregion

        #region Constructor

        // Constructor
        public LootScene(GameSession session)
            : base(session.Game, SceneSettings.None)
        {
            this.session = session;

            this.infoPanel = new(Game) { ShowGradient = true };

            // Player container
            this.playerContainer = new(Game, Atlases.UI.ItemMenuContainer)
            {
                PivotOrigin = RectanglePoint.LeftTop,
                Position = new Vector2(Screen.Area.Left + margin, 20),
                Scale = ScaleInfo.UIElement.Medium
            };

            // Player menu
            this.playerMenu = new(Game, HorizontalAlignment.Center, false, playerContainer.BoundingBox)
            {
                Position = playerContainer.BoundingBox.GetPoint(RectanglePoint.Top, 0, 5),
                OnSelectionChanged = SelectedOptionChanged,
                TitlePosition = playerContainer.BoundingBox.GetPoint(RectanglePoint.Top, 0, -1),
            };

            // Target container
            this.targetContainer = new(Game, Atlases.UI.ItemMenuContainer)
            {
                PivotOrigin = RectanglePoint.RightTop,
                Position = new Vector2(Screen.Area.Right - margin, 20),
                Scale = ScaleInfo.UIElement.Medium
            };

            // Target menu
            this.targetMenu = new(Game, HorizontalAlignment.Center, true, targetContainer.BoundingBox)
            {
                Position = targetContainer.BoundingBox.GetPoint(RectanglePoint.Top, 0, 5),
                OnSelectionChanged = SelectedOptionChanged,
                TitlePosition = targetContainer.BoundingBox.GetPoint(RectanglePoint.Top, 0, -1),
            };

            // Selection container
            this.selectionContainer = new(Game, Atlases.UI.ItemMenuContainerSelection)
            {
                Opacity = .2f,
                PivotOrigin = RectanglePoint.Middle,
                Scale = ScaleInfo.UIElement.Medium
            };

            // Move button
            this.moveButton = new UIControl(Game, InputBindings.Interact)
            {
                PivotOrigin = RectanglePoint.Middle,
                X = Screen.Area.Center.X,
                Y = playerContainer.BoundingBox.GetPoint(RectanglePoint.Middle).Y,
                TextColor = ColorPalette.Text.Default,
                TextScale = ScaleInfo.Text.VeryLarge,
                Text = "Move"
            };

            activeMenu = targetMenu;
        }

        #endregion

        #region Private members

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
                if (playerMenu.BoundingBox.Contains(InputManager.DefaultPlayer.Mouse.VirtualPosition))
                {
                    activeMenu = playerMenu;
                    activeMenu.SelectOptionAt(InputManager.DefaultPlayer.Mouse.VirtualPosition);
                    InvalidateInfo();
                }

                else if (targetMenu.BoundingBox.Contains(InputManager.DefaultPlayer.Mouse.VirtualPosition))
                {
                    activeMenu = targetMenu;
                    activeMenu.SelectOptionAt(InputManager.DefaultPlayer.Mouse.VirtualPosition);
                    InvalidateInfo();
                }

                return true;
            }

            return false;
        }

        // InvalidateInfo
        private void InvalidateInfo()
        {
            if (activeMenu.SelectedOption != null)
            {
                infoPanel.Title = activeMenu.SelectedOption.LinkedObject.GetLocalizedInfo();
                infoPanel.Text = activeMenu.SelectedOption.LinkedObject.MetaItem.LocalizedDescription;
            }
            else
                infoPanel.Text = null;
        }

        // InvalidateTitles
        private void InvalidateTitles()
        {
            
            if (session.Player != null)
            {
                var title = TextRepository.GetValue("Misc.Inventory");
                title += $" ({playerMenu.Options.Count} / {session.Player.InventorySize})";
                playerMenu.Title = title;

                if (IsPlayerInventoryFull)
                    playerMenu.TitleColor = ColorPalette.Text.Terra;
                else
                    playerMenu.ResetColors();
            }

            if (Target != null)
            {
                var title = TextRepository.GetValue(Target.DisplayName);
                if (Target.InventorySize > 0)
                    title += $" ({targetMenu.Options.Count} / {Target.InventorySize})";
                targetMenu.Title = title;

                if (IsTargetInventoryFull)
                    targetMenu.TitleColor =  ColorPalette.Text.Terra;
                else
                    targetMenu.ResetColors();
            }
        }

        // IsPlayerInventoryFull}
        private bool IsPlayerInventoryFull => session.Player != null && playerMenu.Options.Count >= session.Player.InventorySize;

        // IsTargetInventoryFull}
        private bool IsTargetInventoryFull => Target != null && targetMenu.Options.Count >= Target.InventorySize;

        // MoveItem
        private void MoveItem(Actor actor)
        {
            if (activeMenu.SelectedOption?.LinkedObject is Item item)
            {
                if (activeMenu == targetMenu)
                {
                    playerMenu.AddOption(item);
                    targetMenu.RemoveSelectedOption();
                    if (targetMenu.Options.Count == 0)
                        activeMenu = playerMenu;
                }
                else
                {
                    targetMenu.AddOption(item);
                    playerMenu.RemoveSelectedOption();
                    if (playerMenu.Options.Count == 0)
                        activeMenu = targetMenu;
                }

                InvalidateTitles();
            }
        }

        // SelectedOptionChanged
        private void SelectedOptionChanged(PopupMenuOption<Item>? option)
        {
            InvalidateInfo();
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            Game.Shapes.DrawRectangle(Screen.Area, ColorPalette.SceneShade);
            playerContainer.Draw(gameTime);
            targetContainer.Draw(gameTime);

            if (activeMenu.SelectedOption != null)
            {
                selectionContainer.Position = activeMenu.SelectedOption.Position;
                selectionContainer.Draw(gameTime);
            }

            Game.SpriteBatch.End();

            playerMenu.Draw(gameTime);
            targetMenu.Draw(gameTime);

            infoPanel.Draw(gameTime);

            moveButton.Draw(gameTime);
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            if (session.Player == null)
                return HandleInputResult.Unhandled;

            if (activeMenu.HandleInput(gameTime) == HandleInputResult.Handled)
                return HandleInputResult.Handled;

            if (moveButton.TestPressed(PlayerIndex.One))
                MoveItem(session.Player);

            if (HandleMouseInput())
                return HandleInputResult.Handled;

            return base.OnHandleInput(gameTime);
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            playerMenu.Clear();
            MouseCursor.Instance.State = MouseCursorState.Arrow;

            if (session.Player == null || Target == null)
                return;

            // Player items
            for (int i = 0; i < session.Player.Inventory.Items.Count; i++)
            {
                playerMenu.AddOption(session.Player.Inventory.Items[i]);
            }

            // Target items
            for (int i = 0; i < Target.Inventory.Items.Count; i++)
            {
                targetMenu.AddOption(Target.Inventory.Items[i]);
            }

            InvalidateTitles();

            playerMenu.SelectFirst();
            targetMenu.SelectFirst();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            moveButton.IsEnabled = activeMenu.SelectedOption != null;
            if (moveButton.IsEnabled)
            {
                if (activeMenu == playerMenu && IsTargetInventoryFull ||
                    activeMenu == targetMenu && IsPlayerInventoryFull)
                    moveButton.IsEnabled = false;
            }

            playerMenu.HideSelectedOption = activeMenu != playerMenu;
            targetMenu.HideSelectedOption = activeMenu != targetMenu;

            playerMenu.Update(gameTime);
            targetMenu.Update(gameTime);

            moveButton.Update(gameTime);
        }

        #endregion

        // Target
        public GameThing? Target { get; set; }
    }
}
