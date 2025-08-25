using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// Lightning
    /// </summary>
    public sealed class Lightning : GameObject
    {
        //private readonly LightningBoltEffect effect = new LightningBoltEffect();
        private readonly GameSession session;
        private readonly AnimatedSprite sprite;

        // Constructor
        public Lightning(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            this.sprite = new(Game)
            {
                Atlas = Atlases.Environment,
                PivotOrigin = RectanglePoint.Bottom,
                Scale = ScaleInfo.UIElement.Small
            };

            var animation = sprite.AddAnimation("Default");
            animation.AddFrameSequence("Lightning", 80, 1, 3);
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (sprite.Player.IsPlaying)
            {
                Game.SpriteBatch.Begin(session.Camera);
                sprite.Draw(gameTime);
                Game.SpriteBatch.End();
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            sprite.Update(gameTime);
        }

        #endregion

        // Show
        public void Show(Vector2 position)
        {
            sprite.Position = position;
            sprite.Player.Play("Default", false);
            InputManager.DefaultPlayer.GamePad.Vibrate(200, .4f, .4f);
            session.Camera.Shake(TweenStyle.Linear, new Vector2(.5f), 66, 4);
            Sound.Play(SoundNames.Lightning);
        }
    }
}
