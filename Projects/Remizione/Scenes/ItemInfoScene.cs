using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ItemInfoScene
    /// </summary>
    public sealed class ItemInfoScene : Scene
    {
        #region Private fields

        private readonly TextSprite amountText;
        private readonly UIImageButton backButton = UIImageButton.CreateBackButton();
        private readonly UITextButton button;
        private readonly TextSprite effectText;
        private readonly TextSprite descriptionText;
        private readonly Sprite image;
        private readonly Sprite imageSlot;
        private Item? item;
        private readonly Sprite perishStateIcon;
        private readonly TextSprite perishStateText;
        private readonly GameSession session;
        private readonly Sprite shadow;
        private readonly TextSprite nameText;

        #endregion

        #region Constructor

        // Constructor
        public ItemInfoScene(GameSession session)
        {
            this.session = session;
            this.BackgroundColor = Color.Black;

            PausePreviousScenes = true;
            IsOpaque = true;

            backButton.Position = Screen.HUDArea.GetPoint(RectanglePoint.LeftTop);

            // Image slot
            this.imageSlot = new()
            {
                PivotOrigin = RectanglePoint.Top,
                Position = Screen.Area.GetPoint(RectanglePoint.Top, 0, 10),
                RenderImage = Atlases.UI.InventoryItemSlots[0]
            };

            // Image
            this.image = new()
            {
                PivotOrigin = RectanglePoint.Center,
                Position = imageSlot.BoundingBox.GetPoint(RectanglePoint.Center, 0, -1),
            };

            // Shadow
            shadow = new()
            {
                Color = Color.Black,
                Opacity = .3f,
                PivotOrigin = RectanglePoint.Center,
                Y = image.BoundingBox.Center.Y + 1,
                X = image.BoundingBox.Center.X - .5f
            };

            // Name text
            this.nameText = new(Fonts.Common)
            {
                Color = Colors.MouseCursor.Tooltip,
                PivotOrigin = RectanglePoint.Top,
                Position = imageSlot.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, 3),
                Scale = ScaleInfo.Text.Giant,
                Text = "Text"
            };

            // Effect
            this.effectText = new(Fonts.Common)
            {
                Color = Colors.Inventory.EffectDescription,
                MaximumWidth = 150,
                PivotOrigin = RectanglePoint.Top,
                Position = nameText.BoundingBox.GetPoint(RectanglePoint.Bottom),
                Scale = ScaleInfo.Text.VeryLarge,
                Text = "Text"
            };

            // Description
            this.descriptionText = new(Fonts.Common)
            {
                Color = Colors.Inventory.EffectDescription,
                MaximumWidth = 150,
                Opacity = .7f,
                PivotOrigin = RectanglePoint.Top,
                Position = effectText.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, 4),
                Scale = ScaleInfo.Text.VeryLarge
            };

            // Perish state icon
            this.perishStateIcon = new(Atlases.UI.ItemPerishStates[0])
            {
                PivotOrigin = RectanglePoint.Left,
                Position = imageSlot.BoundingBox.GetPoint(RectanglePoint.Right, 3, 0),
                Scale = ScaleInfo.UIElement.Medium
            };

            // Perish state text
            this.perishStateText = new(Fonts.Common)
            {
                Color = Colors.Palette.Text,
                PivotOrigin = RectanglePoint.Left,
                Position = perishStateIcon.BoundingBox.GetPoint(RectanglePoint.Right, 2, .5f),
                Scale = ScaleInfo.Text.Large
            };

            // Amount text
            this.amountText = new(Fonts.Common)
            {
                Color = Colors.Palette.Terra,
                PivotOrigin = RectanglePoint.Left,
                Position = imageSlot.BoundingBox.GetPoint(RectanglePoint.Right, 3, 0),
                Scale = ScaleInfo.Text.VeryLarge
            };

            // Button
            this.button = new()
            {
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, -10, -5),
                Text = Localization.GetValue(UserAction.Drop)
            };
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            imageSlot.Draw(gameTime);
            shadow.Draw(gameTime);
            image.Draw(gameTime);
            nameText.Draw(gameTime);
            effectText.Draw(gameTime);
            descriptionText.Draw(gameTime);

            if (item?.Definition.IsPerishable == true)
            {
                perishStateIcon.Draw(gameTime);
                perishStateText.Draw(gameTime);
            }

            if (item?.Definition.IsStackable == true)
                amountText.Draw(gameTime);

            if (button.IsEnabled)
                button.Draw(gameTime);
            backButton.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            if (backButton.TestPressed() || InputBindings.Back.IsPressed(PlayerIndex.One))
            {
                Game.SceneManager.Pop();
                return HandleInputResult.Handled;
            }

            if (button.TestPressed())
            {
                if (item != null && session.Room is { } room && session.Player is { } player)
                {
                    item.Remove();
                    Sound.Play(SoundNames.ItemDiscard);
                    ItemOrb.Drop(item.Definition, item.Amount, room, player.Position);
                }

                Game.SceneManager.Pop();
                return HandleInputResult.Handled;
            }

            return base.OnHandleInput(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            button.Update(gameTime);
            nameText.Update(gameTime);
            descriptionText.Update(gameTime);
            backButton.Update(gameTime);
        }

        #endregion

        // Show
        public void Show(Item item)
        {
            this.item = item;
            this.nameText.Color = item.Definition.IsKeyItem ? Colors.Inventory.KeyItem : Colors.Inventory.Item;
            this.nameText.Text = item.Definition.GetLabel(item.Amount);
            this.effectText.Text = item.Definition.EffectDescription;
            this.descriptionText.Text = item.Definition.Description;
            this.image.RenderImage = item.Definition.Image;
            this.perishStateText.Text = Localization.GetValue(item.PerishState);
            this.perishStateText.Color = item.PerishState == ItemPerishState.Rotten ? Colors.Palette.Red : Colors.Palette.Text;
            this.perishStateIcon.RenderImage = Atlases.UI.ItemPerishStates[(int)item.PerishState];
            this.amountText.Text = item.Definition.IsStackable ? $"x{item.Amount}" : string.Empty;
            this.shadow.RenderImage = item.Definition.Image;
            this.button.IsEnabled = item.Definition.AllowDrop;
        }
    }
}
