using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Remizione.UI;

namespace Remizione
{
    /// <summary>
    /// CharacterSheetScene
    /// </summary>
    public sealed class CharacterSheetScene : Scene
    {
        private readonly PopupMenu<StatData> menu;
        private readonly UISentence sentence;

        #region Constructor

        // Constructor
        public CharacterSheetScene(RemizioneGame game)
            : base(game, SceneSettings.None)
        {
            this.menu = new(Game, HorizontalAlignment.Center, false, RectangleF.Empty);
            this.sentence = new(Game) { ShowGradient = true };
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
                return true;
            }

            return false;
        }

        // SelectedOptionChanged
        private void SelectedOptionChanged(PopupMenuOption<StatData>? option)
        {
            if (option != null)
                sentence.Text = option.LinkedObject.LocalizedDescription;
            else
                sentence.Text = null;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            menu.Draw(gameTime);
            sentence.Draw(gameTime);
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            if (Actor == null)
                return HandleInputResult.Unhandled;

            if (menu.HandleInput(gameTime) == HandleInputResult.Handled)
                return HandleInputResult.Handled;

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

            for (int i = 0; i < 6; i++)
            {
                menu.AddOption(new StatData((Stat)i, Actor.Stats.GetStatValue((Stat)i)));
            }

            menu.SelectFirst();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            menu.Update(gameTime);
        }

        #endregion

        // Actor
        public Actor? Actor { get; set; }

        /// <summary>
        /// StatData
        /// </summary>
        private sealed class StatData
        {
            // Constructor
            internal StatData(Stat stat, int value)
            {
                this.Stat = stat;
                this.LocalizedName = TextRepository.GetValue($"Stat.{stat}.Name") + ": " + value.ToString();
                this.LocalizedDescription = TextRepository.GetValue($"Stat.{stat}.Description");
            }

            // LocalizedName
            public string LocalizedName { get; }

            // LocalizedDescription
            public string LocalizedDescription { get; }

            // Stat
            internal Stat Stat { get; }

            // ToString
            public override string ToString() => LocalizedName;
        }
    }
}
