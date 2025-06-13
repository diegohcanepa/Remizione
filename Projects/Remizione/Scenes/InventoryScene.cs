using Engendro;
using Engendro.Input;
using EngendroAdventure;
using EngendroAdventure.Scripting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// InventoryScene
    /// </summary>
    public sealed class InventoryScene : Scene
    {
        #region Constants

        private const int slotHeight = 22;
        private const int slotWidth = 18;
        private const int tweenDuration = 300;

        #endregion

        #region Private fields

        private enum OptionKind { None, Verb, UseWith, Combine, Move }
        private readonly ImageSprite bottomGradient;
        private readonly UIControl buttonClose;
        private readonly UIControl buttonSelect;
        //private readonly UIContextMenu contextMenu;
        private Item? combineItem;
        private Vector2 combineItemOriginalPosition;
        private readonly FloatTween fadeTween = new();
        private readonly TextSprite itemDescription;
        private float lastKnownAmount;
        private bool moved;
        private readonly OptionKind[] options = new OptionKind[4];
        private readonly ImageSprite prohibitionMark;
        private readonly ImageSprite[] slots = new ImageSprite[20];
        private readonly StickInputController stick = new(GamePadThumbStick.Left) { AutoRepeatRate = 200 };
        private readonly List<UIItem> visualItems = new() { Capacity = 20 };

        #endregion

        #region Constructor

        // Constructor
        public InventoryScene(Actor owner)
            : base(owner.Game)
        {
            this.Owner = owner;

            for (var i = 0; i < slots.Length; i++)
            {
                slots[i] = new(owner.Game, Atlases.UI.ItemSlot)
                {
                    PivotOrigin = RectanglePoint.Middle,
                    Scale = ScaleInfo.UIElement.Small
                };
            }

            for (var i = 0; i < visualItems.Capacity; i++)
            {
                visualItems.Add(new UIItem(owner.Game));
            }

            // Context menu
            //contextMenu = new UIContextMenu(owner.Game);

            // Close button
            buttonClose = new UIControl(owner.Game, InputBindings.Close)
            {
                AllowContainer = true,
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom),
                Small = true,
            };

            // Select button
            buttonSelect = new UIControl(Game, InputBindings.Select)
            {
                AllowContainer = true,
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, 0, -12),
                Small = true,
            };

            // Bottom gradient
            bottomGradient = new ImageSprite(owner.Game, Atlases.UI.BottomGradient)
            {
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.Area.GetPoint(RectanglePoint.Bottom),
                Scale = new Vector2(1, 1.2f)
            };

            // Item description
            itemDescription = new TextSprite(owner.Session.Game, Fonts.Common)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.Area.GetPoint(RectanglePoint.Bottom, 0, -13),
                Scale = ScaleInfo.Text.Medium
            };

            // Prohibition mark
            prohibitionMark = new ImageSprite(Game, Atlases.UI.ProhibitionMark)
            {
                Color = Color.White * .7f,
                PivotOrigin = RectanglePoint.Middle
            };
        }

        #endregion

        #region Private members

        // DoMoveOption
        private void DoMoveOption()
        {
            /*
            moved = true;

            if (Owner.SelectedItem == null)
                return;

            if (Owner.Children.Count == 1)
            {
                Owner.SelectedItem.Shake();
                return;
            }

            if (Owner.SelectedItemIndex == 0)
            {
                Owner.Children.Swap(Owner.SelectedItemIndex, Owner.Children.Count - 1);
                Owner.SelectedItemIndex = Owner.Children.Count - 1;
                LayoutItems();
                Owner.SelectedItem.ChangeVisualState(ItemVisualState.Active);
            }
            else
            {
                Owner.SelectedItem?.Slide(-slotStep, 0, tweenDuration, Invalidate);
                if (Owner.Children[Owner.SelectedItemIndex - 1] is Item previousItem)
                    previousItem.Slide(slotStep, 0, tweenDuration);

                if (Owner.SelectedItem != null)
                {
                    Owner.Children.SwapPrevious(Owner.SelectedItem);
                    Owner.SelectedItemIndex--;
                }
            }

            Owner.Session.HUD.TopMessage.Hide();

            InputManager.Suspend(tweenDuration);
            */
        }

        // DrawItems
        private void DrawItems(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);

            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].Draw(gameTime);

                /*
                if (visualItems[i] is UIItem item)
                {
                    if (item == combineItem)
                    {
                        emptySlot.Position = itemPositions[i];
                        emptySlot.Draw(gameTime);
                    }

                    item.Draw(gameTime);

                    if (combineItem != null && !item.AllowCombine && Owner.SelectedItem == item)
                    {
                        prohibitionMark.Position = item.Position;
                        prohibitionMark.Draw(gameTime);
                    }
                }
                */
            }

            Game.SpriteBatch.End();
        }

        // DrawTexts
        private void DrawTexts(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            itemDescription.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // InvalidateContextMenu
        private void InvalidateContextMenu()
        {
            /*
            contextMenu.Clear();

            Array.Fill(options, OptionKind.None);

            if (Owner.SelectedItem is not Item item)
                return;

            int index = 0;

            // Verb
            if (combineItem == null)
            {
                if (item.Verb != Verb.None)
                {
                    contextMenu.AddOption(OptionKind.Verb.ToString(), Localization.GetValue(item.Verb), Atlases.UI.UseIcon);
                    options[index] = OptionKind.Verb;
                    index++;
                }

                // Use with
                if (item.AllowUseWith)
                {
                    if (Owner.Target == null || Owner.Target.InteractionMode == OperationMode.Manual)
                    {
                        var text = VladUtils.EncodeUseWithKey(Owner.Target);
                        var menuOption = contextMenu.AddOption(OptionKind.UseWith.ToString(), text, Atlases.UI.UseWithIcon);
                        menuOption.IsDisabled = Owner.Target == null;
                        options[index] = OptionKind.UseWith;
                        index++;
                    }
                }
            }

            // Combine
            if (combineItem != item)
            {
                if (item.AllowCombine || combineItem != null)
                {
                    var menuOption = contextMenu.AddOption(OptionKind.Combine.ToString(), VladUtils.EncodeVerbKey(Verb.Combine), Atlases.UI.CombineIcon);
                    menuOption.IsDisabled = Owner.Children.Count <= 1 || !item.AllowCombine;
                    options[index] = OptionKind.Combine;
                }
            }

            // Move
            if (combineItem == null && Owner.Children.Count > 1)
            {
                contextMenu.AddOption(OptionKind.Move.ToString(), Localization.GetValue(Verb.Move), Atlases.UI.MoveIcon);
                options[index] = OptionKind.Move;
            }

            Vector2 pos;

            InvalidateContextMenuTitle();

            pos.X = itemPositions[Owner.SelectedItemIndex].X - 12;
            pos.Y = 212 - contextMenu.Height;

            if (combineItem != null)
                pos.Y -= slotHeight * ScaleInfo.ItemInventoryInactiveState.Y + 5;

            contextMenu.Position = pos;

            if (moved)
            {
                moved = false;
                contextMenu.Last();
            }
            
            */
        }

        // InvalidateContextMenuTitle
        private void InvalidateContextMenuTitle()
        {
            /*
            if (Owner.SelectedItem is Item item)
            {
                var title = item.GetLocalizedDisplayName();

                if (combineItem == null && item is DegradableItem degradableItem)
                {
                    var displayAmount = degradableItem.GetDisplayAmount();
                    if (!string.IsNullOrWhiteSpace(displayAmount))
                        title += " (" + displayAmount + ")";
                }

                contextMenu.Title = title;
            }
            */
        }

        // InvalidateItemStats
        private void InvalidateItemStats()
        {
            /*
            var item = Owner.SelectedItem;

            if (item == null || string.IsNullOrWhiteSpace(item.DisplayName))
                itemDescription.Text = Localization.GetValue(MessageKey.InventoryEmpty);
            else
            {
                itemDescription.Text = item.GetDescription();
                lastKnownAmount = item is DegradableItem degradableItem ? degradableItem.Amount : -1;
                itemShortcutButton.InputBinding = item.Shortcut;
                itemShortcutButton.Position = itemDescription.BoundingBox.GetPoint(RectanglePoint.Left, -5, 0);
            }
            */
        }

        // LayoutItems
        private void LayoutItems()
        {
            for (var i = 0; i < visualItems.Count; i++)
            {
                if (i == slots.Length)
                    break;

                visualItems[i].Position = slots[i].Position;
            }

            Invalidate();
        }

        // LayoutSlots
        private void LayoutSlots()
        {
            const int spacing = 2;
            var maximum = Owner.Inventory.Size;
            var imageWidth = slots[0].BoundingBox.Width;
            var totalWidth = (maximum * imageWidth) + ((maximum - 1) * spacing);
            var start = (Screen.NativeWidth - totalWidth) / 2;

            for (int i = 0; i < Owner.Inventory.Size; i++)
            {
                var x = start + (i * (imageWidth + spacing));
                slots[i].Position = new(x, 100);
            }
        }

        // MoveSelection
        private bool MoveSelection(int direction)
        {
            /*
            if (direction == 0 || Owner.Children.Count <= 1)
                return false;

            if (Owner.SelectedItem is Item item && item != combineItem)
                item.ChangeVisualState(ItemVisualState.Inactive, false);

            var cycled = false;

            if (direction > 0)
            {
                if (Owner.SelectedItemIndex == Owner.Children.Count - 1)
                {
                    Owner.SelectedItemIndex = 0;
                    cycled = true;
                }
                else
                    Owner.SelectedItemIndex++;
            }
            else
            {
                if (Owner.SelectedItemIndex == 0)
                {
                    Owner.SelectedItemIndex = Owner.Children.Count - 1;
                    cycled = true;
                }
                else
                    Owner.SelectedItemIndex--;
            }

            if (Owner.SelectedItem is Item selectedItem)
            {
                if (selectedItem != combineItem)
                    selectedItem.ChangeVisualState(ItemVisualState.Active, false);

                if (combineItem != null)
                {
                    if (cycled)
                    {
                        combineItem.Position = selectedItem == combineItem ? combineItemOriginalPosition : Owner.SelectedItem.Position;
                        combineItem.PositionY -= slotHeight;
                    }
                    else
                        combineItem.Slide(direction > 0 ? slotStep : -slotStep, 0, 1);

                    InputManager.Suspend(50);
                }
            }

            Owner.Session.HUD.TopMessage.Hide();

            SoundManager.Play(SoundNames.InventoryNavigation.ToString());
            */

            Invalidate();

            return true;
        }

        // PerformCombineOutcome
        private void PerformCombineOutcome()
        {
            /*
            var item = Owner.SelectedItem;

            if (item == null || combineItem == null)
                return;

            // Item1+Item2
            var script = Owner.Session.ScriptLibrary.GetCompoundOutcome(combineItem.StaticName, item.StaticName);

            // Item2+Item1
            if (script == null)
                script = Owner.Session.ScriptLibrary.GetCompoundOutcome(item.StaticName, combineItem.StaticName);

            // CombineItem+?
            if (script == null)
                script = Owner.Session.ScriptLibrary.GetCompoundOutcome(combineItem.StaticName, "?");

            // Unhandled
            if (script == null)
                script = Owner.Session.ScriptLibrary.GetRoutine(RoutineNames.UnhandledInventoryOutcome);

            if (script != null)
            {
                Owner.Session.AwaitScript(script);
                SceneController.Pop();
            }

            */
        }

        /*
        // PerformOutcome
        private bool PerformOutcome()
        {
            var item = Owner.SelectedItem;

            if (item == null || Owner.Target == null || Owner.Room == null)
                return false;

            Script? outcome = Owner.Session.ScriptLibrary.GetCompoundOutcome(Owner.Target.Name, item.StaticName);

            if (outcome == null)
                outcome = Owner.Session.ScriptLibrary.GetCompoundOutcome(Owner.Target.StaticName, item.StaticName);

            // Compound default
            if (outcome == null)
                outcome = Owner.Session.ScriptLibrary.GetOutcome(item.Name + ScriptSyntax.ScriptCompoundSeparator + ScriptSyntax.AnyEntityOp);

            // Default unhandled
            if (outcome == null)
                outcome = Owner.Session.ScriptLibrary.GetRoutine(RoutineNames.DefaultUnhandledOutcome);

            if (outcome != null)
            {
                SceneController.Pop();
                Owner.Session.BeginOutcome(outcome, Owner.Target, item);
                InvalidateContextMenu();
            }

            return outcome != null;
        }
        */

        // PerformMenuOption
        private void PerformMenuOption()
        {
            /*
            if (!contextMenu.HasOptions || Owner.SelectedItem == null || contextMenu.SelectedOption?.Key == null)
                return;

            var item = Owner.SelectedItem;
            if (item == null)
                return;

            if (contextMenu.SelectedOption.IsDisabled || item.IsShaking)
            {
                SoundManager.Play(SoundNames.InputError.ToString());
                return;
            }

            var optionKind = Enum.Parse<OptionKind>(contextMenu.SelectedOption.Key);

            // Move
            if (optionKind == OptionKind.Move)
            {
                DoMoveOption();
                return;
            }

            // Combine
            if (optionKind == OptionKind.Combine)
            {
                if (combineItem == null)
                {
                    combineItem = Owner.SelectedItem;
                    combineItemOriginalPosition = combineItem.Position;

                    if (combineItem != null)
                    {
                        SoundManager.Play(SoundNames.InventoryCombineBegin.ToString());
                        combineItem.ChangeVisualState(ItemVisualState.Combine, false);
                        combineItem.Slide(0, -slotHeight, tweenDuration);
                        InputManager.Suspend(tweenDuration);
                    }

                    InvalidateContextMenu();
                }
                else
                    PerformCombineOutcome();

                return;
            }

            // Use with
            if (optionKind == OptionKind.UseWith)
            {
                PerformOutcome();
                return;
            }

            else
                PerformVerbOutcome();

            */
        }

        // PerformVerbOutcome
        private void PerformVerbOutcome()
        {
            /*
            var item = Owner.SelectedItem;
            if (item == null)
                return;

            var close = item.CloseInventoryOnVerbOutcome && string.IsNullOrWhiteSpace(item.VerbErrorMessage);

            if (close)
                SceneController.Pop();

            item.PerformVerbOutcome();

            InvalidateContextMenu();

            if (!close)
                item.Shake();

            if (!item.HasParent)
            {
                LayoutItems();
                Owner.SelectedItem?.ChangeVisualState(ItemVisualState.Active, true);
                Invalidate();
            }
            */
        }

        // SelectNextItem
        private bool SelectNextItem() => MoveSelection(+1);

        // SelectPreviousItem
        private bool SelectPreviousItem() => MoveSelection(-1);

        // TerminateCombineMode
        private void TerminateCombineMode()
        {
            /*
            if (combineItem != null)
            {
                Owner.SelectItem(combineItem);
                combineItem = null;
            }

            LayoutItems();
            Owner.SelectedItem?.ChangeVisualState(ItemVisualState.Active, true);
            */
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!IsCurrentScene || Owner.Session.IsOutcomeInProgress)
                return;

            ///Shaders.SetColorReduction(fadeTween.CurrentValue);

            // Draw container
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            bottomGradient.Draw(gameTime);

            Game.SpriteBatch.End();

            // Draw items
            DrawItems(gameTime);

            // Draw texts
            DrawTexts(gameTime);

            // Context Menu
            //if (CanHandleInput && !fadeTween.IsRunning)
              //  contextMenu.Draw(gameTime);

            buttonClose.Draw(gameTime);
            buttonSelect.Draw(gameTime);
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            /*
            if (contextMenu.HandleInput(gameTime) == HandleInputResult.Handled)
                return HandleInputResult.Handled;

            // Previous item
            if (InputBindings.MenuLeft.IsPressed(VladInputHelper.PlayerIndex) || stick.IsLeft(VladInputHelper.PlayerIndex))
                SelectPreviousItem();

            // Next item
            else if (InputBindings.MenuRight.IsPressed(VladInputHelper.PlayerIndex) || stick.IsRight(VladInputHelper.PlayerIndex))
                SelectNextItem();

            // Menu Option
            else if (InputBindings.Select.IsPressed(VladInputHelper.PlayerIndex))
                PerformMenuOption();

            // Diary
            else if (buttonDiary.TestPressed(VladInputHelper.PlayerIndex))
            {
                var scene = new DiaryScene(Owner.Session);
                Game.SceneManager.Push(scene);
            }

            // Close
            else if (InputBindings.Close.IsPressed(VladInputHelper.PlayerIndex) || InputBindings.ShowInventory.IsPressed(VladInputHelper.PlayerIndex))
            {
                if (combineItem != null)
                    TerminateCombineMode();
                else
                {
                    SoundManager.Play(SoundNames.InventoryClose.ToString());
                    SceneController.Pop();
                }
            }
            */

            return HandleInputResult.Handled;
        }

        // OnInvalidate
        protected override void OnInvalidate()
        {
            base.OnInvalidate();
            InvalidateContextMenu();
            InvalidateItemStats();
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            base.OnLoadContent();

            //Owner.Session.HUD.BottomMessage.Hide();

            LayoutSlots();

            fadeTween.Start(TweenStyle.Linear, 0, 1, 100);

            combineItem = null;

            // Populate items
            for (int i = 0; i < Owner.Inventory.Items.Count; i++)
            {
                visualItems[i].Item = Owner.Inventory.Items[i];
            }

            LayoutItems();

            //Owner.SelectedItem?.ChangeVisualState(ItemVisualState.Active, true);

            Invalidate();

            //SoundManager.Play(SoundNames.InventoryOpen.ToString());
        }

        // OnUnloadContent
        protected override void OnUnloadContent()
        {
            base.OnUnloadContent();

            for (var i = 0; i < visualItems.Count; i++)
            {
                visualItems[i].Reset();
            }

            // Unload items
            /*
            if (Owner.Children.Count > 0)
            {
                for (int i = 0; i < Owner.Children.Count; i++)
                {
                    Owner.Children[i].UnloadContent();
                }
            }
            */
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            fadeTween.Update(gameTime);
            //contextMenu.Update(gameTime);
            bottomGradient.Update(gameTime);
            stick.Stick = GamePadThumbStick.Left;
            stick.Update(gameTime);
            buttonClose.Update(gameTime);
            buttonSelect.Update(gameTime);

            itemDescription.Update(gameTime);

            /*
            if (Owner.SelectedItem is DegradableItem item && item.Amount != lastKnownAmount)
                InvalidateContextMenuTitle();
            */

            // Update items
            for (int i = 0; i < Owner.Children.Count; i++)
            {
                Owner.Children[i].Update(gameTime);
            }

            base.OnUpdate(gameTime);
        }

        #endregion

        // CanHandleInput
        //public override bool CanHandleInput => !Owner.Session.IsOutcomeInProgress && base.CanHandleInput;

        // Close
        public void Close()
        {
            if (combineItem != null)
                TerminateCombineMode();
            //SoundManager.Play(SoundNames.InventoryClose.ToString());
            SceneController.Pop();
        }

        // Owner
        public Actor Owner { get; set; }
    }
}
