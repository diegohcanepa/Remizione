namespace Adberration.Scripting
{
    // CounterCommand
    // Arguments: {Name} = {Integer}
    internal sealed class CounterCommand : NonAwaitableCommand
    {
        // Constructor
        internal CounterCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 3, TransientArg)
        {
            Parser.ParseName(this, 0);
            AssertKeyword(1, "=");
            Session.ScriptEnvironment.DeclareCounter(body.Clauses[0], Parser.ParseInt32(this, 2), !HasArg(TransientArg));
        }
    }
}
