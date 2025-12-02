using Microsoft.Xna.Framework;
using System;

namespace Adberration.Scripting
{
    // AwaitCommand
    // Syntax: {Int32Range}
    [ForceAwait]
    internal sealed class AwaitCommand : AwaitableCommand
    {
        private int duration;

        // Constructor
        internal AwaitCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            Parser.ParseInt32Range(this, 0);
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            var range = Parser.ParseInt32Range(this, 0);
            duration = range.RandomValue(Random.Shared);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            duration -= gameTime.ElapsedGameTime.Milliseconds;
        }

        #endregion

        // IsAwaiting
        public override bool IsAwaiting => duration > 0;
    }
}
