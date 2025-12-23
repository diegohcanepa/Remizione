using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// NewInventoryScene
    /// </summary>
    public sealed class NewInventoryScene : Scene
    {
        #region Private fields

        private readonly TextSprite amountText;
        private const float animationSpeed = 14;
        private readonly ImageSprite bottomGradient;
        private readonly UIButton buttonClose;
        private readonly UIButton buttonUse;
        private readonly TextSprite chanceText;
        private readonly TextSprite itemNameText;
        private int selectedIndex;
        private readonly ImageSprite slotImage;
        private static readonly Vector2 slotPosition = new(Screen.Center.X, Screen.HUDArea.Bottom - 16);
        private const int spaceBetweenIcons = 15;
        private readonly StickInputController stick = new(GamePadThumbStick.Left) { AutoRepeatRate = 150 };
        private Prop? target;
        private readonly TextSprite title;
        private const int visibleRange = 13;
        private float visualIndex;
        private readonly List<VisualItem> visualItems = [];

        #endregion

        #region Constructor

        // Constructor
        public NewInventoryScene(Inventory pilgrimSack)
            : base(pilgrimSack.Session.Game)
        {
            this.Inventory = pilgrimSack;

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

            // Chance text
            chanceText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.LeftBottom, 0, -3),
                Scale = ScaleInfo.Text.Huge
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
            buttonUse = new UIButton(Game, InputBindings.UseFriendlyItem)
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
            amountText.Text = selectedIndex < 0 ? null : visualItems[selectedIndex].Item.GetDisplayAmount();
        }

        // Select
        private void Select(int index)
        {
            if (index >= visualItems.Count)
                return;

            selectedIndex = index;
            var item = visualItems[index].Item;
            itemNameText.Text = index < 0 ? null : item.DisplayText;

            if (target != null)
            {
                if (item.SkillChance == 0)
                {
                    chanceText.Clear();
                }
                else
                {
                    var chance = item.SkillChance == 100 ? 100 : item.SkillChance - Math.Abs(target.SkillChancePenalty);
                    chanceText.Text = $"{Localization.GetValue(ItemProperty.Chance)}: {chance}%";

                    if (chance == 100)
                        chanceText.Color = ColorPalette.Text.Green;
                    else if (chance <= 25)
                        chanceText.Color = ColorPalette.Text.Orange;
                    else
                        chanceText.Color = ColorPalette.Text.Default;
                }
            }

            InvalidateSlot();
        }

        // SelectedItem
        private VisualItem? SelectedItem => selectedIndex == -1 ? null : visualItems[selectedIndex];

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!IsCurrentScene || Inventory.Session.IsOutcomeInProgress)
                return;

            // Gradient
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            bottomGradient.Draw(gameTime);
            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(Game.Camera);
            title.Draw(gameTime);
            itemNameText.Draw(gameTime);
            chanceText.Draw(gameTime);
            slotImage.Draw(gameTime);
            amountText.Draw(gameTime);
            DrawItems(gameTime);
            Game.SpriteBatch.End();

            buttonClose.Draw(gameTime);
            buttonUse.Draw(gameTime);
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

            // Close
            if (buttonClose.TestPressed(PlayerIndex.One))
            {
                SceneController.Pop();
                return HandleInputResult.Handled;
            }

            // Use
            if (buttonUse.TestPressed(PlayerIndex.One))
            {
                var itemName = SelectedItem?.Item.Name;
                SceneController.Pop();

                if (itemName != null && Inventory.Session.KeyItemTarget != null)
                {
                    var scriptName = $"{Inventory.Session.KeyItemTarget.DeclaredName}-With-{itemName}";
                    if (Inventory.Session.ScriptLibrary.FindRoutine(scriptName) is Script script)
                        Inventory.Session.AwaitScript(script);
                }

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

            target = Inventory.Session.OutcomeTarget as Prop;

            if (target != null)
            {
                visualItems.Clear();

                var friendlyItems = Inventory.Session.GetFriendlyItems(target.DeclaredName);

                for (var i = 0; i < friendlyItems.Length; i++)
                {
                    if (Inventory.Find(friendlyItems[i].Name) is Item item)
                        visualItems.Add(new(Inventory.Session.Game, item));
                }

                Select(0);

                visualIndex = selectedIndex;
            }
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
            buttonUse.Update(gameTime);
        }

        #endregion

        // Inventory
        public Inventory Inventory { get; set; }

        // Text
        public string? Text
        {
            get => title.Text;
            set => title.Text = value;
        }

        /// <summary>
        /// VisualItem
        /// </summary>
        private sealed class VisualItem : GameObject
        {
            private readonly ImageSprite image;

            // Constructor
            public VisualItem(ScaryCastleGame game, Item item)
                : base(game)
            {
                this.Item = item;

                // Image
                this.image = new(Game, item.MetaItem.Image)
                {
                    PivotOrigin = RectanglePoint.Center
                };
            }

            // OnDraw
            protected override void OnDraw(GameTime gameTime)
            {
                image.Draw(gameTime);
            }

            // BoundingBox
            public RectangleF BoundingBox => image.BoundingBox;

            // Item
            public Item Item { get; }

            // Opacity
            public float Opacity
            {
                get => image.Opacity;
                set => image.Opacity = value;
            }

            // Position
            public Vector2 Position
            {
                get => image.Position;
                set => image.Position = value;
            }

            // Scale
            public Vector2 Scale
            {
                get => image.Scale;
                set => image.Scale = value;
            }
        }
    }
}
