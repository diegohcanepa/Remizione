using Engendro;
using Microsoft.Xna.Framework;
using System.Globalization;

namespace ScaryCastle
{
    /// <summary>
    /// UIScore
    /// </summary>
    public class UIScore : GameObject
    {
        private const int duration = 2000;

        private int deltaScore;
        private bool isInitializing = true;
        private readonly FloatTween tween = new();
        private readonly TextSprite valueText;

        // Constructor
        public UIScore(EngendroGame game, Color textColor, Vector2 textScale, bool progressive = true)
        {
            this.Progressive = progressive;

            // Score text
            this.valueText = new(Fonts.CommonOutline)
            {
                Color = textColor,
                PivotOrigin = RectanglePoint.Top,
                Scale = textScale
            };

            this.Value = 0;

            isInitializing = true;
            Progressive = progressive;
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            valueText.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (tween.IsRunning)
            {
                tween.Update(gameTime);
                int newDeltaScore = (int)tween.CurrentValue;
                if (newDeltaScore != deltaScore)
                {
                    deltaScore = newDeltaScore;
                    valueText.Text = deltaScore.ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        #endregion

        // BoundingBox
        public RectangleF BoundingBox => valueText.BoundingBox;

        // Color
        public Color Color
        {
            get => valueText.Color;
            set => valueText.Color = value;
        }

        // PivotOrigin
        public RectanglePoint PivotOrigin
        {
            get => valueText.PivotOrigin;
            set => valueText.PivotOrigin = value;
        }

        // Position
        public Vector2 Position
        {
            get => valueText.Position;
            set => valueText.Position = value;
        }

        // Progressive
        public bool Progressive { get; }

        // SetInitialValue
        public void SetInitialValue(int value)
        {
            isInitializing = true;
            this.Value = value;
        }

        // Value
        public int Value
        {
            get;
            set
            {
                if (value != field || isInitializing)
                {
                    if (!isInitializing)
                        tween.Start(TweenStyle.Linear, field, value, duration);

                    field = value;
                    valueText.Text = field.ToString(CultureInfo.InvariantCulture);

                    if (Progressive)
                        isInitializing = false;
                }
            }
        }
    }
}
