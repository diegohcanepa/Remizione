using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ActorCloseAttackState
    /// </summary>
    public sealed class ActorCloseAttackState : ActorAnimatedState
    {
        private bool damageTaken;

        // Constructor
        public ActorCloseAttackState(Actor owner)
            : base(owner, ActorStateNames.CloseAttack, false)
        {
        }

        #region Protected members

        // GetAnimationName
        protected override string GetAnimationName() => MetaItem?.Name ?? string.Empty;

        // Update
        public override void Update(GameTime gameTime)
        {
            if (MetaItem?.Damage == null)
                return;

            if (!damageTaken && Owner.Room != null && Owner.GetFrameSubArea() != RectangleF.Empty)
            {
                for (var i = 0; i < Owner.Room.CulledThings.Count; i++)
                {
                    var target = Owner.Room.CulledThings[i] as GameThing;

                    // Skip owner
                    if (target == null || target == Owner)
                        continue;

                    if (target is IsometricProp && target == Owner.InteractiveTarget)
                    {
                        if (!Owner.Y.IsBetween(target.Position.Y - target.BoundingBox.Height / 2, target.Position.Y))
                            return;
                    }

                    if (target.RuntimeHotspot.BoundingRectangleF.Intersects(Owner.GetFrameSubArea()))
                    {
                        damageTaken = true;
                        var damageAmount = MetaItem.Damage.Roll();
                        target.TakeDamage(Owner, damageAmount, MetaItem.DamageKind, MetaItem.DamageIntensity, false, MetaItem.Knockback, MetaItem.ImpactWord);
                    }
                }
            }
        }

        #endregion

        // CheckTransitions
        public override string? CheckTransitions()
        {
            if (!Owner.AnimationPlayer.IsPlaying)
                return ActorStateNames.Stand;
            else
                return base.CheckTransitions();
        }

        // Enter
        public override void Enter()
        {
            base.Enter();
            damageTaken = false;
            //Owner.FaceToTarget();
        }

        // MetaItem
        public MetaItem? MetaItem { get; set; }
    }
}