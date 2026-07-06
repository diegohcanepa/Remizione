using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // AwaitMonitorTextCommand
    // Arguments: {"Text"}
    [ForceAwait]
    internal sealed class AwaitMonitorTextCommand : LocalizableCommand
    {
        private Monitor? monitorRoom;

        // Constructor
        public AwaitMonitorTextCommand(Script script, string source, StatementBody args)
            : base(script, source, args, 1, FastArg, ActionArg, ColorArg)
        {
            Parser.ParseQuotedString(this, 0);
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            monitorRoom = Session.Room as Monitor;
            if (monitorRoom == null)
                return;

            string text = GetDisplayText();
            monitorRoom.AddText(text, HasArg(FastArg), HasArg(ColorArg));
        }

        // OnExecutionCompleted
        protected override void OnExecutionCompleted()
        {
            base.OnExecutionCompleted();
            monitorRoom = null;
        }

        // TextClauseIndex
        protected override int TextClauseIndex => 0;

        #endregion

        // GetTextEmitterName
        protected override string GetTextEmitterName()
        {
            return Body.Clauses[0];
        }

        // IsAwaiting
        public override bool IsAwaiting()
        {
            return monitorRoom != null && monitorRoom.IsTypingText;
        }
    }
}
