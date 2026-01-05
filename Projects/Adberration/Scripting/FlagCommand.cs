namespace Adberration.Scripting
{
    // FlagCommand
    // Arguments: {Name} = {Boolean} [#transient]
    [ScriptStatement(CodingContext.Declaration)]
    internal sealed class FlagCommand : NonAwaitableCommand
    {
        // Constructor
        internal FlagCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 3, TransientArg)
        {
            Parser.ParseName(this, 0);
            AssertKeyword(1, "=");
            Session.ScriptEnvironment.DeclareFlag(body.Clauses[0], Parser.ParseBoolean(this, 2), !HasArg(TransientArg));
        }
    }
}
