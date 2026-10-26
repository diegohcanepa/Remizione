using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// UICapacity
    /// </summary>
    public sealed class UICapacity : GameObject
    {
        #region Private fields

        private readonly TextSprite currentAmount;
        private int lastCurrentAmount = int.MinValue;
        private int lastMaximumAmount = int.MinValue;
        private readonly TextSprite maximumAmount;

        #endregion

        #region Constructor

        // Constructor
        public UICapacity()
            : base()
        {
            // Current amount
            this.currentAmount = new(Fonts.CommonOutline)
            {
                Color = Colors.Palette.Terra,
                PivotOrigin = RectanglePoint.Left,
                Scale = ScaleInfo.Text.Large
            };

            // Maximum amount
            this.maximumAmount = new(Fonts.CommonOutline)
            {
                Color = Colors.Palette.TerraDark,
                PivotOrigin = RectanglePoint.Left,
                Scale = ScaleInfo.Text.Medium
            };

            Refresh();
        }

        #endregion

        #region Private members

        // Refresh
        private void Refresh()
        {
            currentAmount.Tweens.Reset();

            currentAmount.Text = $"{CurrentAmount}";
            lastCurrentAmount = CurrentAmount;
            lastMaximumAmount = MaximumAmount;
            maximumAmount.Text = $"| {MaximumAmount}";
            maximumAmount.Position = currentAmount.BoundingBox.GetPoint(RectanglePoint.Right, .5f, 0);

            BoundingBox = RectangleF.Union(currentAmount.BoundingBox, maximumAmount.BoundingBox);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            currentAmount.Draw(gameTime);
            maximumAmount.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (lastCurrentAmount != CurrentAmount || lastMaximumAmount != MaximumAmount)
                Refresh();

            currentAmount.Update(gameTime);
        }

        #endregion

        // BoundingBox
        public RectangleF BoundingBox { get; private set; }

        // CurrentAmount
        public int CurrentAmount
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    Refresh();
                }
            }
        }

        // MaximumAmount
        public int MaximumAmount
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    Refresh();
                }
            }
        }

        // PivotOrigin
        public RectanglePoint PivotOrigin
        {
            get => currentAmount.PivotOrigin;
            set
            {
                if (value != currentAmount.PivotOrigin)
                {
                    currentAmount.PivotOrigin = value;
                    Refresh();
                }
            }
        }

        // Position
        public Vector2 Position
        {
            get => currentAmount.Position;
            set
            {
                if (value != currentAmount.Position)
                {
                    currentAmount.Position = value;
                    Refresh();
                }
            }
        }
    }
}