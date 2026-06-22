using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Globalization;

namespace ScaryCastle
{
    /// <summary>
    /// UIBossMeter
    /// </summary>
    public sealed class UIBossMeter : GameObject
    {
        #region Private fields

        private readonly Sprite amountContainer;
        private readonly TextSprite amountText;
        private readonly Sprite icon;
        private readonly TextSprite labelText;
        private readonly Meter meter;
        private readonly FloatTween shakeTween = new();
        private readonly List<Actor> targetList = [];

        #endregion

        #region Constructor

        // Constructor
        public UIBossMeter()
            : base()
        {
            this.meter = new Meter(ColorPalette.BossMeter.Back, ColorPalette.BossMeter.Fore, ColorPalette.BossMeter.Diff, new(60, 6), 1)
            {
                Position = Screen.HUDArea.GetPoint(RectanglePoint.Bottom, 0, -10)
            };

            this.labelText = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.Bottom,
                Position = meter.BoundingBox.GetPoint(RectanglePoint.Top, 0, .25f),
                Scale = ScaleInfo.Text.Large
            };

            this.amountContainer = new(Atlases.UI.BossMeterAmount)
            {
                PivotOrigin = RectanglePoint.Left,
                Position = meter.BoundingBox.GetPoint(RectanglePoint.Right, -1, 0)
            };

            this.amountText = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.Center,
                Position = amountContainer.BoundingBox.GetPoint(RectanglePoint.Center),
                Scale = ScaleInfo.Text.ExtraLarge,
                ShadowColor = ColorPalette.SceneShade,
                ShadowOffset = new(0, 1)
            };

            this.icon = new(Atlases.UI.BossMeter)
            {
                PivotOrigin = RectanglePoint.Right,
                Position = meter.BoundingBox.GetPoint(RectanglePoint.Left, 1, 0)
            };
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (targetList.Count == 0)
                return;

            Game.SpriteBatch.Begin(Game.Camera);
            meter.Draw(gameTime);
            icon.Draw(gameTime);
            amountContainer.Draw(gameTime);
            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            labelText.Draw(gameTime);
            amountText.X += shakeTween.CurrentValue;
            amountText.Draw(gameTime);
            amountText.X -= shakeTween.CurrentValue;
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (targetList.Count == 0)
                return;

            var currentTotalHP = 0;
            var allDead = true;

            for (int i = 0; i < targetList.Count; i++)
            {
                var t = targetList[i];

                if (t != null && !t.IsDead)
                {
                    currentTotalHP += t.HP;
                    allDead = false;
                }
            }

            if (allDead)
            {
                targetList.Clear();
                // Call end of run script
                //?session.AddCorridorExit();
            }
            else if (meter.Value != currentTotalHP)
            {
                meter.Value = currentTotalHP;
                amountText.Text = currentTotalHP.ToString(CultureInfo.InvariantCulture);
                shakeTween.Start(TweenStyle.CubicInOut, 0, .5f, 60, 4);
            }

            meter.Update(gameTime);
            shakeTween.Update(gameTime);
        }

        #endregion

        // Reset
        public void Reset()
        {
            targetList.Clear();
            amountText.Clear();
            labelText.Clear();
            meter.MaximumValue = 0;
            meter.Value = 0;
        }

        // SetTargets
        public void SetTargets(IList<Actor> targets)
        {
            Reset();

            targetList.AddRange(targets);

            int totalMaxHP = 0;
            int totalCurrentHP = 0;

            for (int i = 0; i < targets.Count; i++)
            {
                totalMaxHP += targets[i].MaxHP;
                totalCurrentHP += targets[i].HP;
            }

            meter.MaximumValue = totalMaxHP;
            meter.Value = totalCurrentHP;
            amountText.Text = totalCurrentHP.ToString(CultureInfo.InvariantCulture);

            if (targetList.Count > 1)
                labelText.Text = TextRepository.GetValue($"{targetList[0].DisplayNameKey}.Group");

            if (labelText.IsEmpty)
                labelText.Text = targets[0].DisplayName;
        }
    }
}
