using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remizione.UI;

namespace Remizione
{
    /// <summary>
    /// InventoryScene
    /// </summary>
    public sealed class InventoryScene : Scene
    {
        private readonly ContextMenu<ItemAction> actionMenu;
        private readonly ItemMenu menu;
        private readonly UISentence sentence;

        #region Constructor

        // Constructor
        public InventoryScene(RemizioneGame game)
            : base(game, SceneSettings.None)
        {
            this.menu = new(Game);
            this.sentence = new(Game);

            this.actionMenu = new ContextMenu<ItemAction>(game, game.Camera);
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
                if (!menu.BoundingBox.Contains(InputManager.DefaultPlayer.Mouse.VirtualPosition))
                    SceneController.Pop();
                else
                    ShowActionMenu();

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

        // ShowActionMenu
        private void ShowActionMenu()
        {
            actionMenu.Clear();
            actionMenu.AddOption(ItemAction.Discard, "@ItemActions.Discard");
            actionMenu.AddOption(ItemAction.Use, "@ItemActions.Use");

            var pos = InputManager.DefaultPlayer.Mouse.VirtualPosition;
            var option = menu.GetOptionAt(pos);
            if (option != null)
            {
                actionMenu.Show(pos, false);
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            menu.Draw(gameTime);
            sentence.Draw(gameTime);

            if (actionMenu.IsVisible)
            {
                Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp);
                Game.Shapes.DrawRectangle(Screen.Area, ColorPalette.ContextMenu.SceneShade);
                Game.SpriteBatch.End();

                actionMenu.Draw(gameTime);
            }
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            if (Actor == null)
                return HandleInputResult.Unhandled;

            if (actionMenu.IsVisible)
            { 
                var result = actionMenu.HandleInput(gameTime);

                if (result == HandleInputResult.Handled && actionMenu.SelectedOption is ContextMenuOption<ItemAction> action)
                {
                    // Discard
                    if (action.Key == ItemAction.Discard)
                    {
                        if (menu.SelectedOption?.Item is Item item)
                        {
                            Actor.Inventory.Remove(item.Name);
                            menu.RemoveSelectedOption();
                            menu.Title = GetTitle();
                            Actor.Session.HUD.Log.Show("@LogVerbs.Discarded", item.MetaItem.LocalizedName);
                        }
                    }
                }

                return result;
            }

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
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (!actionMenu.IsVisible)
                menu.Update(gameTime);

            if (menu.SelectedOption != null)
                sentence.Text = menu.SelectedOption.Item.MetaItem.LocalizedDescription;
            else
                sentence.Text = null;

            actionMenu.Update(gameTime);
        }

        #endregion

        // Actor
        public Actor? Actor { get; set; }
    }
}
