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
        private readonly ImageSprite container;
        private readonly ImageSprite containerSelection;
        private readonly PopupMenu<StatData> menu;
        private readonly UISentence sentence;

        #region Constructor

        // Constructor
        public CharacterSheetScene(RemizioneGame game)
            : base(game, SceneSettings.None)
        {
            // Container
            this.container = new(game, Atlases.UI.CharacterSheetContainer)
            {
                PivotOrigin = RectanglePoint.Top,
                Position = new Vector2(Screen.Center.X, 14),
                Scale = ScaleInfo.UIElement.Medium
            };

            // ContainerSelection
            this.containerSelection = new(game, Atlases.UI.CharacterSheetStatContainer)
            {
                Opacity = .2f,
                PivotOrigin = RectanglePoint.Middle,
                Scale = ScaleInfo.UIElement.Medium
            };

            // Menu
            this.menu = new(Game, HorizontalAlignment.Center, false, container.BoundingBox)
            {
                OnSelectionChanged = SelectedOptionChanged,
                Position = container.BoundingBox.GetPoint(RectanglePoint.Top, 0, 12),
                Title = Localization.GetLocalizedValue(InGameMenuOptionName.Attributes),
                TitlePosition = container.BoundingBox.GetPoint(RectanglePoint.Top, 0, 2)
            };

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
            Game.SpriteBatch.Begin(Game.Camera);
            container.Draw(gameTime);

            if (menu.SelectedOption != null)
            {
                containerSelection.Position = menu.SelectedOption.Position;
                containerSelection.Draw(gameTime);
            }

            Game.SpriteBatch.End();

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
            
            menu.AddOption(new StatData(Actor.Level));
            menu.AddOption(new StatData(DerivedStat.Spirit, Actor.HP, Actor.MaxHP));
            menu.AddOption(new StatData(DerivedStat.Faith, Actor.Faith, Actor.MaxFaith));

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
            internal StatData(int level)
            {
                this.LocalizedName = TextRepository.GetValue($"ActorProperty.Level.Name") + ": " + level.ToString();
                this.LocalizedDescription = TextRepository.GetValue($"ActorProperty.Level.Description");
            }

            // Constructor
            internal StatData(Stat stat, int value)
            {
                this.LocalizedName = TextRepository.GetValue($"Stat.{stat}.Name") + ": " + value.ToString();
                this.LocalizedDescription = TextRepository.GetValue($"Stat.{stat}.Description");
            }

            // Constructor
            internal StatData(DerivedStat derivedStat, int value, int max)
            {
                this.LocalizedName = TextRepository.GetValue($"DerivedStat.{derivedStat}.Name") + ": " + $"{value} / {max}";
                this.LocalizedDescription = TextRepository.GetValue($"DerivedStat.{derivedStat}.Description");
            }

            // LocalizedName
            public string LocalizedName { get; }

            // LocalizedDescription
            public string LocalizedDescription { get; }

            // ToString
            public override string ToString() => LocalizedName;
        }
    }
}
