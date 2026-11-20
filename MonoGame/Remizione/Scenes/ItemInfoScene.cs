using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ItemInfoScene
    /// </summary>
    public sealed class ItemInfoScene : SessionOverlayScene
    {
        #region Private fields

        private readonly UITextButton actionButton;
        private readonly TextSprite amountText;
        private readonly UIImageButton backButton = UIImageButton.CreateBackButton();
        private readonly TextSprite breakChancesText;
        private readonly UITextButton dropButton;
        private readonly TextSprite effectText;
        private readonly TextSprite descriptionText;
        private readonly Sprite image;
        private readonly Sprite imageSlot;
        private Item? item;
        private readonly TextSprite nameText;
        private readonly Sprite stateIcon;
        private readonly TextSprite stateText;
        private readonly Sprite shadow;

        #endregion

        #region Constructor

        // Constructor
        public ItemInfoScene(GameSession session)
            : base(session)
        {
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
                Color = Colors.Palette.Text,
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
                Color = Colors.Palette.TextDark,
                MaximumWidth = 150,
                PivotOrigin = RectanglePoint.Top,
                Position = effectText.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, 4),
                Scale = ScaleInfo.Text.VeryLarge
            };

            // Perish state icon
            this.stateIcon = new(Atlases.UI.ItemPerishStates[0])
            {
                PivotOrigin = RectanglePoint.Left,
                Position = imageSlot.BoundingBox.GetPoint(RectanglePoint.Right, 3, 0),
                Scale = ScaleInfo.UIElement.Medium
            };

            // Perish state text
            this.stateText = new(Fonts.Common)
            {
                Color = Colors.Palette.Text,
                PivotOrigin = RectanglePoint.Left,
                Scale = ScaleInfo.Text.Large
            };

            // Amount text
            this.amountText = new(Fonts.Common)
            {
                Color = Colors.Palette.Text,
                PivotOrigin = RectanglePoint.Left,
                Position = imageSlot.BoundingBox.GetPoint(RectanglePoint.Right, 3, 0),
                Scale = ScaleInfo.Text.ExtraLarge
            };

            // Break chances text
            this.breakChancesText = new(Fonts.Common)
            {
                Color = Colors.Palette.Text,
                PivotOrigin = RectanglePoint.Left,
                Position = imageSlot.BoundingBox.GetPoint(RectanglePoint.Right, 3, 0),
                Scale = ScaleInfo.Text.Medium
            };

            // Drop Button
            this.dropButton = new()
            {
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, -10, -5)
            };

            // Action Button
            this.actionButton = new()
            {
                PivotOrigin = RectanglePoint.RightBottom,
                Y = dropButton.Y
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

            if (item != null)
            {
                stateIcon.Draw(gameTime);
                stateText.Draw(gameTime);

                if (item.Definition.IsStackable)
                {
                    amountText.Draw(gameTime);
                }

                //if (item.BreakChance > 0)
                //    breakChancesText.Draw(gameTime);
            }

            dropButton.Draw(gameTime);

            if (actionButton.IsEnabled)
                actionButton.Draw(gameTime);

            backButton.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            if (backButton.TestPressed() ||
                InputBindings.Back.IsPressed(PlayerIndex.One) ||
                InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
            {
                Game.SceneManager.Pop();
                Sound.Play(SoundNames.ButtonPress);
                return HandleInputResult.Handled;
            }

            if (item != null && Session.Room is { } room && Session.Player is { } player)
            {
                // Drop
                if (dropButton.TestPressed())
                {
                    Game.SceneManager.Pop();
                    player.DropItem(item);
                    return HandleInputResult.Handled;
                }

                // Use
                if (actionButton.TestPressed())
                {
                    Game.SceneManager.Pop();
                    player.ExecuteAction(item, player);
                    return HandleInputResult.Handled;
                }
            }

            return base.OnHandleInput(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            dropButton.Update(gameTime);
            actionButton.Update(gameTime);
            nameText.Update(gameTime);
            descriptionText.Update(gameTime);
            backButton.Update(gameTime);
        }

        #endregion

        // Show
        public void Show(Item item)
        {
            this.item = item;

            nameText.Color = item.Definition.LabelColor;
            nameText.Text = item.Definition.Label;
            effectText.Text = item.Definition.EffectDescription;
            descriptionText.Text = item.Definition.Description;
            image.RenderImage = item.Definition.Image;

            if (item.Definition.ItemKind == ItemKind.Organic)
            {
                stateText.Text = Localization.GetValue(item.PerishState);
                stateText.Color = item.PerishState == ItemPerishState.Rotten ? Colors.Palette.Red : Colors.Palette.Text;
                stateIcon.RenderImage = Atlases.UI.ItemPerishStates[(int)item.PerishState];
            }
            else if (!item.HasSafeUses)
            {
                stateText.Text = TextRepository.GetValue("Misc.Worn");
                stateText.Color = Colors.Palette.Text;
                stateIcon.RenderImage = Atlases.UI.WarningIcon;
            }
            else
            {
                stateText.Clear();
                stateIcon.RenderImage = null;
            }

            stateText.Position = stateIcon.BoundingBox.GetPoint(RectanglePoint.Right, 2, .5f);
            amountText.Text = item.Definition.IsStackable ? $"x{item.Amount}" : string.Empty;
            breakChancesText.Text = item.Definition.HasBreakChance ? $"{TextRepository.GetValue("Misc.BreakChance")}{System.Environment.NewLine}{(item.BreakChance * 100).Round()}%" : null;
            breakChancesText.Color = item.BreakChance > .65f ? Colors.Palette.Red : Colors.Palette.Orange;
            dropButton.Text = item.Definition.ItemKind == ItemKind.Key ? Localization.GetValue(UserAction.Drop) : Localization.GetValue(UserAction.Discard);
            shadow.RenderImage = item.Definition.Image;
            actionButton.IsEnabled = item.Definition.AllowSelfAction;

            if (actionButton.IsEnabled)
            {
                actionButton.X = dropButton.BoundingBox.GetPoint(RectanglePoint.LeftBottom, -8, 0).X;
                actionButton.Text = Localization.GetValue(item.Definition.SelfUseAction);
            }
        }
    }
}
