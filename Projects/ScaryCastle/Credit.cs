using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// Credit
    /// </summary>
    public sealed class Credit : GameObject
    {
        private readonly TextSprite textSprite;

        // Constructor
        public Credit(Credits credits, string text, Vector2 position, Color textColor, Vector2 textScale)
            : base(credits.Game)
        {
            this.textSprite = new TextSprite(Game, Fonts.Common)
            {
                Color = textColor,
                PivotOrigin = RectanglePoint.Top,
                Position = position,
                Scale = textScale,
                Text = text
            };

            textSprite.Velocity = new Vector2(0, Credits.Speed);
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);
            textSprite.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            textSprite.Update(gameTime);

            if (textSprite.BoundingBox.Bottom < 0)
                Done = true;
        }

        #endregion

        // BoundingBox
        public RectangleF BoundingBox => textSprite.BoundingBox;

        // Done
        public bool Done { get; private set; }

        // IsActiveInGameLoop
        public override bool IsActiveInGameLoop => !Done;
    }
}
