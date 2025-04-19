namespace EngendroAdventure.Scripting
{
    // FollowCommand
    // Syntax: {Thing} [#focus] [#speed-ratio:Float]
    internal sealed class FollowCommand : NonAwaitableCommand
    {
        private const string SpeedRatioArg = "#speed-ratio";

        // Constructor
        internal FollowCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1, FocusArg, SpeedRatioArg)
        {
            AssertEntity<Thing>(0);
            Parser.ParseRatioArgument(this, SpeedRatioArg, 1);
        }

        // OnExecute
        protected override void OnExecute()
        {
            var thing = AssertEntity<Thing>(0);
            if (thing != null)
            {
                if (HasArg(FocusArg))
                {
                    Session.Camera.Position = thing.Position;
                }

                Session.Camera.FollowTarget(thing);
            }
        }
    }
}
