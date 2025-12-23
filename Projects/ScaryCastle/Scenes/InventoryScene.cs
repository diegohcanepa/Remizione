using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// InventoryScene
    /// </summary>
    public sealed class InventoryScene : Scene
    {
        #region Private fields

        private readonly TextSprite amountText;
        private const float animationSpeed = 14;
        private readonly ImageSprite bottomGradient;
        private readonly UIButton buttonAction;
        private readonly UIButton buttonClose;
        private readonly TextSprite itemNameText;
        private int selectedIndex;
        private readonly GameSession session;
        private readonly ImageSprite slotImage;
        private static readonly Vector2 slotPosition = new(Screen.Center.X, Screen.HUDArea.Bottom - 16);
        private const int spaceBetweenIcons = 15;
        private readonly StickInputController stick = new(GamePadThumbStick.Left) { AutoRepeatRate = 150 };
        private readonly TextSprite title;
        private const int visibleRange = 13;
        private float visualIndex;
        private readonly List<VisualItem> visualItems = [];

        #endregion

        #region Constructor

        // Constructor
        public InventoryScene(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            // Bottom gradient
            bottomGradient = new ImageSprite(Game, Atlases.UI.BottomGradient)
            {
                Opacity = .6f,
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.Area.GetPoint(RectanglePoint.Bottom),
                Scale = new Vector2(1, 1.2f)
            };

            // SlotImage
            this.slotImage = new(Game, Atlases.UI.ItemGridSlot)
            {
                PivotOrigin = RectanglePoint.Center,
                Position = slotPosition
            };

            // Item name
            itemNameText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Bottom,
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.Top),
                Scale = ScaleInfo.Text.VeryLarge
            };

            // Amount text
            amountText = new TextSprite(Game, Fonts.Common)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Top,
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, -3),
                Scale = ScaleInfo.Text.ExtraLarge
            };

            // Close button
            buttonClose = new UIButton(Game, InputBindings.Close)
            {
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, 0, -2),
            };

            // Title
            title = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.Bottom,
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, -25),
                Scale = ScaleInfo.Text.Huge
            };

            // Use button
            buttonAction = new UIButton(Game, InputBindings.UseFriendlyItem)
            {
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, 0, -12),
                Sound = Sound.Find(SoundNames.UISelectB)
            };
        }

        #endregion

        #region Private members

        // DrawItems
        private void DrawItems(GameTime gameTime)
        {
            for (int i = -visibleRange; i <= visibleRange; i++)
            {
                int index = (int)visualIndex + i;
                if (index < 0 || index >= visualItems.Count)
                    continue;

                // desplazamiento relativo animado
                float offset = i - (visualIndex - (int)visualIndex);
                visualItems[index].Position = slotPosition + new Vector2(offset * spaceBetweenIcons, 0);

                // Escala y opacidad basadas en distancia
                float distance = MathF.Abs(offset);
                float scale = MathF.Max(.6f, .8f - (distance * .2f)); // escala mínima 0.6
                float alpha = MathF.Max(.3f, 1 - (distance * .3f)); // transparencia mínima 0.3

                visualItems[index].Opacity = alpha;
                visualItems[index].Scale = new(scale);

                visualItems[index].Draw(gameTime);
            }
        }

        // GetInventoryItemAt
        private VisualItem? GetInventoryItemAt(Vector2 position)
        {
            for (int i = 0; i < visualItems.Count; i++)
            {
                if (visualItems[i].BoundingBox.Contains(position))
                    return visualItems[i];
            }

            return null;
        }

        // HandleMouseInput
        private bool HandleMouseInput()
        {
            if (InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
            {
                SceneController.Pop();
                return true;
            }

            if (!InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
                return false;

            if (GetInventoryItemAt(InputManager.DefaultPlayer.Mouse.VirtualPosition) is VisualItem item)
            {
                Select(visualItems.IndexOf(item));
                MouseCursor.Instance.AnimateClick();
                Sound.Play(SoundNames.UIHover);
            }

            return false;
        }

        // InvalidateSlot
        private void InvalidateSlot()
        {
            amountText.Text = selectedIndex < 0 ? null : SelectedItem?.Item.GetDisplayAmount();
            buttonAction.Text = null;

            if (SelectedItem?.Item is Item item)
            {
                if (item.MetaItem.IsEquipment)
                {
                    if (item.MetaItem.Category == ItemCategory.Gadget)
                    {
                        buttonAction.Text = Localization.GetValue(item.IsEquipped ? InventoryVerb.TakeOff : InventoryVerb.Equip);
                    }
                    else
                    {
                        buttonAction.Text = Localization.GetValue(InventoryVerb.Equip);
                    }
                }
                else if (SelectedItem.Item.MetaItem.Category == ItemCategory.Consumable)
                {
                    buttonAction.Text = Localization.GetValue(InventoryVerb.Use);
                }
            }
        }

        // PerformAction
        private bool PerformAction()
        {
            if (SelectedItem?.Item is not Item item)
                return false;

            // Action
            if (buttonAction.Text != null && buttonAction.TestPressed(PlayerIndex.One))
            {
                if (item.MetaItem.IsEquipment)
                {
                    if (item.IsEquipped)
                        session.Inventory.Unequip(item);
                    else
                        session.Inventory.Equip(item);
                }
                else if (item.MetaItem.IsConsumable)
                {
                    session.Player?.ConsumeItem(item);
                    if (item.Count <= 0)
                    {
                        visualItems.Remove(SelectedItem);
                        if (selectedIndex == 0)
                            Select(visualItems.NextIndex(selectedIndex));
                        else
                            Select(visualItems.PreviousIndex(selectedIndex));
                    }

                    InvalidateSlot();
                }

                return true;
            }

            return false;
        }

        // Select
        private void Select(int index)
        {
            if (index >= visualItems.Count)
                return;

            selectedIndex = index;
            var item = visualItems[index].Item;
            itemNameText.Text = index < 0 ? null : item.DisplayText;
            InvalidateSlot();
        }

        // SelectedItem
        private VisualItem? SelectedItem => selectedIndex == -1 ? null : visualItems[selectedIndex];

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!IsCurrentScene || session.IsOutcomeInProgress)
                return;

            // Gradient
            //Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            //bottomGradient.Draw(gameTime);
            //Game.SpriteBatch.End();

            session.Inventory.Session.HUD.Draw(gameTime);

            Game.SpriteBatch.Begin(Game.Camera);
            title.Draw(gameTime);
            itemNameText.Draw(gameTime);
            slotImage.Draw(gameTime);
            amountText.Draw(gameTime);
            DrawItems(gameTime);
            Game.SpriteBatch.End();

            buttonClose.Draw(gameTime);

            if (buttonAction.Text != null)
                buttonAction.Draw(gameTime);
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            // Mouse input
            if (InputManager.DefaultPlayer.LastInputMethod == InputMethod.Mouse)
            {
                if (HandleMouseInput())
                    return HandleInputResult.Handled;
            }

            // Action
            if (PerformAction())
                return HandleInputResult.Handled;

            // Close
            if (buttonClose.TestPressed(PlayerIndex.One))
            {
                SceneController.Pop();
                return HandleInputResult.Handled;
            }

            if (visualItems.Count > 1)
            {
                // Move left
                if (selectedIndex > 0)
                {
                    if (InputBindings.SelectLeft.IsPressed(PlayerIndex.One) || stick.IsLeft(PlayerIndex.One))
                    {
                        Select(Math.Max(0, selectedIndex - 1));
                        Sound.Play(SoundNames.UIHover);
                        return HandleInputResult.Handled;
                    }
                }

                // Move right
                if (selectedIndex < visualItems.Count - 1)
                {
                    if (InputBindings.SelectRight.IsPressed(PlayerIndex.One) || stick.IsRight(PlayerIndex.One))
                    {
                        Select(Math.Min(visualItems.Count - 1, selectedIndex + 1));
                        Sound.Play(SoundNames.UIHover);
                        return HandleInputResult.Handled;
                    }
                }

                // Move up (first item)
                if (InputBindings.SelectUp.IsPressed(PlayerIndex.One) || stick.IsUp(PlayerIndex.One))
                {
                    Select(0);
                    Sound.Play(SoundNames.UIHover);
                    return HandleInputResult.Handled;
                }

                // Move down (last item)
                if (InputBindings.SelectDown.IsPressed(PlayerIndex.One) || stick.IsDown(PlayerIndex.One))
                {
                    Select(visualItems.Count - 1);
                    Sound.Play(SoundNames.UIHover);
                    return HandleInputResult.Handled;
                }
            }

            return HandleInputResult.Handled;
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            Sound.Play(SoundNames.UIInventoryOpen);

            base.OnLoadContent();

            visualItems.Clear();

            for (var i = 0; i < session.Inventory.Count; i++)
            {
                visualItems.Add(new(session.Game, session.Inventory[i]));
            }

            Select(0);

            visualIndex = selectedIndex;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            bottomGradient.Update(gameTime);
            stick.Update(gameTime);

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            visualIndex += (selectedIndex - visualIndex) * MathF.Min(1f, animationSpeed * deltaTime);

            base.OnUpdate(gameTime);

            buttonClose.Update(gameTime);
            buttonAction.Update(gameTime);
        }

        #endregion

        // Text
        public string? Text
        {
            get => title.Text;
            set => title.Text = value;
        }
    }
}
