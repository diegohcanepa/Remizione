using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ScaryCastle.Menus
{
    /// <summary>
    /// Menu
    /// </summary>
    public class Menu : UIComponent, IInputHandler
    {
        #region Private fields

        private bool layout;
        private readonly List<MenuItem> items = [];
        private Vector2 position;
        private readonly StickInputController stick = new(GamePadThumbStick.Left) { AutoRepeatRate = 250 };

        #endregion

        #region Constructor

        // Constructor
        public Menu(ScaryCastleGame game)
            : base(game)
        {
            this.Game = game;
            this.Items = new ReadOnlyCollection<MenuItem>(items);
        }

        #endregion

        #region Private members

        // MoveSelection
        private void MoveSelection(int direction)
        {
            if (items.Count < 2)
            {
                return;
            }

            var index = -1;

            if (SelectedItem == null)
            {
                if (items.Count > 0)
                {
                    index = 0;
                }
            }

            else if (direction < 0)
            {
                index = items.IndexOf(SelectedItem);
                if (index == 0)
                {
                    index = items.Count - 1;
                }
                else
                {
                    index--;
                }
            }
            else
            {
                index = items.IndexOf(SelectedItem);
                if (index == items.Count - 1)
                {
                    index = 0;
                }
                else
                {
                    index++;
                }
            }

            SelectedItem = items[index];
            Invalidate();
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            for (var i = 0; i < items.Count; i++)
            {
                items[i].Draw(gameTime);
            }
        }

        // OnInvalidate
        protected override void OnInvalidate()
        {
            base.OnInvalidate();

            var pos = Position;
            for (var i = 0; i < items.Count; i++)
            {
                if (layout)
                {
                    items[i].Position = pos;
                }

                items[i].Invalidate();

                if (layout)
                {
                    pos.Y += items[0].BoundingBox.Height + 1;
                }
            }

            layout = false;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            //?
            /*
            for (var i = 0; i < items.Count; i++)
            {
                items[i].Update(gameTime);

                // Mouse
                if (InputManager.Mouse.Allowed && InputManager.DeviceInfo[(int)InputHelper.PlayerIndex] != InputDevice.Gamepad)
                {
                    var pos = Game.ViewportAdapter.ToVirtual(InputManager.Mouse.Position);

                    if (items[i].BoundingBox.Contains(pos))
                    {
                        if (SelectedItem != items[i] && InputManager.Mouse.HoveredObject != items[i])
                        {
                            SelectedItem = items[i];
                            InputManager.Mouse.HoveredObject = items[i];
                        }
                        else if (SelectedItem != null && InputManager.Mouse.IsLeftButtonClicked())
                        {
                            if (SelectedItem == InputManager.Mouse.HoveredObject)
                            {
                                SelectedItem.Press();
                            }
                            else
                            {
                                InputManager.Mouse.HoveredObject = null;
                            }
                        }
                    }
                }
            }

            stick.Stick = InputHelper.PreferredStick;
            stick.Update(gameTime);
            */
        }

        #endregion

        // AddItem
        public MenuItem AddItem(MenuItemName name, Action? onPress, AtlasImage? iconImage)
        {
            MenuItem result = new(this, name, iconImage, onPress);

            items.Add(result);
            layout = true;
            Invalidate();
            if (SelectedItem == null)
                SelectedItem = result;

            return result;
        }

        // BoundingBox
        public RectangleF BoundingBox
        {
            get
            {
                var result = RectangleF.Empty;
                for (var i = 0; i < items.Count; i++)
                {
                    result = RectangleF.Union(result, items[i].BoundingBox);
                }

                return result;
            }
        }

        // Game
        public new ScaryCastleGame Game { get; }

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            if (SelectedItem == null)
            {
                return HandleInputResult.Unhandled;
            }

            var playerIndex = PlayerIndex.One;

            if (InputBindings.SelectUp.IsPressed(playerIndex) || stick.IsUp(playerIndex))
            {
                PreviousItem();
                return HandleInputResult.Handled;
            }

            else if (InputBindings.SelectDown.IsPressed(playerIndex) || stick.IsDown(playerIndex))
            {
                NextItem();
                return HandleInputResult.Handled;
            }

            else if (InputBindings.Select.IsPressed(playerIndex))
            {
                SelectedItem.Press();
                return HandleInputResult.Handled;
            }

            else
            {
                return HandleInputResult.Unhandled;
            }
        }

        // Items
        public ReadOnlyCollection<MenuItem> Items { get; }

        // Position
        public Vector2 Position
        {
            get => position;
            set
            {
                if (value != position)
                {
                    position = value;
                    layout = true;
                    Invalidate();
                }
            }
        }

        // SelectedItem
        public MenuItem? SelectedItem
        {
            get;
            set
            {
                if (value != field)
                {
                    if (field != null)
                    {
                        //SoundManager.Play(SoundNames.MenuItem.ToString());
                    }

                    field = value;
                    Invalidate();
                }
            }
        }

        // NextItem
        public void NextItem()
        {
            MoveSelection(1);
        }

        // PreviousItem
        public void PreviousItem()
        {
            MoveSelection(-1);
        }
    }
}
