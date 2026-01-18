using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ScaryCastle
{
    /// <summary>
    /// UILog
    /// </summary>
    public sealed class UILog : GameObject
    {
        private ItemDefinition? itemDefinition;
        private readonly FloatTween fadeTween = new() { StartDelay = 2600 };
        private readonly ImageSprite icon;
        private bool isWarning;
        private readonly TextSprite nounText;
        private int showCooldown;
        private LogVerb verb;
        private readonly TextSprite verbText;

        // Constructor
        public UILog(EngendroGame game)
            : base(game)
        {
            // Icon
            this.icon = new ImageSprite(game)
            {
                PivotOrigin = RectanglePoint.Top,
                Scale = ScaleInfo.UIElement.Small
            };

            // Verb
            this.verbText = new(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Top,
                Scale = ScaleInfo.Text.Giant
            };

            // Noun
            this.nounText = new(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Top,
                Scale = ScaleInfo.Text.VeryLarge
            };
        }

        #region Private members

        // ShowCore
        private void ShowCore(string verb, string noun, bool isWarning, AtlasImage? image)
        {
            verbText.Color = isWarning ? ColorPalette.Text.Orange : ColorPalette.Text.Green;
            verbText.Position = new Vector2(Screen.Center.X, 4);
            verbText.Text = verb;

            nounText.Position = verbText.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, -2);
            nounText.Text = noun;
            icon.Image = image;
            icon.Position = nounText.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, -1);

            fadeTween.Start(TweenStyle.CubicIn, 1, 0, 1000);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!fadeTween.IsRunning)
                return;

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            verbText.Draw(gameTime);
            nounText.Draw(gameTime);
            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(Game.Camera);
            icon.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (showCooldown > 0)
            {
                showCooldown -= gameTime.ElapsedGameTime.Milliseconds;

                if (showCooldown <= 0 && itemDefinition != null)
                {
                    ShowCore(Localization.GetValue(verb), itemDefinition.LocalizedDisplayName, isWarning, itemDefinition.Image);

                    if (verb == LogVerb.Found)
                    {
                        if (itemDefinition.PickupSound != null)
                            itemDefinition.PickupSound.Play();
                        else
                            Sound.Play(SoundNames.PickupGeneric);
                    }

                    else if (verb == LogVerb.Requires)
                        Sound.Play(SoundNames.Error);

                    itemDefinition = null;
                }

                return;
            }

            fadeTween.Update(gameTime);
            verbText.Update(gameTime);
            nounText.Update(gameTime);
            icon.Update(gameTime);

            verbText.Opacity = fadeTween.IsRunning ? fadeTween.CurrentValue : 1;
            nounText.Opacity = verbText.Opacity;
            icon.Opacity = verbText.Opacity;
        }

        #endregion

        // Hide
        public void Hide()
        {
            fadeTween.Stop();
        }

        // Show
        public void Show(string message, bool isWarning, AtlasImage? image = null)
        {
            ShowCore(message, string.Empty, isWarning, image);
        }

        // Show
        public void Show(LogVerb verb, string noun, bool isWarning, AtlasImage? image = null)
        {
            ShowCore(Localization.GetValue(verb), noun, isWarning, image);
        }

        // Show
        public void Show(LogVerb verb, ItemDefinition itemDefinition, int delay)
        {
            this.showCooldown = delay;
            this.verb = verb;
            this.itemDefinition = itemDefinition;
            this.isWarning = verb is LogVerb.Used or LogVerb.Requires;
        }
    }
}
