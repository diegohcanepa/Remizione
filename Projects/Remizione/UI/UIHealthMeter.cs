using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// UIHealthMeter
    /// </summary>
    public sealed class UIHealthMeter : GameObject
    {
        private Actor? actor;
        private int fullHearts;
        private bool hasHalfHeart;
        private readonly ImageSprite[] hearts;
        private int lastKnownMaxValue;
        private int lastKnownValue;
        private int totalHearts;

        // Constructor
        public UIHealthMeter(EngendroGame game)
            : base(game)
        {
            this.hearts = new ImageSprite[10];
            var pos = Screen.HUDArea.GetPoint(RectanglePoint.LeftTop, 16, 0);

            for (var i = 0; i < hearts.Length; i++)
            {
                hearts[i] = new(Game, Atlases.UI.HeartIcon)
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
            if (actor == null)
                return;

            fullHearts = actor.Health / 2;
            hasHalfHeart = actor.Health % 2 == 1;
            totalHearts = actor.MaxHealth / 2;

            for (int i = 0; i < totalHearts; i++)
            {
                if (i < fullHearts)
                    hearts[i].Image = Atlases.UI.HeartIcon;

                else if (i == fullHearts && hasHalfHeart)
                    hearts[i].Image = Atlases.UI.HeartHalfIcon;

                else
                    hearts[i].Image = Atlases.UI.HeartEmptyIcon;
            }

            lastKnownValue = actor.Health;
            lastKnownMaxValue = actor.MaxHealth;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (actor == null)
                return;

            Game.SpriteBatch.Begin(Game.Camera);
            for (var i = 0; i < totalHearts; i++)
            {
                hearts[i].Draw(gameTime);
            }
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (actor != null)
            {
                if (lastKnownValue != actor.Health || lastKnownMaxValue != actor.MaxHealth)
                    Invalidate();
            }
        }

        #endregion

        // Actor
        public Actor? Actor
        {
            get => actor;
            set
            {
                if (value != actor)
                {
                    actor = value;

                    if (actor == null)
                    {
                        lastKnownValue = int.MinValue;
                        lastKnownMaxValue = int.MinValue;
                    }

                    Invalidate();
                }
            }
        }
    }
}
