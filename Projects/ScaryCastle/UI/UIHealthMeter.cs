using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// UIHealthMeter
    /// </summary>
    public sealed class UIHealthMeter : GameObject
    {
        #region Private fields

        private int fullHearts;
        private bool hasHalfHeart;
        private readonly ImageSprite[] hearts;
        private int lastKnownMaxValue;
        private int lastKnownValue;
        private int totalHearts;

        #endregion

        #region Constructor

        // Constructor
        public UIHealthMeter(EngendroGame game)
            : base(game)
        {
            this.hearts = new ImageSprite[10];
            var pos = Screen.HUDArea.GetPoint(RectanglePoint.LeftTop, 2, 0);

            for (var i = 0; i < hearts.Length; i++)
            {
                hearts[i] = new(Game, Atlases.UI.HeartFull)
                {
                    Position = pos
                };

                pos.X += hearts[i].BoundingBox.Width + .5f;
            }
        }

        #endregion

        #region Private members

        // Refresh
        private void Refresh()
        {
            if (Actor == null)
                return;

            fullHearts = Actor.HP / 2;
            hasHalfHeart = Actor.HP % 2 == 1;
            totalHearts = Actor.MaxHP / 2;

            for (int i = 0; i < totalHearts; i++)
            {
                if (i < fullHearts)
                    hearts[i].Image = Atlases.UI.HeartFull;

                else if (i == fullHearts && hasHalfHeart)
                    hearts[i].Image = Atlases.UI.HeartHalf;

                else
                    hearts[i].Image = Atlases.UI.HeartEmpty;
            }

            lastKnownValue = Actor.HP;
            lastKnownMaxValue = Actor.MaxHP;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (Actor == null)
                return;

            for (var i = 0; i < totalHearts; i++)
            {
                hearts[i].Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (Actor != null)
            {
                if (lastKnownValue != Actor.HP || lastKnownMaxValue != Actor.MaxHP)
                    Refresh();
            }
        }

        #endregion

        // Actor
        public Actor? Actor
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;

                    if (field == null)
                    {
                        lastKnownValue = int.MinValue;
                        lastKnownMaxValue = int.MinValue;
                    }

                    Refresh();
                }
            }
        }
    }
}
