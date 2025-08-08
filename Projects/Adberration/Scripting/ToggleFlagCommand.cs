namespace Adberration.Scripting
{
    // ToggleFlagCommand
    // Arguments: {Flag}
    internal sealed class ToggleFlagCommand : NonAwaitableCommand
    {
        // Constructor
        internal ToggleFlagCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            var flag = CheckFlag(body.Clauses[0]);
            flag.SetValueReferenceCount++;
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (Session.ScriptEnvironment.GetFlag(Body.Clauses[0]) is Flag flag)
            {
                flag.Value = !flag.Value;
            }
        }
    }
}
