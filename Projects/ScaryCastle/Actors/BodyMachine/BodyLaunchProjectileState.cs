using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// BodyLaunchProjectileState
    /// </summary>
    public sealed class BodyLaunchProjectileState : BodyAnimatedState
    {
        private bool projectileLaunched;

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
            return ActionName;
        }

        #endregion

        // ActionName
        public string ActionName { get; set; } = string.Empty;

        // Enter
        public override void Enter()
        {
            base.Enter();
            projectileLaunched = ProjectileDescriptor == null;
        }

        // ProjectileDescriptor
        public ProjectileDescriptor? ProjectileDescriptor { get; set; }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (!projectileLaunched && Owner.AnimationPlayer.Frame?.IsEvent == true)
            {
                projectileLaunched = true;

                if (Owner.AnimationPlayer.Frame.SubArea is Rectangle subArea)
                {
                    if (ProjectileDescriptor != null)
                    {
                        var projectile = new Projectile(Owner.Session);
                        var pos = Owner.GetAnchoredPosition(23,19);
                        projectile.Launch(Owner, pos, Vector2.UnitX, ProjectileDescriptor);
                    }
                }

                return;
            }

            if (!Owner.AnimationPlayer.IsPlaying)
                Machine.ChangeState<BodyStandState>();
        }
    }
}