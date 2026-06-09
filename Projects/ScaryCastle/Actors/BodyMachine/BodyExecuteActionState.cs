using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// BodyExecuteActionState
    /// </summary>
    public sealed class BodyExecuteActionState : BodyAnimatedState
    {
        private bool animationFound;
        private bool eventDone;

        // Constructor
        public BodyExecuteActionState()
            : base(string.Empty, false)
        {
        }

        #region Private members

        // CanInflictDamage
        private bool CanInflictDamage(GameThing target)
        {
            if (target.CanBeHit() && Owner.AnimationPlayer.Frame?.IsTrigger == true)
            {
                //if (Owner.IsInAttackLane(target))
                {
                    //if (Owner.AnimationPlayer.GetFrameSubArea().Intersects(target.RuntimeHotspot.BoundingRectangleF))
                        return true;
                }
            }

            return false;
        }

        // ResolveInPlaceAction
        private void ResolveInPlaceAction(IAction action)
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
        private void ResolveProjectileAction(IAction action)
        {
            if (action.Projectile == null)
                return;

            if (Owner.AnimationPlayer.Frame?.SpawnPoint is Vector2 actionPoint && actionPoint != Vector2.Zero)
            {
                var projectile = Owner.Session.ObjectPools.Projectiles.Get();
                var pos = Owner.GetAnchoredPosition(actionPoint);
                projectile.Launch(Owner, pos, Owner.Direction == Adberration.FacingDirection.Right ? Vector2.UnitX : -Vector2.UnitX, action.Projectile);
            }
        }

        // ResolveProximityAction
        private void ResolveProximityAction(IAction action)
        {
            if (Target != null && CanInflictDamage(Target))
            {
                EffectDescriptor.Apply(action.EffectDescriptors, Owner, Target, EffectContext.Attack);
                Owner.Session.InterruptAwaitingScript();
            }
        }

        // ResolveSelfAction
        private void ResolveSelfAction(IAction action)
        {
            ActionProcessor.Apply(action, Owner, null, EffectContext.Use);
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
        public IAction? Action { get; set; }

        // Enter
        public override void Enter()
        {
            base.Enter();

            animationFound = Owner.ContainsAnimation(GetAnimationName());
            eventDone = !animationFound;

            if (Action?.SoundStart != null)
                Owner.PlaySound(Action.SoundStart);
        }

        // Exit
        public override void Exit()
        {
            base.Exit();
            Action = null;
            Target = null;
            Owner.Session.ProcessTurn();
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

                switch (Action.ActionKind)
                {
                    // InPlaceAction
                    case ActionKind.InPlace:
                        ResolveInPlaceAction(Action);
                        break;

                    // ProjectileAction
                    case ActionKind.Projectile:
                        ResolveProjectileAction(Action);
                        break;

                    // ProximityAction
                    case ActionKind.Proximity:
                        ResolveProximityAction(Action);
                        break;

                    // SelfAction
                    case ActionKind.Self:
                        ResolveSelfAction(Action);
                        break;

                    default:
                        break;
                }

                Action.Consume(Owner);

                return;
            }

            if (!animationFound || !Owner.AnimationPlayer.IsPlaying)
                Machine.ChangeState<BodyStandState>();
        }
    }
}