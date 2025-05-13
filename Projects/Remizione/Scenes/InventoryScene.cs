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
        private readonly UIControl discardButton;
        private readonly ItemMenu menu;
        private readonly UISentence sentence;

        #region Constructor

        // Constructor
        public InventoryScene(RemizioneGame game)
            : base(game, SceneSettings.None)
        {
            this.menu = new(Game, SelectedOptionChanged);
            this.sentence = new(Game) { ShowGradient = true };

            // Default action
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
            if (menu.SelectedOption?.Item is Item item)
            {
                actor.Inventory.Remove(item.Name);
                menu.RemoveSelectedOption();
                menu.Title = GetTitle();
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

        // GetTitle
        private string GetTitle()
        {
            var title = Localization.GetLocalizedValue(ItemContainerCategory.Inventory);
            if (Actor != null)
                title += $" ({Actor.Inventory.Items.Count} / {Actor.InventoryCapacity})";

            return title;
        }

        // SelectedOptionChanged
        private void SelectedOptionChanged(ItemMenuOption? option)
        {
            if (option != null)
            {
                sentence.Info = option.Item.GetLocalizedInfo();
                sentence.Text = option.Item.MetaItem.LocalizedDescription;
                actionButton.Text = Localization.EncodeKey(option.Item.MetaItem.Action);
            }
            else
                sentence.Text = null;
        }

        // UseItem
        private void UseItem(Actor actor)
        {
            if (menu.SelectedOption?.Item is Item item)
            {
                if (item.MetaItem.Category == ItemCategory.Consumable)
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
            menu.Draw(gameTime);
            sentence.Draw(gameTime);

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
                UseItem(Actor);

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

            menu.Show(new Vector2(Screen.NativeWidth / 2, 12), GetTitle());
            menu.SelectFirst();

            discardButton.Position = menu.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 5, -10);
            actionButton.Position = menu.BoundingBox.GetPoint(RectanglePoint.RightBottom, -5, -10);
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
