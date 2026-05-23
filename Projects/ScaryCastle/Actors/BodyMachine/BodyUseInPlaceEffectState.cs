using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// BodyUseInPlaceItemState
    /// </summary>
    public sealed class BodyUseInPlaceEffectState : BodyAnimatedState
    {
        private bool eventDone;

        // Constructor
        public BodyUseInPlaceEffectState()
            : base(string.Empty, false)
        {
        }

        #region Private members

        // CanInflictDamage
        private bool CanInflictDamage(GameThing target)
        {
            if (target.CanBeHit && Owner.AnimationPlayer.Frame?.IsEvent == true)
            {
                if (Owner.IsInAttackLane(target))
                {
                    if (Owner.AnimationPlayer.GetFrameSubArea().Intersects(target.RuntimeHotspot.BoundingRectangleF))
                        return true;
                }
            }

            return false;
        }

        // TryInflictDamage
        private bool TryInflictDamage(CombatIntent intent, GameThing target)
        {
            if (CanInflictDamage(target))
            {
                EffectDescriptor.Apply(intent.EffectDescriptors, Owner, target, EffectContext.Attack);
                Owner.Session.InterruptAwaitingScript();
                return true;
            }

            return false;
        }

        #endregion

        #region Protected members

        // GetAnimationName
        protected override string GetAnimationName()
        {
            return AnimationName;
        }

        #endregion

        // AnimationName
        public string AnimationName { get; set; } = string.Empty;

        // Enter
        public override void Enter()
        {
            base.Enter();
            eventDone = EffectType != ScaryCastle.InPlaceEffectType.None;
        }

        // Exit
        public override void Exit()
        {
            base.Exit();
            EffectType = InPlaceEffectType.None; 
            Target = null;
        }

        // EffectType
        public InPlaceEffectType EffectType { get; set; }

        // Target
        public GameThing? Target { get; set; }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (!eventDone && Owner.AnimationPlayer.Frame?.IsEvent == true)
            {
                eventDone = true;

                if (EffectType == InPlaceEffectType.Lightning)
                {
                    if (Target != null)
                    {
                        var lightning = new LightningInvocation(Target, item);
                    }
                }

                return;
            }

            if (!Owner.AnimationPlayer.IsPlaying)
                Machine.ChangeState<BodyStandState>();
        }
    }
}