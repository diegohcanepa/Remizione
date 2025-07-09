using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// InventoryScene
    /// </summary>
    public sealed class NewInventoryScene : Scene
    {
        #region Private fields

        private const float animationSpeed = 14;
        private readonly ImageSprite bottomGradient;
        private readonly TextSprite itemNameText;
        private static readonly Vector2 slotPosition = new(Screen.Center.X, Screen.HUDArea.Bottom - 14);
        private int selectedIndex;
        private readonly ImageSprite slotImage;
        private readonly List<InventoryItem> slots = [];
        private const int spaceBetweenIcons = 15;
        private readonly StickInputController stick = new(GamePadThumbStick.Left) { AutoRepeatRate = 150 };
        private const int visibleRange = 13;
        private float visualIndex;

        #endregion

        #region Constructor

        // Constructor
        public NewInventoryScene(Actor owner)
            : base(owner.Game)
        {
            this.Owner = owner;

            // Bottom gradient
            bottomGradient = new ImageSprite(owner.Game, Atlases.UI.BottomGradient)
            {
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.Area.GetPoint(RectanglePoint.Bottom),
                Scale = new Vector2(1, 1.2f)
            };

            // SlotImage
            this.slotImage = new(Game, Atlases.UI.InventorySlot)
            {
                PivotOrigin = RectanglePoint.Middle,
                Position = slotPosition
            };

            // Item name
            itemNameText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.Bottom,
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.Top),
                Scale = ScaleInfo.Text.VeryLarge
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
                if (index < 0 || index >= slots.Count)
                    continue;

                // desplazamiento relativo animado
                float offset = i - (visualIndex - (int)visualIndex);
                slots[index].Position = slotPosition + new Vector2(offset * spaceBetweenIcons, 0);

                // Escala y opacidad basadas en distancia
                float distance = MathF.Abs(offset);
                float scale = MathF.Max(.6f, .8f - distance * .2f); // escala mínima 0.6
                float alpha = MathF.Max(.3f, 1 - distance * .3f); // transparencia mínima 0.3

                slots[index].Opacity = alpha;
                slots[index].Scale = new(scale);

                slots[index].Draw(gameTime);
            }
        }

        // Select
        private void Select(int index)
        {
            if (index >= slots.Count)
                return;

            selectedIndex = index;
            itemNameText.Text = slots[index].Item.DisplayText;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!IsCurrentScene || Owner.Session.IsOutcomeInProgress)
                return;

            // Gradient
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            bottomGradient.Draw(gameTime);
            itemNameText.Draw(gameTime);
            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(Game.Camera);
            slotImage.Draw(gameTime);
            DrawItems(gameTime);
            Game.SpriteBatch.End();
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            if (InputBindings.SelectLeft.IsPressed(PlayerIndex.One) || stick.IsLeft(PlayerIndex.One))
            {
                Select(Math.Max(0, selectedIndex - 1));
                Sound.Play(SoundNames.UINavigation);
                return HandleInputResult.Handled;
            }

            else if (InputBindings.SelectRight.IsPressed(PlayerIndex.One) || stick.IsRight(PlayerIndex.One))
            {
                Select(Math.Min(slots.Count - 1, selectedIndex + 1));
                Sound.Play(SoundNames.UINavigation);
                return HandleInputResult.Handled;
            }

            return HandleInputResult.Handled;
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            Owner.Stand();
            base.OnLoadContent();

            slots.Clear();
            for (var i = 0; i < Owner.Inventory.Items.Count; i++)
            {
                slots.Add(new(Owner.Inventory.Items[i]));
            }

            Select(0);
        }

        // OnUnloadContent
        protected override void OnUnloadContent()
        {
            base.OnUnloadContent();
            slots.Clear();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            bottomGradient.Update(gameTime);
            stick.Update(gameTime);
            
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            visualIndex += (selectedIndex - visualIndex) * MathF.Min(1f, animationSpeed * deltaTime);

            base.OnUpdate(gameTime);
        }

        #endregion

        // Owner
        public Actor Owner { get; set; }

        // InventoryItem
        private sealed class InventoryItem : GameObject
        {
            private readonly ImageSprite image;

            // Constructor
            public InventoryItem(Item item)
                : base(item.Owner.Game)
            {
                this.Item = item;

                // Image
                this.image = new(Game, item.MetaItem.Image)
                {
                    PivotOrigin = RectanglePoint.Middle
                };
            }

            // OnDraw
            protected override void OnDraw(GameTime gameTime)
            {
                image.Draw(gameTime);
            }

            // OnUpdate
            protected override void OnUpdate(GameTime gameTime)
            {
            }

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
