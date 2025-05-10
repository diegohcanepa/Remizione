using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ActorFatigueState
    /// </summary>
    public sealed class ActorFatigueState : ActorAnimatedState
    {
        // Constructor
        public ActorFatigueState(Actor owner)
            : base(owner, ActorStateNames.Fatigue, false)
        {
        }

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
            Owner.ShowMessage(Message.NoFaith, 2500);
        }

        // Update
        public override void Update(GameTime gameTime)
        {
        }
    }
}
