using Engendro;
using EngendroAdventure.Scripting;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ActorCreateState
    /// </summary>
    public sealed class ActorCreateState : ActorState
    {
        private Script? routine;

        // Constructor
        public ActorCreateState(Actor owner)
            : base(owner, ActorStateNames.CreateItem)
        {
        }

        // CheckTransitions
        public override string? CheckTransitions()
        {
            if (routine == null || Owner.Session.IsAwaitingScript(routine))
                return ActorStateNames.Stand;
            else
                return base.CheckTransitions();
        }

        // Enter
        public override void Enter()
        {
            base.Enter();
            routine = Item?.MetaItem.CreationRoutine;
            if (routine != null)
            {
                Item?.Use();
                Owner.Session.AwaitScript(routine);
            }
        }

        // Exit
        public override void Exit()
        {
            base.Exit();
            Item = null;
        }

        // Item
        public Item? Item { get; set; }
    }
}
