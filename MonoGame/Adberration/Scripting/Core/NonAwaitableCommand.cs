
namespace Adberration.Scripting
{
    /// <summary>
    /// NonAwaitableCommand
    /// </summary>
    public abstract class NonAwaitableCommand : Command
    {
        // Constructor
        protected NonAwaitableCommand(Script script, string source, StatementBody body, int clauseCount, params string[] supportedFlags)
            : base(script, StatementType.NonAwaitableCommmand, source, body, clauseCount, supportedFlags)
        {
            if (body.Await)
                throw new ScriptException(this, $"The command {Name} does not support await.");
        }
    }
}
