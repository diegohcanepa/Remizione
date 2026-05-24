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
            return Action?.AnimationName ?? string.Empty;
        }

        #endregion

        // Action
        public IGameAction? Action { get; set; }

        // Enter
        public override void Enter()
        {
            base.Enter();
            eventDone = Action?.InPlaceEffectType != InPlaceEffectType.None;
        }

        // Exit
        public override void Exit()
        {
            base.Exit();
            Action = null;
            Target = null;
        }

        // Target
        public GameThing? Target { get; set; }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (!eventDone && Owner.AnimationPlayer.Frame?.IsEvent == true)
            {
                eventDone = true;

                if (Action != null)
                    Owner.PerformAction(Action, Target);

                return;
            }

            if (!Owner.AnimationPlayer.IsPlaying)
                Machine.ChangeState<BodyStandState>();
        }
    }
}