using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// BodyLaunchProjectileState
    /// </summary>
    public sealed class BodyLaunchProjectileState : BodyAnimatedState
    {
        private bool eventDone;

        // Constructor
        public BodyLaunchProjectileState()
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
            eventDone = Action?.Projectile == null;
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (!eventDone && Owner.AnimationPlayer.Frame?.IsEvent == true)
            {
                eventDone = true;

                if (Owner.AnimationPlayer.Frame.ActionPoint != Vector2.Zero)
                {
                    if (Action?.Projectile != null)
                    {
                        var projectile = new Projectile(Owner.Session);
                        var pos = Owner.GetAnchoredPosition(Owner.AnimationPlayer.Frame.ActionPoint);
                        projectile.Launch(Owner, pos, Owner.Direction == Adberration.FacingDirection.Right ? Vector2.UnitX : -Vector2.UnitX, Action.Projectile);
                        if (Action.Sound != null)
                            Owner.PlaySound(Action.Sound);
                    }
                }

                return;
            }

            if (!Owner.AnimationPlayer.IsPlaying)
                Machine.ChangeState<BodyStandState>();
        }
    }
}