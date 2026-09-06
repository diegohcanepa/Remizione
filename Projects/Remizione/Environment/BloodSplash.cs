using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// BloodSplash
    /// </summary>
    public sealed class BloodSplash : GameObject
    {
        private readonly AnimatedSprite sprite;

        // Constructor
        public BloodSplash(Actor actor)
        {
            this.Actor = actor;

            sprite = new()
            {
                Atlas = Atlases.Environment,
                Scale = new(.7f)
            };

            if (actor.BodySize is BodySize.Small or BodySize.Medium)
                sprite.AddAnimation("Default").AddFrameSequence("BloodSplashLow", 100, 1, 6);
            else
                sprite.AddAnimation("Default").AddFrameSequence("BloodSplashHigh", 100, 1, 6);

            sprite.Color = actor.RemainsKind switch
            {
                RemainsKind.ToxicGuts => ColorPalette.BloodColor.ToxicGuts,
                _ => ColorPalette.BloodColor.Guts
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (sprite.Player.IsPlaying)
                sprite.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (sprite.Player.IsPlaying)
                sprite.Update(gameTime);
        }

        #endregion

        // Actor
        public Actor Actor { get; }

        // Show
        public void Show(Vector2 position)
        {
            sprite.Position = position;
            sprite.Player.Play("Default", false);
        }
    }
}
