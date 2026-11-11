namespace Adberration.Scripting
{
    // IncrementCounterCommand
    // Arguments: {Counter} by {Value}
    internal sealed class IncrementCounterCommand : NonAwaitableCommand
    {
        // Constructor
        internal IncrementCounterCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 3)
        {
            AssertCounter(body.Clauses[0]);
            AssertKeyword(1, "by");
            Parser.ParseInt32(this, 2);
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (Session.ScriptEnvironment.FindCounter(Body.Clauses[0]) is Counter counter)
            {
                counter.Value += Parser.ParseInt32(this, 2);
            }
        }
    }
}
