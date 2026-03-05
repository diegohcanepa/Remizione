using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ScaryCastle.Menus
{
    /// <summary>
    /// OptionMenu
    /// </summary>
    public sealed class OptionMenu : UIComponent, IInputHandler
    {
        #region Private fields

        private readonly Sprite highlightSprite;
        private readonly List<IOption> options = [];
        private Vector2 position;
        private readonly StickInputController stick;

        #endregion

        #region Constructor

        // Constructor
        public OptionMenu(ScaryCastleGame game, int verticalSpacing)
            : base(game)
        {
            this.VerticalSpacing = verticalSpacing;

            // Option highlight
            highlightSprite = new Sprite(Game, Atlases.Menu.MenuItemHighlight)
            {
                Opacity = .5f,
                PivotOrigin = RectanglePoint.Center,
                X = Screen.Area.Center.X,
                Scale = new Vector2(.8f)
            };
            highlightSprite.Tweens.OpacityTween = FloatTween.Create(TweenStyle.QuadraticInOut, .8f, 1, 800, -1);
            VerticalSpacing = verticalSpacing;

            stick = new StickInputController(GamePadThumbStick.Left) { AutoRepeatRate = 250 };
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp);
            highlightSprite.Draw(gameTime);
            for (var i = 0; i < options.Count; i++)
            {
                options[i].Draw(gameTime);
            }
            Game.SpriteBatch.End();
        }

        // OnInvalidate
        protected override void OnInvalidate()
        {
            var pos = Position;

            foreach (var option in options)
            {
                option.Position = pos;
                pos.Y += 12 + VerticalSpacing;
            }

            if (SelectedOption != null)
            {
                highlightSprite.Y = SelectedOption.LabelBoundingBox.Center.Y - .5f;
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            //?
            /*
            stick.Stick = InputHelper.PreferredStick;
            stick.Update(gameTime);

            highlightSprite.Update(gameTime);

            for (var i = 0; i < options.Count; i++)
            {
                options[i].Update(gameTime);

                // Mouse
                if (InputManager.Mouse.Allowed && InputManager.DeviceInfo[(int)InputHelper.PlayerIndex] != InputDevice.Gamepad)
                {
                    var pos = Game.ViewportAdapter.ToVirtual(InputManager.Mouse.Position);

                    if (options[i].LabelBoundingBox.Contains(pos))
                    {
                        if (SelectedIndex != i && InputManager.Mouse.HoveredObject != options[i])
                        {
                            SelectedIndex = i;
                            InputManager.Mouse.HoveredObject = options[i];
                        }
                        else if (SelectedIndex != -1 && InputManager.Mouse.IsLeftButtonClicked())
                        {
                            if (options[SelectedIndex] == InputManager.Mouse.HoveredObject)
                            {
                                if (options[i].LeftArrowBoundingBox.Contains(pos))
                                {
                                    options[i].PreviousValue();
                                }
                                else if (options[i].RightArrowBoundingBox.Contains(pos))
                                {
                                    options[i].NextValue();
                                }
                                else
                                {
                                    SelectedOption?.PerformClick();
                                }
                            }
                            else
                            {
                                InputManager.Mouse.HoveredObject = null;
                            }
                        }
                    }
                }
            }
            */
        }

        #endregion

        // AddOption
        public void AddOption(IOption item)
        {
            options.Add(item);

            if (SelectedIndex == -1)
            {
                SelectedIndex = 0;
            }

            Invalidate();
        }

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            if (SelectedOption == null)
            {
                return HandleInputResult.Unhandled;
            }

            var playerIndex = PlayerIndex.One;

            if (InputBindings.SelectLeft.IsPressed(playerIndex) || stick.IsUp(playerIndex))
            {
                PreviousOption();
                return HandleInputResult.Handled;
            }

            else if (InputBindings.SelectRight.IsPressed(playerIndex) || stick.IsDown(playerIndex))
            {
                NextOption();
                return HandleInputResult.Handled;
            }

            else if (InputBindings.SelectRight.IsPressed(playerIndex) || stick.IsRight(playerIndex))
            {
                SelectedOption.NextValue();
                return HandleInputResult.Handled;
            }

            else if (InputBindings.SelectLeft.IsPressed(playerIndex) || stick.IsLeft(playerIndex))
            {
                SelectedOption.PreviousValue();
                return HandleInputResult.Handled;
            }

            else if (InputBindings.ClickMenuItem.IsPressed(playerIndex))
            {
                SelectedOption.PerformClick();
                return HandleInputResult.Handled;
            }

            else
            {
                return HandleInputResult.Unhandled;
            }
        }

        // NextOption
        public bool NextOption()
        {
            if (options.Count > 0)
            {
                var index = SelectedIndex + 1;
                if (index == options.Count)
                {
                    index = 0;
                }

                SelectedIndex = index;

                //SoundManager.Play(SoundNames.MenuItem.ToString());

                return true;
            }

            return false;
        }

        // Position
        public Vector2 Position
        {
            get => position;
            set
            {
                if (value != position)
                {
                    position = value;
                    Invalidate();
                }
            }
        }

        // PreviousOption
        public bool PreviousOption()
        {
            if (options.Count > 0)
            {
                var index = SelectedIndex - 1;
                if (index == -1)
                {
                    index = options.Count - 1;
                }

                SelectedIndex = index;

                //SoundManager.Play(SoundNames.MenuItem.ToString());

                return true;
            }

            return false;
        }

        // SelectedIndex
        public int SelectedIndex
        {
            get;
            set
            {
                if (value != field)
                {
                    if (field != -1)
                    {
                        //SoundManager.Play(SoundNames.MenuItem.ToString());
                    }

                    field = value;
                    Invalidate();
                }
            }
        } = -1;

        // SelectedOption
        public IOption? SelectedOption => SelectedIndex == -1 ? null : options[SelectedIndex];

        // VerticalSpacing
        public int VerticalSpacing { get; }
    }
}
