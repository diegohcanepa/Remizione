using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// Prop
    /// </summary>
    public class Prop : GameThing
    {
        private readonly Dictionary<PropState, Func<bool>?> handlers = [];
        private PropState propState;
        private readonly Vector2Tween bounceScaleTween = new();
        private readonly ImageSprite shadow;
        private readonly FloatTween xTween = new();

        #region Constructor

        // Constructor
        public Prop(GameSession session, string name)
            : base(session, name)
        {
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
        private void InvalidateShadowImage()
        {
            shadow.Image = Atlas?.FindImage(GetDefaultImageName() + "Shadow");
        }

        #endregion

        #region Protected members

        // InitializeState
        protected void InitializeState(PropState initialState)
        {
            propState = initialState;
            OnInitializeState(initialState);
        }

        // OnDrawShadow
        protected override void OnDrawShadow(GameTime gameTime)
        {
            if (shadow.IsEmpty)
                base.OnDrawShadow(gameTime);
            else
                shadow.Draw(gameTime);
        }

        // OnInitializeState
        protected virtual void OnInitializeState(PropState state)
        {
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            InvalidateShadowImage();
        }

        // OnPropStateChanged
        protected virtual void OnPropStateChanged()
        {
        }

        // OnTransform
        protected override void OnTransform(TransformChange change)
        {
            base.OnTransform(change);
            shadow?.MatchTransform(Sprite);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (bounceScaleTween.IsRunning)
            {
                bounceScaleTween.Update(gameTime);
                Scale = bounceScaleTween.CurrentValue;
            }

            if (xTween.IsRunning)
            {
                xTween.Update(gameTime);
                X = xTween.CurrentValue;
            }
        }

        // SetStateHandler
        protected void SetStateHandler(PropState s, Func<bool>? handler)
        {
            handlers[s] = handler;
        }

        #endregion

        // Bounce
        [ScriptMethod]
        public void Bounce()
        {
            bounceScaleTween.Start(TweenStyle.QuadraticInOut, Scale, new Vector2(1f, .95f), 100, 2);
            xTween.Start(TweenStyle.QuadraticInOut, X, X - 1, 40, 6);
        }

        // PropState
        [ScriptProperty]
        public PropState PropState
        {
            get => propState;
            set
            {
                if (propState == value)
                    return;

                // Try handler
                if (handlers.TryGetValue(value, out var handler))
                {
                    if (handler == null || handler())
                    {
                        propState = value;
                        OnPropStateChanged();
                    }
                }
            }
        }

        // SkillChancePenalty
        [ScriptProperty]
        public int SkillChancePenalty { get; set; }

        // TestSkillChance
        public bool TestSkillChance(Actor actor, Item item, PropState successState)
        {
            var roll = DiceExpression.Dice100.Roll();
            var successChance = item.SkillChance - SkillChancePenalty;
            var success = roll <= successChance;

            item.Use(actor);

            Session.HUD.Log.Show(LogVerb.Used, item.MetaItem);

            var text = TextRepository.GetValue(success ? "Misc.Success" : "Misc.Failed");

            actor.ShowFloatingText(text, success ? ColorPalette.Text.Green : ColorPalette.Text.Red);

            if (success)
            {
                if (successState != PropState.None)
                    PropState = successState;
            }
            else
            {
                Sound.Play(SoundNames.TestSkillFail);
            }

            return success;
        }
    }
}
