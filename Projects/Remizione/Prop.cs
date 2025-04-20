using Engendro;
using EngendroAdventure.Scripting;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// Prop
    /// </summary>
    public class Prop : GameThing
    {
        private bool isRevealBoxDirty;
        private RectangleF revealBox;
        private readonly FloatTween revealTween = new();
        private readonly ImageSprite shadow;

        #region Protected members

        // Constructor
        public Prop(GameSession session, string name)
            : base(session, name)
        {
            this.Atlas = Atlases.Environment;

            // Shadow
            this.shadow = new ImageSprite(session.Game)
            {
                Opacity = ColorPalette.ShadowOpacity,
                PivotOrigin = RectanglePoint.Bottom,
            };

        }

        #endregion

        #region Private members

        // InvalidateShadowImage
        private void InvalidateShadowImage() => shadow.Image = Atlas?.GetImage(GetDefaultImageName() + "Shadow");

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

        // IsProp
        protected override bool IsProp => true;

        // OnDrawShadow
        protected override void OnDrawShadow(GameTime gameTime) => shadow.Draw(gameTime);

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            InvalidateShadowImage();
        }

        // OnTransform
        protected override void OnTransform(TransformChange change)
        {
            base.OnTransform(change);

            shadow?.MatchTransform(Sprite);

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
