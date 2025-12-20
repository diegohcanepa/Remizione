using System;

namespace Adberration.Scripting
{
    // SetCounterCommand
    // Arguments: {Counter} to {Int32Range}
    internal sealed class SetCounterCommand : NonAwaitableCommand
    {
        // Constructor
        internal SetCounterCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 3)
        {
            AssertCounter(body.Clauses[0]);
            AssertKeyword(1, "to");
            Parser.ParseInt32Range(this, 2);
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (Session.ScriptEnvironment.GetCounter(Body.Clauses[0]) is Counter counter)
            {
                counter.Value = Parser.ParseInt32Range(this, 2).GetRandomValue(Random.Shared);
            }
        }
    }
}
