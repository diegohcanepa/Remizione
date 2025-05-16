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
        private readonly UIInfo info;
        private const int margin = 22;
        private readonly UIControl moveButton;
        private readonly ImageSprite playerContainer;
        private readonly ImageSprite playerContainerSelection;
        private readonly PopupMenu<Item> playerMenu;
        private readonly GameSession session;
        private readonly ImageSprite targetContainer;
        private readonly ImageSprite targetContainerSelection;
        private readonly PopupMenu<Item> targetMenu;


        #region Constructor

        // Constructor
        public LootScene(GameSession session)
            : base(session.Game, SceneSettings.None)
        {
            this.session = session;

            this.info = new(Game) { ShowGradient = true };

            // Player container
            this.playerContainer = new(Game, Atlases.UI.ItemMenuContainer)
            {
                PivotOrigin = RectanglePoint.LeftTop,
                Position = new Vector2(Screen.Area.Left + margin, 14),
                Scale = ScaleInfo.UIElement.Medium
            };

            // Player container selection
            this.playerContainerSelection = new(Game, Atlases.UI.ItemMenuContainerSelection)
            {
                Opacity = .2f,
                PivotOrigin = RectanglePoint.Middle,
                Scale = ScaleInfo.UIElement.Medium
            };

            // Player menu
            this.playerMenu = new(Game, HorizontalAlignment.Center, true, playerContainer.BoundingBox)
            {
                Position = playerContainer.BoundingBox.GetPoint(RectanglePoint.Top, 0, 12),
                OnSelectionChanged = SelectedOptionChanged,
                TitlePosition = playerContainer.BoundingBox.GetPoint(RectanglePoint.Top, 0, 2),
            };

            // Target container
            this.targetContainer = new(Game, Atlases.UI.ItemMenuContainer)
            {
                PivotOrigin = RectanglePoint.RightTop,
                Position = new Vector2(Screen.Area.Right - margin, 14),
                Scale = ScaleInfo.UIElement.Medium
            };

            // Target container selection
            this.targetContainerSelection = new(Game, Atlases.UI.ItemMenuContainerSelection)
            {
                Opacity = .2f,
                PivotOrigin = RectanglePoint.Middle,
                Scale = ScaleInfo.UIElement.Medium
            };

            // Target menu
            this.targetMenu = new(Game, HorizontalAlignment.Center, true, targetContainer.BoundingBox)
            {
                Position = targetContainer.BoundingBox.GetPoint(RectanglePoint.Top, 0, 12),
                OnSelectionChanged = SelectedOptionChanged,
                TitlePosition = targetContainer.BoundingBox.GetPoint(RectanglePoint.Top, 0, 2),
            };

            // Move button
            this.moveButton = new UIControl(Game, InputBindings.Select)
            {
                ContainerImage = Atlases.UI.GetImage("ButtonContainer"),
                PivotOrigin = RectanglePoint.Middle,
                X = Screen.Area.Center.X,
                Y = playerContainer.BoundingBox.GetPoint(RectanglePoint.Middle).Y,
                TextColor = ColorPalette.Text.Default,
                Text = "Borrow"
            };
        }

        #endregion

        #region Private members

        // DiscardItem
        private void DiscardItem(Actor actor)
        {
            if (playerMenu.SelectedOption?.LinkedObject is Item item)
            {
                actor.Inventory.Remove(item.Name);
                playerMenu.RemoveSelectedOption();
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
                if (!playerMenu.BoundingBox.Contains(InputManager.DefaultPlayer.Mouse.VirtualPosition))
                    SceneController.Pop();
                return true;
            }

            return false;
        }

        // InvalidateTitle
        private void InvalidateTitle()
        {
            var title = Localization.GetLocalizedValue(ItemContainerCategory.Inventory);
            
            if (session.Player != null)
                title += $" ({session.Player.Inventory.Items.Count} / {session.Player.InventoryCapacity})";

            playerMenu.Title = title;
        }

        // SelectedOptionChanged
        private void SelectedOptionChanged(PopupMenuOption<Item>? option)
        {
            if (option != null)
            {
                info.Info = option.LinkedObject.GetLocalizedInfo();
                info.Text = option.LinkedObject.MetaItem.LocalizedDescription;
            }
            else
                info.Text = null;
        }

        // UseItem
        private void UseItem(Actor actor)
        {
            if (playerMenu.SelectedOption?.LinkedObject is Item item)
            {
                if (item.MetaItem.Action == ItemAction.Consume)
                    actor.Consume(item);
                else
                    item.Use();

                SceneController.Pop();
            }
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
            if (playerMenu.SelectedOption != null)
            {
                playerContainerSelection.Position = playerMenu.SelectedOption.Position;
                playerContainerSelection.Draw(gameTime);
            }

            if (targetMenu.SelectedOption != null)
            {
                targetContainerSelection.Position = targetMenu.SelectedOption.Position;
                targetContainerSelection.Draw(gameTime);
            }
            Game.SpriteBatch.End();

            playerMenu.Draw(gameTime);
            targetMenu.Draw(gameTime);

            info.Draw(gameTime);

            if (playerMenu.SelectedOption != null)
                moveButton.Draw(gameTime);
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            if (session.Player == null)
                return HandleInputResult.Unhandled;

            if (playerMenu.HandleInput(gameTime) == HandleInputResult.Handled)
                return HandleInputResult.Handled;

            if (moveButton.TestPressed(PlayerIndex.One))
                UseItem(session.Player);

            if (HandleMouseInput())
                return HandleInputResult.Handled;

            return base.OnHandleInput(gameTime);
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            Sound.Play(SoundNames.InventoryOpen);

            playerMenu.Clear();
            MouseCursor.Instance.State = MouseCursorState.Arrow;

            if (session.Player == null || Target == null)
                return;

            // Player items
            for (int i = 0; i < session.Player.Inventory.Items.Count; i++)
            {
                playerMenu.AddOption(session.Player.Inventory.Items[i]);
            }
            playerMenu.SelectFirst();

            // Target items
            for (int i = 0; i < Target.Inventory.Items.Count; i++)
            {
                targetMenu.AddOption(Target.Inventory.Items[i]);
            }

            InvalidateTitle();

            targetMenu.Title = Target.DisplayName;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            playerMenu.Update(gameTime);
            targetMenu.Update(gameTime);

            if (playerMenu.SelectedOption != null)
                moveButton.Update(gameTime);
        }

        #endregion

        // Target
        public GameThing? Target { get; set; }
    }
}
