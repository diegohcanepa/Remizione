using Engendro;
using Engendro.Audio;
using EngendroAdventure.Scripting;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// IsometricProp
    /// </summary>
    public class IsometricProp : Prop
    {
        #region Private fields

        private bool isRevealBoxDirty;
        private RectangleF revealBox;
        private readonly FloatTween revealTween = new();

        #endregion

        #region Constructor

        // Constructor
        public IsometricProp(GameSession session, string name)
            : base(session, name)
        {
            this.Atlas = Atlases.Environment;
            this.HitTestSource = HitTestSource.Collider;
        }

        #endregion

        #region Private members

        // UpdateOpacityFactor
        private void UpdateOpacityFactor(GameTime gameTime)
        {
            const int tweenDuration = 200;

            if (Session.Player != null && RevealBox.Contains(Session.Player.Position))
            {
                if (Sprite.OpacityFactor == GameSettings.PropRevealOpacity)
                    return;

                if (!revealTween.IsRunning || revealTween.EndValue == 1)
                    revealTween.Start(TweenStyle.Linear, Sprite.OpacityFactor, .5f, tweenDuration);
            }
            else
            {
                if (Sprite.OpacityFactor == 1)
                    return;

                if (!revealTween.IsRunning || revealTween.EndValue == GameSettings.PropRevealOpacity)
                    revealTween.Start(TweenStyle.Linear, Sprite.OpacityFactor, 1, tweenDuration);
            }

            if (revealTween.IsRunning)
            {
                revealTween.Update(gameTime);
                Sprite.OpacityFactor = revealTween.CurrentValue;
            }
            else
                Sprite.OpacityFactor = 1;
        }

        #endregion

        #region Protected members

        // OnTransform
        protected override void OnTransform(TransformChange change)
        {
            base.OnTransform(change);
            isRevealBoxDirty = true;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (!RevealArea.IsEmpty)
                UpdateOpacityFactor(gameTime);
        }

        #endregion

        // RevealArea
        [ScriptProperty]
        public Rectangle RevealArea { get; set; }

        // RevealBox
        public RectangleF RevealBox
        {
            get
            {
                if (isRevealBoxDirty)
                {
                    revealBox = RevealArea.IsEmpty ? RectangleF.Empty : this.GetAbsoluteBounds(RevealArea);
                    isRevealBoxDirty = false;
                }

                return revealBox;
            }
        }
    }
}
