using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
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
        private int value;
        private readonly TextSprite valueText;

        // Constructor
        public UIScore(EngendroGame game, Color textColor, bool progressive = true)
            : base(game)
        {
            this.Progressive = progressive;

            // Score text
            this.valueText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = textColor,
                PivotOrigin = RectanglePoint.Top,
                Scale = ScaleInfo.Text.ExtraLarge,
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
                    valueText.Text = deltaScore.ToString();
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
            get => value;
            set
            {
                if (value != this.value || isInitializing)
                {
                    if (!isInitializing)
                        tween.Start(TweenStyle.Linear, this.value, value, duration);

                    this.value = value;
                    valueText.Text = this.value.ToString();

                    if (Progressive)
                        isInitializing = false;
                }
            }
        }
    }
}
