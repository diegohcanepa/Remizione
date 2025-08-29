using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione.UI
{
    /// <summary>
    /// UITargetMeter
    /// </summary>
    public sealed class UITargetMeter : GameObject
    {
        #region Private fields

        private int cooldown;
        private readonly TextSprite labelText;
        private readonly Meter meter;
        private GameThing? target;

        #endregion

        #region Constructor

        // Constructor
        public UITargetMeter(EngendroGame game)
            : base(game)
        {
            this.meter = new Meter(game, ColorPalette.Text.TerraDarker, ColorPalette.Text.Red, new(30, 3))
            {
                Alignment = HorizontalAlignment.Center,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.Top, 0, 4)
            };

            this.labelText = new(game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Bottom,
                Position = meter.BoundingBox.GetPoint(RectanglePoint.Top, 0, 1),
                Scale = ScaleInfo.Text.Medium
            };
        }

        #endregion

        #region Private members

        // ResetCooldown
        private void ResetCooldown() => cooldown = 5000;

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (target == null)
                return;

            Game.SpriteBatch.Begin(Game.Camera);
            meter.Draw(gameTime);
            labelText.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (target != null)
            {
                cooldown -= gameTime.ElapsedGameTime.Milliseconds;

                if (target.IsDead || cooldown <= 0)
                {
                    target = null;
                }
                else
                {
                    if (meter.Value != target.Health)
                    {
                        meter.Value = target.Health;
                        ResetCooldown();
                    }

                    meter.Update(gameTime);
                }
            }
        }

        #endregion

        // Target
        public GameThing? Target
        {
            get => target;
            set
            {
                if (value != target)
                {
                    target = value;
                 
                    if (target != null && target.MaxHealth == 0)
                        target = null;

                    if (target != null)
                    {
                        meter.MaximumValue = target.MaxHealth;
                        labelText.Text = target.DisplayName;
                        ResetCooldown();
                    }
                }
            }
        }
    }
}
