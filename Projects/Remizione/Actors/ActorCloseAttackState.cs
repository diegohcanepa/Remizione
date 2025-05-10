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
        protected override string GetAnimationName() => Owner.AttackSkill is Item skill ? skill.Name.ToString() : string.Empty;

        // Update
        public override void Update(GameTime gameTime)
        {
            if (Owner.Target == null || Owner.AttackSkill == null)
                return;

            if (!damageTaken && Owner.AnimationPlayer.Frame is SpriteFrame frame)
            {
                if (frame.Label == GameSettings.KeyFrame)
                {
                    damageTaken = true;
                    
                    var attackRoll = Owner.Stats.RollAttack(AttackRollStat.Strength, out bool criticalHit);
                    var hitType = criticalHit ? HitType.Critical : HitType.Default;

                    if (Owner.Target is Actor target)
                    {
                        var defenseRoll = criticalHit || target.IsTired ? 0 : target.Stats.GetDefense();

                        if (hitType != HitType.Critical)
                        {
                            // 50% miss chances
                            if (attackRoll < defenseRoll && DiceBag.Dice10.Roll() <= 5)
                            {
                                hitType = HitType.Glancing;
                                defenseRoll = 0;
                            }
                        }

                        if (attackRoll >= defenseRoll)
                            Owner.AttackSkill.EndUse(Owner.Target, hitType);
                        else
                        {
                            Owner.Session.CombatManager.Add(target);
                            target.ShowMessage(Message.Miss);
                        }
                    }
                    else
                        Owner.AttackSkill.EndUse(Owner.Target, hitType);
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