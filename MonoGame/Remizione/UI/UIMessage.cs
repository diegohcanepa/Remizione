using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione
{
    /// <summary>
    /// UIMessage
    /// </summary>
    public sealed class UIMessage : GameObject
    {
        private readonly Sprite container = new(Atlases.UI.GetImage("MessageContainer")) { PivotOrigin = RectanglePoint.Center };
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
                MaximumWidth = 170,
                PivotOrigin = pivotOrigin,
                Position = position
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!IsVisible)
                return;

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp);
            //container.Draw(gameTime);

            if (icon.RenderImage != null)
                icon.Draw(gameTime);

            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            messageText.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (!IsVisible)
                return;

            fadeTween.Update(gameTime);
            messageText.Update(gameTime);

            float currentOpacity = fadeTween.IsRunning ? fadeTween.CurrentValue : 1f;
            messageText.Opacity = currentOpacity;

            if (icon.RenderImage != null)
            {
                icon.Opacity = currentOpacity;
                icon.Position = messageText.BoundingBox.GetPoint(RectanglePoint.Left);
            }

            if (!fadeTween.IsRunning)
                IsVisible = false;
        }

        #endregion

        // Hide
        public void Hide()
        {
            fadeTween.Stop();
            scaleTween.Stop();
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

        // Show
        public void Show(MessageKind message, AtlasImage? image = null)
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
        public void Show(string text, Color color, AtlasImage? image, int duration)
        {
            messageText.Color = color;
            messageText.Text = text;
            icon.RenderImage = image;
            fadeTween.StartDelay = duration;
            fadeTween.Start(TweenStyle.CubicIn, 1f, 0f, 200);
            scaleTween.Start(TweenStyle.CubicIn, scale * .8f, scale, 50);
            messageText.Tweens.ScaleTween = scaleTween;
            container.Position = messageText.BoundingBox.Center + Vector2.UnitY;
            IsVisible = true;
        }

        // ShowItem
        public void ShowItem(Item item, Color color)
        {
            Show(item.Definition.Label, color, item.Definition.Image, 5000);
        }

        // StackFull
        public void StackFull()
        {
            Show(Localization.GetValue(MessageKind.StackFull), Colors.Palette.Orange, null, 5000);
        }
    }
}