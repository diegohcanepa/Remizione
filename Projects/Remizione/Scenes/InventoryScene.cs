using Engendro;
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
        private readonly UIControl discardButton;
        private readonly ItemMenu menu;
        private readonly UISentence sentence;
        private readonly UIControl useButton;

        #region Constructor

        // Constructor
        public InventoryScene(RemizioneGame game)
            : base(game, SceneSettings.None)
        {
            this.menu = new(Game, SelectedOptionChanged);
            this.sentence = new(Game) { ShowGradient = true };

            this.discardButton = new UIControl(game, InputBindings.Select)
            {
                PivotOrigin = RectanglePoint.LeftBottom,
                Text = Localization.EncodeKey(ItemAction.Discard)
            };

            this.useButton = new UIControl(game, InputBindings.Select)
            {
                PivotOrigin = RectanglePoint.RightBottom,
                Text = Localization.EncodeKey(ItemAction.Use)
            };
        }

        #endregion

        #region Private members

        // DiscardItem
        private void DiscardItem(Actor actor)
        {
            if (menu.SelectedOption?.Item is Item item)
            {
                actor.Inventory.Remove(item.Name);
                menu.RemoveSelectedOption();
                menu.Title = GetTitle();
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

        // GetTitle
        private string GetTitle()
        {
            var title = TextRepository.GetValue("ItemContainerCategory.Inventory");
            if (Actor != null)
                title += $" ({Actor.Inventory.Items.Count} / {Actor.InventoryCapacity})";

            return title;
        }

        // SelectedOptionChanged
        private void SelectedOptionChanged() => sentence.Text = menu.SelectedOption?.Item.MetaItem.LocalizedDescription;

        // UseItem
        private void UseItem(Actor actor)
        {
            if (menu.SelectedOption?.Item is Item item)
            {
                item.Consume();
                if (item.Count == 0)
                {
                    actor.Inventory.Remove(item.Name);
                    menu.RemoveSelectedOption();
                    menu.Title = GetTitle();
                }
                else
                    menu.SelectedOption.Invalidate();
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            menu.Draw(gameTime);
            sentence.Draw(gameTime);

            if (menu.SelectedOption != null)
            {
                discardButton.Draw(gameTime);
                useButton.Draw(gameTime);
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

            if (useButton.TestPressed(PlayerIndex.One))
                UseItem(Actor);

            if (HandleMouseInput())
                return HandleInputResult.Handled;

            return base.OnHandleInput(gameTime);
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            menu.Clear();
            MouseCursor.Instance.State = MouseCursorState.Arrow;

            if (Actor == null)
                return;

            for (int i = 0; i < Actor.Inventory.Items.Count; i++)
            {
                menu.AddOption(Actor.Inventory.Items[i]);
            }

            menu.Show(new Vector2(Screen.NativeWidth / 2, 20), GetTitle());

            discardButton.Position = menu.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 5, -9);
            useButton.Position = menu.BoundingBox.GetPoint(RectanglePoint.RightBottom, -5, -9);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            menu.Update(gameTime);

            if (menu.SelectedOption != null)
            {
                discardButton.Update(gameTime);
                useButton.Update(gameTime);
            }
        }

        #endregion

        // Actor
        public Actor? Actor { get; set; }
    }
}
