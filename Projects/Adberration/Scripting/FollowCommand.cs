namespace Adberration.Scripting
{
    // FollowCommand
    // Syntax: {Thing} [#focus]
    internal sealed class FollowCommand : NonAwaitableCommand
    {
        // Constructor
        internal FollowCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1, FocusArg)
        {
            AssertEntity<Thing>(0);
        }

        // OnExecute
        protected override void OnExecute()
        {
            var thing = AssertEntity<Thing>(0);
            if (thing == null)
                return;

            if (HasArg(FocusArg))
                Session.Camera.Position = thing.Position;

            Session.Camera.FollowTarget(thing);
        }
    }
}
