using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// BloodSplash
    /// </summary>
    public sealed class BloodSplash : GameObject
    {
        private AnimatedSprite? sprite;
        private readonly AnimatedSprite[] sprites;

        // Constructor
        public BloodSplash(EngendroGame game, Actor actor)
            : base(game)
        {
            this.Actor = actor;

            sprites = new AnimatedSprite[2];

            sprites[0] = new AnimatedSprite(game) { Atlas = Atlases.Environment, Opacity = .5f, Scale = new(.7f) };
            var animation = sprites[0].AddAnimation("Default");
            animation.AddFrameSequence("BloodSplashLow", 100, 1, 6);

            sprites[1] = new AnimatedSprite(game) { Atlas = Atlases.Environment, Opacity = .5f, Scale = new(.7f) };
            animation = sprites[1].AddAnimation("Default");
            animation.AddFrameSequence("BloodSplashHigh", 100, 1, 6);
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            sprite?.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (sprite != null)
            {
                if (sprite.Player.IsPlaying)
                    sprite?.Update(gameTime);
                else
                    sprite = null;
            }
        }

        #endregion

        // Actor
        public Actor Actor { get; }

        // Show
        public void Show(ActorSize size, Vector2 position)
        {
            sprite = size == ActorSize.Small ? sprites[0] : sprites[1];
            sprite.Position = position;
            sprite.Player.Play("Default", false);
        }
    }
}
