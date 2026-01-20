using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // SayCommand
    // Arguments: {Actor} {"Text"} [#condition:Flag[...,Flag]] [#literal] [#no-await] [#title:"String"] [#lid:Integer]
    [ForceAwait]
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class SayCommand : LocalizableCommand
    {
        private Actor? actor;

        // Constructor
        public SayCommand(Script script, string source, StatementBody args)
            : base(script, source, args, 2, ConditionArg, LiteralArg, LocalizationIdArg, NoAwaitArg)
        {
            AssertEntity<Actor>(0);
            Parser.ParseQuotedString(this, 1);
            Parser.ParseFlagConditionArgument(this, ConditionArg);
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            actor = AssertEntity<Actor>(0);
            if (actor == null || !actor.InCurrentRoom)
                return;

            if (HasArg(ConditionArg) && Parser.ParseFlagConditionArgument(this, ConditionArg) is FlagCondition condition)
            {
                if (!condition.Evaluate())
                    return;
            }

            string text = GetDisplayText();
            actor.Say(text, !HasArg(NoAwaitArg));
        }

        // OnExecutionCompleted
        protected override void OnExecutionCompleted()
        {
            actor = null;
        }

        // TextClauseIndex
        protected override int TextClauseIndex => 1;

        #endregion

        // GetTextEmitterName
        protected override string GetTextEmitterName()
        {
            return Body.Clauses[0];
        }

        // IsAwaiting
        public override bool IsAwaiting => actor != null && actor.HasSpeechBubble;
    }
}
