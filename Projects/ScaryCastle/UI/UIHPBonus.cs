using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// UIHPBonus
    /// </summary>
    public sealed class UIHPBonus : GameObject
    {
        private int fullHearts;
        private bool hasHalfHeart;
        private readonly Sprite[] hearts;
        private int lastKnownValue;
        private Vector2 position;

        // Constructor
        public UIHPBonus(EngendroGame game)
            : base(game)
        {
            this.hearts = new Sprite[10];
            var pos = Screen.HUDArea.GetPoint(RectanglePoint.LeftTop, 18, 0);

            for (var i = 0; i < hearts.Length; i++)
            {
                hearts[i] = new(Game, Atlases.UI.HeartFull)
                {
                    Scale = ScaleInfo.UIElement.Large,
                    Position = pos
                };

                pos.X += hearts[i].BoundingBox.Width;
            }
        }

        #region Private members

        // Invalidate
        private void Invalidate()
        {
            fullHearts = Amount / 2;
            hasHalfHeart = Amount % 2 == 1;

            for (var i = 0; i < hearts.Length; i++)
            {
                hearts[i].RenderImage = null;
            }

            if (fullHearts == 0 && hasHalfHeart)
            {
                hearts[0].RenderImage = Atlases.UI.HeartHalf;
            }
            else
            {
                for (int i = 0; i < fullHearts; i++)
                {
                    if (i < fullHearts)
                        hearts[i].RenderImage = Atlases.UI.HeartFull;

                    else if (i == fullHearts && hasHalfHeart)
                        hearts[i].RenderImage = Atlases.UI.HeartHalf;
                }
            }

            lastKnownValue = Amount;

            for (var i = 0; i < hearts.Length; i++)
            {
                hearts[i].Position = new(position.X + (i * hearts[i].BoundingBox.Width), position.Y);
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (Amount <= 0)
                return;

            Game.SpriteBatch.Begin(Game.Camera);
            for (var i = 0; i < hearts.Length; i++)
            {
                if (hearts[i].RenderImage == null)
                    break;

                hearts[i].Draw(gameTime);
            }
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (Amount > 0)
            {
                if (lastKnownValue != Amount)
                    Invalidate();
            }
        }

        #endregion

        // Amount
        public int Amount
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;

                    if (field <= 0)
                        lastKnownValue = int.MinValue;

                    Invalidate();
                }
            }
        }

        // Position
        public Vector2 Position
        {
            get => position;
            set
            {
                if (value != position)
                {
                    position = value;
                    Invalidate();
                }
            }
        }
    }
}
