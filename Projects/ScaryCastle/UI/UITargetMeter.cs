using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
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
            this.meter = new Meter(game, ColorPalette.Text.TerraDarker, ColorPalette.Text.Red, new(30, 5))
            {
                Alignment = HorizontalAlignment.Center,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.Top, 0, 8)
            };

            this.labelText = new(game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Bottom,
                Position = meter.BoundingBox.GetPoint(RectanglePoint.Top),
                Scale = ScaleInfo.Text.Large
            };
        }

        #endregion

        #region Private members

        // ResetCooldown
        private void ResetCooldown() => cooldown = 3000;

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
                    if (meter.Value != target.HP)
                    {
                        meter.Value = target.HP;
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

                    if (target != null && target.MaxHP == 0)
                        target = null;

                    if (target != null)
                    {
                        meter.MaximumValue = target.MaxHP;
                        labelText.Text = target.LocalizedDisplayName;
                        ResetCooldown();
                    }
                }
            }
        }
    }
}
