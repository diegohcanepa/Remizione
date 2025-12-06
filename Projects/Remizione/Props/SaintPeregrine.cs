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
        private int unlockCooldown;

        // Constructor
        public SaintPeregrine(GameSession session, string name)
            : base(session, name)
        {
            this.Atlas = Atlases.Environment;
            this.HighlightInteraction = false;
            this.HitEffect = HitEffect.Shake;
            this.HitTestPolygon = TestPolygon.Hotspot;
            this.PropState = PropState.Locked;

            this.eyes = new ImageSprite(Game, Atlas?.GetImage($"{StaticName}Eyes"));

            eyes.Tweens.OpacityTween = FloatTween.Create(TweenStyle.Linear, 1, .7f, 70, -1);

            SetStateHandler(PropState.Locked, Lock);
            SetStateHandler(PropState.Unlocked, Unlock);
            
            InitializeState(PropState.Locked);
        }

        #region Private members

        // Lock
        private bool Lock()
        {
            AnimationPlayer.Play(AnimationNames.Locked, false);
            return true;
        }

        // Unlock
        private bool Unlock()
        {
            unlockCooldown = 500;
            return true;
        }

        #endregion


        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            if (PropState == PropState.Unlocked)
                eyes.Draw(gameTime);
        }

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

            if (unlockCooldown > 0)
            {
                unlockCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                if (unlockCooldown <= 0)
                {
                    unlockCooldown = 0;
                    PlaySound(SoundNames.SaintPeregrineArm);
                    AnimationPlayer.Play(AnimationNames.Unlocked, false);
                }
            }

        }

        #endregion
    }
}
