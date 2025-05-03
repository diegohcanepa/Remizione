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

                    if (Owner.Target is Actor target)
                    {
                        var attackRoll = Owner.Stats.RollAttack();
                        var defenseRoll = target.Stats.GetDefense();

                        if (Owner.IsBehind(target) || attackRoll >= defenseRoll)
                            Owner.AttackSkill.EndUse(Owner.Target);
                        else
                        {
                            Owner.Session.CombatManager.Add(target);
                            target.ShowMessage("@Messages.Miss");
                        }
                    }
                    else
                        Owner.AttackSkill.EndUse(Owner.Target);
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