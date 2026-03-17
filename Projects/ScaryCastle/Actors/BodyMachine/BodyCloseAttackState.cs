using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// BodyCloseAttackState
    /// </summary>
    public class BodyCloseAttackState : BodyAnimatedState
    {
        // Constructor
        public BodyCloseAttackState()
            : base(string.Empty, false)
        {
        }

        #region Private members

        // TryInflictDamage
        private bool TryInflictDamage(CombatIntent intent, GameThing target)
        {
            if (CanInflictDamage(target))
            {
                EffectDescriptor.Apply(intent.EffectDescriptors, Owner, target);
                Owner.Session.InterruptAwaitingScript();
                return true;
            }

            return false;
        }

        #endregion

        #region Protected members

        // CanInflictDamage
        protected bool CanInflictDamage(GameThing target)
        {
            return target.CanBeHit && Owner.AnimationPlayer.Frame?.IsEvent == true;
        }

        // DamageTaken
        protected bool DamageTaken { get; set; }

        // GetAnimationName
        protected override string GetAnimationName()
        {
            return Intent?.Name ?? string.Empty;
        }

        #endregion

        // Enter
        public override void Enter()
        {
            base.Enter();
            DamageTaken = false;
            if (Intent?.Sound is { } sound)
                Owner.PlaySound(sound);
        }

        // Intent
        public CombatIntent? Intent { get; set; }

        // Target
        public GameThing? Target { get; set; }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (!DamageTaken && Intent != null && Target != null)
            {
                DamageTaken = TryInflictDamage(Intent, Target);
            }

            if (!Owner.AnimationPlayer.IsPlaying)
                Machine.ChangeState<BodyStandState>();
        }
    }
}