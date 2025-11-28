using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// UIHPBonus
    /// </summary>
    public sealed class UIHPBonus : GameObject
    {
        private int fullHearts;
        private bool hasHalfHeart;
        private readonly ImageSprite[] hearts;
        private int lastKnownValue;
        private Vector2 position;

        // Constructor
        public UIHPBonus(EngendroGame game)
            : base(game)
        {
            this.hearts = new ImageSprite[10];
            var pos = Screen.HUDArea.GetPoint(RectanglePoint.LeftTop, 18, 0);

            for (var i = 0; i < hearts.Length; i++)
            {
                hearts[i] = new(Game, Atlases.UI.HeartIconWithShadow)
                {
                    Scale = ScaleInfo.UIElement.Medium,
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
                hearts[i].Image = null;
            }

            if (fullHearts == 0 && hasHalfHeart)
            {
                hearts[0].Image = Atlases.UI.HeartHalfIconWithShadow;
            }
            else
            {
                for (int i = 0; i < fullHearts; i++)
                {
                    if (i < fullHearts)
                        hearts[i].Image = Atlases.UI.HeartIconWithShadow;

                    else if (i == fullHearts && hasHalfHeart)
                        hearts[i].Image = Atlases.UI.HeartHalfIconWithShadow;
                }
            }

            lastKnownValue = Amount;

            for (var i = 0; i < hearts.Length; i++)
            {
                hearts[i].Position = new(position.X + i * hearts[i].BoundingBox.Width, position.Y);
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
                if (hearts[i].Image == null)
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
