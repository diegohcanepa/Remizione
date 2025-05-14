using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remizione.UI;

namespace Remizione
{
    /// <summary>
    /// CharacterSheetScene
    /// </summary>
    public sealed class CharacterSheetScene : Scene
    {
        private readonly ImageSprite container;
        private readonly UISentence sentence;
        private readonly StatData[] slots;
        private readonly TextSprite titleText;

        #region Constructor

        // Constructor
        public CharacterSheetScene(RemizioneGame game)
            : base(game, SceneSettings.None)
        {
            // Container
            this.container = new(game, Atlases.UI.CharacterSheetContainer)
            {
                PivotOrigin = RectanglePoint.Middle,
                Position = Screen.Area.GetPoint(RectanglePoint.Middle, 0, -5),
                Scale = ScaleInfo.UIElement.Medium
            };

            // Title
            this.titleText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Bottom,
                Position = container.BoundingBox.GetPoint(RectanglePoint.Top, 0, 2),
                Scale = ScaleInfo.Text.VeryLarge,
                Text = Localization.GetLocalizedValue(InGameMenuOptionName.Attributes)
            };

            this.sentence = new(Game) { ShowGradient = true };

            var x = container.BoundingBox.Center.X;
            var y = container.BoundingBox.Top + 10;

            slots = new StatData[6];
            for (var i = 0; i < slots.Length; i++)
            {
                slots[i] = new StatData(game, (Stat)i, new(x, y));
                y += slots[i].BoundingBox.Height + 1;
            }
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

            /*
            if (InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
            {
                if (!menu.BoundingBox.Contains(InputManager.DefaultPlayer.Mouse.VirtualPosition))
                    SceneController.Pop();
                return true;
            }
            */

            return false;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            container.Draw(gameTime);
            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            titleText.Draw(gameTime);
            Game.SpriteBatch.End();

            for (var i = 0; i < slots.Length; i++)
            {
                slots[i].Draw(gameTime);
            }
            
            sentence.Draw(gameTime);
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            if (Actor == null)
                return HandleInputResult.Unhandled;

            if (HandleMouseInput())
                return HandleInputResult.Handled;

            return base.OnHandleInput(gameTime);
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            Sound.Play(SoundNames.InventoryOpen);

            MouseCursor.Instance.State = MouseCursorState.Arrow;

            if (Actor == null)
                return;

            for (var i = 0; i < slots.Length; i++)
            {
                slots[i].Value = Actor.Stats.GetStatValue((Stat)i).ToString();
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
        }

        #endregion

        // Actor
        public Actor? Actor { get; set; }

        /// <summary>
        /// StatData
        /// </summary>
        private sealed class StatData : GameObject
        {
            private readonly ImageSprite container;
            private readonly TextSprite name;
            private readonly TextSprite value;

            // Constructor
            internal StatData(EngendroGame game, Stat stat, Vector2 position)
                : base(game)
            {
                // Container
                this.container = new(game, Atlases.UI.CharacterSheetStatContainer)
                {
                    Opacity = .2f,
                    PivotOrigin = RectanglePoint.Middle,
                    Position = position,
                    Scale = ScaleInfo.UIElement.Medium
                };

                // Name
                name = new TextSprite(Game, Fonts.CommonOutline)
                {
                    Color = ColorPalette.Text.Default,
                    PivotOrigin = RectanglePoint.Left,
                    Position = container.BoundingBox.GetPoint(RectanglePoint.Left, 2, 0),
                    Scale = ScaleInfo.Text.Large,
                    Text = Localization.GetLocalizedValue(stat)
                };

                // Value
                value = new TextSprite(Game, Fonts.CommonOutline)
                {
                    Color = ColorPalette.Text.Highlight,
                    PivotOrigin = RectanglePoint.Right,
                    Position = container.BoundingBox.GetPoint(RectanglePoint.Right, -2, 0),
                    Scale = ScaleInfo.Text.Large,
                };
            }

            // OnDraw
            protected override void OnDraw(GameTime gameTime)
            {
                Game.SpriteBatch.Begin(Game.Camera);
                container.Draw(gameTime);
                Game.SpriteBatch.End();

                Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
                name.Draw(gameTime);
                value.Draw(gameTime);
                Game.SpriteBatch.End();
            }

            // BoundingBox
            public RectangleF BoundingBox => container.BoundingBox;

            // Value
            public string? Value
            {
                get => value.Text;
                set => this.value.Text = value;
            }
        }
    }
}
