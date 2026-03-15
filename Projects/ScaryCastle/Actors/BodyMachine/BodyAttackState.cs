using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// BodyAttackState
    /// </summary>
    public abstract class BodyAttackState : BodyAnimatedState
    {
        // Constructor
        protected BodyAttackState()
            : base(string.Empty, false)
        {
        }

        #region Private members

        // TryInflictDamage
        private bool TryInflictDamage(CombatIntent intent)
        {
            if (Owner.Room != null)
            {
                for (var i = 0; i < Owner.Room.Children.Count; i++)
                {
                    if (Owner.Room.Children[i] is GameThing thing && Owner.IsEnemy(thing))
                    {
                        if (CanInflictDamage(thing))
                        {
                            EffectDescriptor.Apply(intent.EffectDescriptors, Owner, thing);
                            //Owner.Session.InterruptAwaitingScript();
                            return true;
                        }
                    }
                }
            }

            return false;
        }

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
        protected abstract bool CanInflictDamage(GameThing target);

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
            if (!DamageTaken && Intent != null)
            {
                DamageTaken = Target == null ? TryInflictDamage(Intent) : TryInflictDamage(Intent, Target);
            }

            if (!Owner.AnimationPlayer.IsPlaying)
                Machine.ChangeState<BodyStandState>();
        }
    }
}