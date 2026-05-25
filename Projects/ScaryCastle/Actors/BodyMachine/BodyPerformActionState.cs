using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// BodyPerformActionState
    /// </summary>
    public sealed class BodyPerformActionState : BodyAnimatedState
    {
        private bool eventDone;

        // Constructor
        public BodyPerformActionState()
            : base(string.Empty, false)
        {
        }

        #region Private members

        // CanInflictDamage
        private bool CanInflictDamage(GameThing target)
        {
            if (target.CanBeHit && Owner.AnimationPlayer.Frame?.IsTrigger == true)
            {
                if (Owner.IsInAttackLane(target))
                {
                    if (Owner.AnimationPlayer.GetFrameSubArea().Intersects(target.RuntimeHotspot.BoundingRectangleF))
                        return true;
                }
            }

            return false;
        }

        // ResolveCloseAction
        private void ResolveCloseAction(IGameAction action)
        {
            if (Target != null && CanInflictDamage(Target))
            {
                EffectDescriptor.Apply(action.EffectDescriptors, Owner, Target, EffectContext.Attack);
                Owner.Session.InterruptAwaitingScript();
            }
        }

        // ResolveInPlaceAction
        private void ResolveInPlaceAction(IGameAction action)
        {
            if (action.InPlaceEffectType == InPlaceEffectType.None)
                return;

            if (action.InPlaceEffectType == InPlaceEffectType.Lightning)
            {
                if (Target != null && Owner.Room != null)
                {
                    var lightning = new LightningInvocation(action, Target);
                    Owner.Room.Children.Add(lightning);
                }
            }
        }

        // ResolveProjectileAction
        private void ResolveProjectileAction(IGameAction action)
        {
            if (action.Projectile == null)
                return;

            if (Owner.AnimationPlayer.Frame?.ActionPoint is Vector2 actionPoint && actionPoint != Vector2.Zero)
            {
                var projectile = new Projectile(Owner.Session);
                var pos = Owner.GetAnchoredPosition(actionPoint);
                projectile.Launch(Owner, pos, Owner.Direction == Adberration.FacingDirection.Right ? Vector2.UnitX : -Vector2.UnitX, action.Projectile);
            }
        }

        // ResolveSelfAction
        private void ResolveSelfAction(IGameAction action)
        {
            GameActionProcessor.Apply(action, Owner, null, EffectContext.Use);
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
            eventDone = false;

            if (Action?.SoundStart != null)
                Owner.PlaySound(Action.SoundStart);
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
            if (Action != null && !eventDone && Owner.AnimationPlayer.Frame?.IsTrigger == true)
            {
                eventDone = true;

                if (Action.SoundTrigger != null)
                    Owner.PlaySound(Action.SoundTrigger);

                switch (Action.UsageScope)
                {
                    // Close
                    case ItemUsageScope.Close:
                        ResolveCloseAction(Action);
                        break;

                    // InPlace
                    case ItemUsageScope.InPlace:
                        ResolveInPlaceAction(Action);
                        break;

                    // Projectile
                    case ItemUsageScope.Projectile:
                        ResolveProjectileAction(Action);
                        break;

                    // Self
                    case ItemUsageScope.Self:
                        ResolveSelfAction(Action);
                        break;

                    default:
                        break;
                }

                Action.Consume();

                return;
            }

            if (!Owner.AnimationPlayer.IsPlaying)
                Machine.ChangeState<BodyStandState>();
        }
    }
}