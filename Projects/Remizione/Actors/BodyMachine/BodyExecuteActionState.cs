using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// BodyExecuteActionState
    /// </summary>
    public sealed class BodyExecuteActionState : BodyAnimatedState
    {
        private bool animationFound;
        private bool eventDone;
        private static readonly string missText = TextRepository.GetValue("Misc.Miss");

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
                if (Owner.IsInAttackLane(target))
                    return true;
            }

            return false;
        }

        // ResolveProjectileAction
        private void ResolveProjectileAction(IAction action)
        {
            if (Target != null && Owner.AnimationPlayer.Frame?.SpawnPoint is Vector2 spawnPoint && spawnPoint != Vector2.Zero)
            {
                var projectile = Owner.Session.ObjectPools.Projectiles.Get();
                var pos = Owner.GetAnchoredPosition(spawnPoint);
                projectile.Throw(Owner, pos, action, Target);
            }
        }

        // ResolveProximityAction
        private void ResolveProximityAction(IAction action)
        {
            if (Target != null && CanInflictDamage(Target))
            {
                if (action.MissChance.Roll())
                    Owner.ShowFlyOff(missText, ColorPalette.MouseCursor.Tooltip);
                else
                    EffectDescriptor.Apply(action.EffectDescriptors, Owner, Target, EffectContext.Attack);

                //Owner.Session.InterruptAwaitingScript();
            }
        }

        // ResolveSelfAction
        private void ResolveSelfAction(IAction action)
        {
            EffectDescriptor.Apply(action.EffectDescriptors, Owner, null, EffectContext.Use);
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
        }

        // Target
        public GameThing? Target { get; set; }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (Action is IAction action && !eventDone && Owner.AnimationPlayer.Frame?.IsTrigger == true)
            {
                eventDone = true;

                if (action.SoundTrigger != null)
                    Owner.PlaySound(action.SoundTrigger);

                switch (action.ActionKind)
                {
                    // ProjectileAction
                    case ActionKind.Projectile:
                        ResolveProjectileAction(action);
                        break;

                    // ProximityAction
                    case ActionKind.Proximity:
                        ResolveProximityAction(action);
                        break;

                    // SelfAction
                    case ActionKind.Self:
                        ResolveSelfAction(action);
                        break;

                    default:
                        break;
                }

                action.Consume(Owner);

                return;
            }

            if (!animationFound || !Owner.AnimationPlayer.IsPlaying)
                Machine.ChangeState<BodyStandState>();
        }
    }
}