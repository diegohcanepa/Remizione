namespace Adberration.Scripting
{
    // SetFlagCommand
    // Arguments: {Flag} to {Boolean}
    internal sealed class SetFlagCommand : NonAwaitableCommand
    {
        // Constructor
        internal SetFlagCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 3)
        {
            var flag = CheckFlag(body.Clauses[0]);
            flag.SetValueReferenceCount++;
            AssertKeyword(1, "to");
            Parser.ParseBoolean(this, 2);
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (Session.ScriptEnvironment.GetFlag(Body.Clauses[0]) is Flag flag)
            {
                flag.Value = Parser.ParseBoolean(this, 2);
            }
        }
    }
}
