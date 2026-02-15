using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// BodyAttackState
    /// </summary>
    public abstract class BodyAttackState : BodyAnimatedState
    {
        // Constructor
        protected BodyAttackState(string animationName)
            : base(animationName, false)
        {
        }

        #region Protected members

        // CanInflictDamage
        protected abstract bool CanInflictDamage(GameThing target);

        // DamageTaken
        protected bool DamageTaken { get; set; }

        #endregion

        // Enter
        public override void Enter()
        {
            base.Enter();
            DamageTaken = false;
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
                if (CanInflictDamage(Target))
                {
                    DamageTaken = true;
                    EffectDescriptor.Apply(Intent.EffectDescriptors, Owner, Target);
                    Owner.Session.InterruptAwaitingScript();
                }
            }

            if (!Owner.AnimationPlayer.IsPlaying)
                Machine.ChangeState<BodyStandState>();
        }
    }
}