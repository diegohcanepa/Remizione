using Adberration.Scripting;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ScaryCastle.Scripting
{
    // AwaitEnemiesTurnCommand
    [ForceAwait]
    internal sealed class AwaitEnemiesTurnCommand : AwaitableCommand
    {
        private readonly List<Actor> actors = [];

        // Constructor
        internal AwaitEnemiesTurnCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0)
        {
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            if (Session.Room is not ProceduralRoom room)
                return;

            for (var i = 0; i < room.Children.Count; i++)
            {
                if (room.Children[i] is Actor actor && !actor.IsPlayer && actor.Patience <= 0)
                    actors.Add(actor);
            }

            if (actors.Count > 0)
                actors[0].React();
        }

        // OnExecutionCompleted
        protected override void OnExecutionCompleted()
        {
            actors.Clear();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (actors.Count == 0)
                return;

            if (!actors[0].IsPerformingAction && !actors[0].IsMoving)
            {
                actors.RemoveAt(0);
                if (actors.Count > 0)
                    actors[0].React();
            }
        }

        #endregion

        // IsAwaiting
        public override bool IsAwaiting => actors.Count > 0;
    }
}