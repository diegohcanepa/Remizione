using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Globalization;

namespace Remizione
{
    /// <summary>
    /// UIMessage
    /// </summary>
    public sealed class UIMessage : GameObject
    {
        private readonly FloatTween fadeTween = new();
        private readonly Sprite icon = new() { PivotOrigin = RectanglePoint.Right };
        private readonly TextSprite messageText;
        private readonly Vector2 scale = ScaleInfo.Text.VeryLarge;
        private readonly Vector2Tween scaleTween = new();

        // Constructor
        public UIMessage(RectanglePoint pivotOrigin, Vector2 position)
        {
            this.messageText = new(Fonts.CommonOutline)
            {
                MaximumWidth = (int)(Screen.HUDArea.Width * .7f),
                PivotOrigin = pivotOrigin,
                Position = position
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!fadeTween.IsRunning)
                return;

            Game.SpriteBatch.Begin(Game.Camera);
            icon.Draw(gameTime);
            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            messageText.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            fadeTween.Update(gameTime);
            messageText.Update(gameTime);
            messageText.Opacity = fadeTween.IsRunning ? fadeTween.CurrentValue : 1;
            icon.Opacity = fadeTween.CurrentValue;
            icon.Position = messageText.BoundingBox.GetPoint(RectanglePoint.Left, -2, -1);

            if (IsVisible && !fadeTween.IsRunning)
                IsVisible = false;
        }

        #endregion

        // Hide
        public void Hide()
        {
            fadeTween.Stop();
            IsVisible = false;
        }

        // InventoryFull
        public void InventoryFull()
        {
            var text = Localization.GetValue(MessageKind.InventoryFull);
            Show(text, Colors.Palette.Yellow, Atlases.UI.Sack, 3000);
        }

        // IsVisible
        public bool IsVisible { get; private set; }

        // ItemBroken
        public void ItemBroken(Item item)
        {
            var text = Localization.GetValue(MessageKind.ItemBroken) + " " + item.Definition.Label;
            Show(text, Colors.Palette.Orange, item.Definition.Image, 5000, true);
        }

        // Show
        public void Show(MessageKind message, AtlasImage? image)
        {
            var text = Localization.GetValue(message);
            var color = Colors.Palette.Text;
            Show(text, color, image, 3000);
        }

        // Show
        public void Show(string text)
        {
            Show(text, Colors.Palette.Text, null, 3000);
        }

        // Show
        public void Show(string text, Color color, AtlasImage? image, int duration, bool isItem = false)
        {
            messageText.Color = color;
            messageText.Text = text;
            icon.RenderImage = image;
            fadeTween.StartDelay = duration;
            fadeTween.Start(TweenStyle.CubicIn, 1, 0, 200);
            scaleTween.Start(TweenStyle.CubicIn, scale * .8f, scale, 50);
            messageText.Tweens.ScaleTween = scaleTween;
            IsVisible = true;
        }

        // StackFull
        public void StackFull(Item item)
        {
            var text = Localization.GetValue(MessageKind.StackFull) + " " + item.Definition.Label;
            Show(text, Colors.Palette.Orange, item.Definition.Image, 5000, true);
        }
    }
}
