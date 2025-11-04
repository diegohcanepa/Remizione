using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// Prop
    /// </summary>
    public class Prop : GameThing
    {
        private int propAmount;
        private PropState propState;
        private readonly ImageSprite shadow;

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
        private void InvalidateShadowImage() => shadow.Image = Atlas?.GetImage(GetDefaultImageName() + "Shadow");

        #endregion

        #region Protected members

        // OnDrawShadow
        protected override void OnDrawShadow(GameTime gameTime) => shadow.Draw(gameTime);

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            InvalidateShadowImage();
        }

        // OnPropAmountChanged
        protected virtual void OnPropAmountChanged()
        {
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

        #endregion

        // PropAmount
        [ScriptProperty]
        public int PropAmount
        {
            get => propAmount;
            set
            {
                if (value != propAmount)
                {
                    propAmount = value;
                    OnPropAmountChanged();
                }
            }
        }

        // PropState
        [ScriptProperty]
        public PropState PropState
        {
            get => propState;
            set
            {
                if (value != PropState)
                {
                    propState = value;
                    OnPropStateChanged();
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

            Session.HUD.Log.Show(LogVerb.Lost, item.MetaItem);

            var text = TextRepository.GetValue(success ? "Misc.Success" : "Misc.Failed");

            actor.ShowFloatingText(text, success ? ColorPalette.Text.Green : ColorPalette.Text.Red);

            if (success)
            {
                if (successState != PropState.None)
                    PropState = successState;
            }
            else
                Sound.Play(SoundNames.TestSkillFail);

            return success;
        }
    }
}
