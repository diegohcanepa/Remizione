using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// Prop
    /// </summary>
    public class Prop : GameThing, IThingDefinition
    {
        #region Private fields

        private readonly Vector2Tween bounceScaleTween = new();
        private List<AtlasImage>? remainsPieces;
        private readonly FloatTween xTween = new();

        #endregion

        #region Constructor

        // Constructor
        public Prop(GameSession session, string name)
            : base(session, name)
        {
            this.IgnoreKnockback = true;
            this.Verb = Verb.Use;
            this.IsHittable = false;
            this.Definition = PropDefinition.Container.Find(DeclaredName);

            // Shadow
            this.Shadow = new Sprite()
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
            Shadow.RenderImage = Atlas?.FindImage(GetDefaultImageName() + "Shadow");
        }

        #endregion

        #region Protected members

        // BounceCore
        protected void BounceCore(float intensity, int bounceCount)
        {
            bounceScaleTween.Start(TweenStyle.QuadraticInOut, Scale, new Vector2(1f, intensity), 100, bounceCount);
            xTween.Start(TweenStyle.QuadraticInOut, X, X - 1, 40, 6);
        }

        // MatchShadowTransform
        protected bool MatchShadowTransform { get; set; } = true;

        // OnAtlasChanged
        protected override void OnAtlasChanged()
        {
            base.OnAtlasChanged();

            remainsPieces?.Clear();

            if (Atlas == null)
                return;

            var index = 1;
            while (true)
            {
                if (Atlas.FindImage($"{DeclaredName}Remains{index}") is AtlasImage image)
                {
                    remainsPieces ??= [];
                    remainsPieces.Add(image);
                }
                else
                {
                    break;
                }

                index++;
            }
        }

        // OnDeath
        protected override void OnDeath()
        {
            if (Room != null && remainsPieces?.Count > 0)
            {
                SpawnRemains(Room);
                Unparent();
            }
        }

        // OnDrawShadow
        protected override void OnDrawShadow(GameTime gameTime)
        {
            if (Shadow.IsEmpty)
                base.OnDrawShadow(gameTime);
            else
                Shadow.Draw(gameTime);
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

            if (MatchShadowTransform)
                Shadow?.MatchTransform(Sprite);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            Shadow.Update(gameTime);

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

        // Shadow
        protected Sprite Shadow { get; }

        #endregion

        // Bounce
        [ScriptMethod]
        public virtual void Bounce()
        {
            BounceCore(.95f, 2);
        }

        // CanInteract
        public override bool CanInteract()
        {
            return !bounceScaleTween.IsRunning && base.CanInteract();
        }

        // Definition
        public PropDefinition? Definition { get; }

        // GetThrowableImageName
        public virtual string GetThrowableImageName()
        {
            return DeclaredName;
        }

        // IsAmbientLight
        public bool IsAmbientLight => AttachedLight != null && AttachedLight.Ambient;

        // IsLiftable
        [ScriptProperty]
        public bool IsLiftable { get; set; }

        // SkillChancePenalty
        [ScriptProperty]
        public int SkillChancePenalty { get; set; }

        // SpawnRemains
        public void SpawnRemains(GameRoom room)
        {
            if (remainsPieces?.Count > 0)
            {
                var remains = new Remains(Session, string.Empty, Vector2.One, remainsPieces, remainsPieces.Count, true)
                {
                    Position = Position
                };

                room.Children.Add(remains);
            }
        }

        // TestSkillChance
        public bool TestSkillChance(Actor actor, Item item)
        {
            var roll = DiceExpression.Dice100.Roll();
            var successChance = item.Definition.SkillChance - SkillChancePenalty;
            var success = roll <= successChance;

            item.Consume(actor);

            var text = success ? Localization.GetValue(FloatingMessage.Success) : Localization.GetValue(FloatingMessage.Failed);

            actor.ShowFlyOff(text, success ? ColorPalette.Text.Green : ColorPalette.Text.Terra);

            if (!success)
                Sound.Play(SoundNames.TestSkillFail);

            return success;
        }
    }
}
