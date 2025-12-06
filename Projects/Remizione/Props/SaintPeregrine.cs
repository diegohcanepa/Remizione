using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// SaintPeregrine
    /// </summary>
    public sealed class SaintPeregrine : Prop
    {
        private readonly ImageSprite eyes;

        // Constructor
        public SaintPeregrine(GameSession session, string name)
            : base(session, name)
        {
            this.Atlas = Atlases.Environment;
            this.HitEffect = HitEffect.Shake;
            this.HitTestPolygon = TestPolygon.Hotspot;
            PropState = PropState.Locked;

            this.eyes = new ImageSprite(Game, Atlas?.GetImage($"{StaticName}Eyes"))
            {
            };

            eyes.Tweens.OpacityTween = FloatTween.Create(TweenStyle.Linear, 1, .7f, 70, -1);
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            if (PropState == PropState.Unlocked)
                eyes.Draw(gameTime);
        }

        /*
        // OnPropStateChanged
        protected override void OnPropStateChanged(PropState previousState)
        {
            AnimationPlayer.Play(PropState == PropState.Locked ? AnimationNames.Locked : AnimationNames.Unlocked, false);

            AllowInteraction = PropState == PropState.Locked;

            if (LoadState != LoadState.Loaded)
                return;
        }
        */

        // OnTransform
        protected override void OnTransform(TransformChange change)
        {
            base.OnTransform(change);
            eyes?.MatchTransform(Sprite);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            eyes.Update(gameTime);
        }

        #endregion
    }
}
