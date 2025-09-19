using Adberration.Scripting;

namespace Remizione.Scripting
{
    // AwaitUseFriendlyItemCommand
    // Arguments: {Actor} title {"Action"} [#success-state:PropState]
    [ForceAwait]
    internal sealed class AwaitUseFriendlyItemCommand : LocalizableCommand
    {
        // Constructor
        internal AwaitUseFriendlyItemCommand(Script script, string source, StatementBody args)
            : base(script, source, args, 3, SuccessStateArg)
        {
            AssertEntity<Actor>(0);
            AssertKeyword(1, "title");
            Parser.ParseQuotedString(this, 2);
            Parser.ParseEnumArgument<PropState>(this, SuccessStateArg);
        }

        // TextClauseIndex
        protected override int TextClauseIndex => 2;

        // OnExecute
        protected override void OnExecute()
        {
            // Actor
            if (AssertEntity<Actor>(0) is not Actor actor)
                return;

            actor.ChooseFriendlyItem(Parser.ParseQuotedString(this, 2));
        }

        // IsAwaiting
        public override bool IsAwaiting => Game.SceneManager.CurrentScene is UseFriendlyItemScene;
    }
}
