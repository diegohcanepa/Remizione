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
        protected override string GetAnimationName() => Owner.GetAttackItem() is Item attackItem ? attackItem.Name.ToString() : string.Empty;

        // Update
        public override void Update(GameTime gameTime)
        {
            if (Owner.Target == null || Owner.GetAttackItem() is not Item attackItem)
                return;

            if (!damageTaken && Owner.AnimationPlayer.Frame is SpriteFrame frame)
            {
                if (frame.IsEvent)
                {
                    damageTaken = true;
                    
                    var attackRoll = Owner.Stats.RollAttack(AttackRollStat.Strength, 0, out bool criticalHit);
                    var hitType = criticalHit ? HitType.Critical : HitType.Default;

                    if (Owner.Target is Actor target)
                    {
                        var defenseRoll = criticalHit || target.IsTired ? 0 : target.Stats.GetDefense();

                        if (hitType != HitType.Critical)
                        {
                            // 50% miss chances
                            if (attackRoll < defenseRoll && DiceExpression.Dice10.Roll() <= 5)
                            {
                                hitType = HitType.Glancing;
                                defenseRoll = 0;
                            }
                        }

                        if (attackRoll >= defenseRoll)
                        {
                            attackItem.ApplyDamage(Owner.Target, hitType);
                        }
                        else
                        {
                            target.ShowMessage(Message.Miss);
                        }
                    }
                    else
                        attackItem.ApplyDamage(Owner.Target, hitType);
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
            Owner.FaceToTarget();
        }
    }
}