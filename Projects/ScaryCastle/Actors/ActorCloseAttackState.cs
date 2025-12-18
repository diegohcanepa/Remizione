using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
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
        protected override string GetAnimationName()
        {
            return MetaItem?.Name ?? string.Empty;
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (MetaItem?.Effect.Damage == null)
                return;

            if (!damageTaken && Owner.Room != null && Owner.GetFrameSubArea() != RectangleF.Empty)
            {
                for (var i = 0; i < Owner.Room.CulledThings.Count; i++)
                {
                    // Skip owner
                    if (Owner.Room.CulledThings[i] is not GameThing target || target == Owner)
                        continue;

                    if (target is Prop && target == Owner.InteractiveTarget)
                    {
                        if (!Owner.Y.IsBetween(target.Position.Y - (target.BoundingBox.Height / 2), target.Position.Y))
                            return;
                    }

                    if (target.RuntimeHotspot.BoundingRectangleF.Intersects(Owner.GetFrameSubArea()))
                    {
                        damageTaken = true;
                        target.TakeDamage(Owner, MetaItem.Effect);
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