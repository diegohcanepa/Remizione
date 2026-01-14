using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// UISentence
    /// </summary>
    public sealed class UISentence : GameObject
    {
        private readonly TextSprite sentence;
        private readonly GameSession session;
        private GameThing? target;

        // Constructor
        public UISentence(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            this.sentence = new(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.OrangeLight,
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.Area.GetPoint(RectanglePoint.Bottom, 0, -6),
                Scale = ScaleInfo.Text.ExtraGiant
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (session.IsCurrentScene && target != null)
            {
                Game.SpriteBatch.Begin(Game.Camera);
                sentence.Draw(gameTime);
                Game.SpriteBatch.End();
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (session.IsCurrentScene && session.Player?.InteractiveTarget is GameThing currentTarget)
            {
                if (currentTarget != target)
                {
                    target = currentTarget;
                    sentence.Text = currentTarget.GetInteractPrompt() ?? currentTarget.LocalizedDisplayName;
                }
            }
            else
            {
                sentence.Text = null;
                target = null;
            }

            sentence.Update(gameTime);
        }

        #endregion
    }
}
