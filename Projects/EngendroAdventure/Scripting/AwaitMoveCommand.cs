namespace EngendroAdventure.Scripting
{
    // AwaitMoveCommand
    // Arguments: {Thing[,...]} [#include-tweens]
    [ForceAwait]
    internal sealed class AwaitMoveCommand : AwaitableCommand
    {
        private const string IncludeTweensArg = "#include-tweens";

        private Thing?[]? targets;

        // Constructor
        internal AwaitMoveCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1, IncludeTweensArg)
        {
            Parser.ParseEntities<Thing>(this, 0);
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            targets = Parser.ParseEntities<Thing>(this, 0);
        }

        // OnExecutionCompleted
        protected override void OnExecutionCompleted()
        {
            base.OnExecutionCompleted();
            targets = null;
        }

        #endregion

        // IsAwaiting
        public override bool IsAwaiting
        {
            get
            {
                if (targets != null && targets.Length > 0)
                {
                    for (var i = 0; i < targets.Length; i++)
                    {
                        if (targets[i] is Thing thing)
                        {
                            if (thing.IsMoving || (HasArg(IncludeTweensArg) && thing.Tweens.IsTweeningPosition))
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
        }
    }
}
