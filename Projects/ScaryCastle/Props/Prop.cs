using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// Prop
    /// </summary>
    public class Prop : GameThing, IThingDefinition
    {
        #region Private fields

        private readonly Vector2Tween bounceScaleTween = new();
        private readonly Sprite shadow;
        private readonly FloatTween xTween = new();

        #endregion

        #region Constructor

        // Constructor
        public Prop(GameSession session, string name)
            : base(session, name)
        {
            this.ApproachBehavior = ApproachBehavior.InFront;
            this.CanBeHit = false;
            this.Definition = PropDefinition.Definitions.Find(DeclaredName);
            this.HurtSound = Sound.Find(SoundNames.ImpactA);

            // Assign defaults from definition if available
            if (Definition != null)
            {
                this.DropMode = Definition.DropMode;
                this.DropChanceMultiplier = Definition.DropChanceMultiplier;
            }

            // Shadow
            this.shadow = new Sprite()
            {
                Opacity = ColorPalette.ShadowOpacity,
                PivotOrigin = RectanglePoint.Bottom,
            };
        }

        #endregion

        #region IThingDefinition

        ThingDefinition? IThingDefinition.Definition => this.Definition;

        #endregion

        #region Private members

        // InvalidateShadowImage
        private void InvalidateShadowImage()
        {
            shadow.RenderImage = Atlas?.FindImage(GetDefaultImageName() + "Shadow");
        }

        #endregion

        #region Protected members

        // BounceCore
        protected void BounceCore(float intensity, int bounceCount)
        {
            bounceScaleTween.Start(TweenStyle.QuadraticInOut, Scale, new Vector2(1f, intensity), 100, bounceCount);
            xTween.Start(TweenStyle.QuadraticInOut, X, X - 1, 40, 6);
        }

        // OnDrawShadow
        protected override void OnDrawShadow(GameTime gameTime)
        {
            if (shadow.IsEmpty)
                base.OnDrawShadow(gameTime);
            else
                shadow.Draw(gameTime);
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            InvalidateShadowImage();
        }

        // OnTakeDamage
        protected override void OnTakeDamage(GameThing attacker, int amount, DamageType damageType)
        {
            if (!IsDead && Definition?.DropTrigger == LootDropTrigger.OnImpact)
                DropLoot();
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

        #endregion

        // Bounce
        [ScriptMethod]
        public virtual void Bounce()
        {
            BounceCore(.95f, 2);
        }

        // Definition
        public PropDefinition? Definition { get; }

        // IsAmbientLightSource
        public bool IsAmbientLightSource { get; init; }

        // SkillChancePenalty
        [ScriptProperty]
        public int SkillChancePenalty { get; set; }

        // TestSkillChance
        public bool TestSkillChance(Actor actor, Item item)
        {
            var roll = DiceExpression.Dice100.Roll();
            var successChance = item.Definition.SkillChance - SkillChancePenalty;
            var success = roll <= successChance;

            item.ComputeUse();

            var text = TextRepository.GetValue(success ? Localization.GetValue(FloatingMessage.Success) : Localization.GetValue(FloatingMessage.Failed));

            actor.ShowFloatingText(text, success ? ColorPalette.Text.Green : ColorPalette.Text.Default);

            if (!success)
                Sound.Play(SoundNames.TestSkillFail);

            return success;
        }
    }
}
