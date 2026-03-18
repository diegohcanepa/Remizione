using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// BodyCloseAttackState
    /// </summary>
    public sealed class BodyCloseAttackState : BodyAnimatedState
    {
        private bool damageTaken;

        // Constructor
        public BodyCloseAttackState()
            : base(string.Empty, false)
        {
        }

        #region Private members

        // CanInflictDamage
        private bool CanInflictDamage(GameThing target)
        {
            return target.CanBeHit && Owner.AnimationPlayer.Frame?.IsEvent == true;
        }

        // TryInflictDamage
        private bool TryInflictDamage(CombatIntent intent, GameThing target)
        {
            if (CanInflictDamage(target))
            {
                EffectDescriptor.Apply(intent.EffectDescriptors, Owner, target, EffectContext.Attack);
                Owner.Session.InterruptAwaitingScript();
                if (Owner.IsHostile(target))
                    Owner.Session.HUD.NotifyCombatIntent(intent);
                return true;
            }

            return false;
        }

        #endregion

        #region Protected members

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
            damageTaken = false;
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
            if (!damageTaken && Intent != null && Target != null)
                damageTaken = TryInflictDamage(Intent, Target);

            if (!Owner.AnimationPlayer.IsPlaying)
                Machine.ChangeState<BodyStandState>();
        }
    }
}