using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ActorFatigueState
    /// </summary>
    public sealed class ActorFatigueState : ActorState
    {
        private readonly FloatTween cooldown = new();

        // Constructor
        public ActorFatigueState(Actor owner)
            : base(owner, ActorStateNames.Fatigue, ActorStateSettings.LoopAnimation)
        {
        }

        // CheckTransitions
        public override string? CheckTransitions()
        {
            if (Owner.IsPlayer && !cooldown.IsRunning)
                return ActorStateNames.Stand;
            else
                return base.CheckTransitions();
        }

        // Enter
        public override void Enter()
        {
            base.Enter();
            cooldown.Start(TweenStyle.Linear, Owner.Stamina, Owner.MaxStamina, Owner.Stats.FatigueRecoveyPenalty);
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            cooldown.Update(gameTime);
            Owner.Stamina = (int)cooldown.CurrentValue;
        }
    }
}
