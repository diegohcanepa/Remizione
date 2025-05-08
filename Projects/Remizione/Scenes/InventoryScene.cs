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
        private readonly ItemMenu menu;
        private readonly UISentence sentence;

        #region Constructor

        // Constructor
        public InventoryScene(RemizioneGame game)
            : base(game, SceneSettings.None)
        {
            this.menu = new(Game);
            this.sentence = new(Game);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            Game.Shapes.DrawRectangle(Screen.Area, ColorPalette.SceneShade);
            Game.SpriteBatch.End();

            if (menu.Options.Count == 0)
                return;

            menu.Draw(gameTime);

            sentence.Draw(gameTime);
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            if (InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
            {
                SceneController.Pop();
                return HandleInputResult.Handled;
            }

            return base.OnHandleInput(gameTime);
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            menu.Clear();
            MouseCursor.Instance.State = MouseCursorState.Arrow;

            if (Container == null)
                return;

            for (int i = 0; i < Container.Items.Count; i++)
            {
                menu.AddOption(Container.Items[i]);
            }

            menu.Show(new Vector2(Screen.NativeWidth / 2, 20), $"@ItemContainerCategory.{Container.Category}");
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            menu.Update(gameTime);

            if (menu.SelectedOption != null)
                sentence.Text = menu.SelectedOption.Item.MetaItem.LocalizedDescription;
            else
                sentence.Text = null;
        }

        #endregion

        // Container
        public ItemContainer? Container { get; set; }
    }
}
