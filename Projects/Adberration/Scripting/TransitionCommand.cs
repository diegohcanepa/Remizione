using Engendro;

namespace Adberration.Scripting
{
    // TransitionCommand
    // Arguments: {TransitionState} [#duration:Int32] [#immediate]
    internal sealed class TransitionCommand : AwaitableCommand
    {
        // Constructor
        internal TransitionCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1, DurationArg, ImmediateArg)
        {
            Parser.ParseEnum<TransitionState>(this, 0);
            Parser.ParseInt32Argument(this, DurationArg);
        }

        // OnExecute
        protected override void OnExecute()
        {
            var mode = Parser.ParseEnum<TransitionState>(this, 0);
            var duration = Parser.ParseInt32Argument(this, DurationArg, HasArg(ImmediateArg) ? 0 : TransitionManager.CurrentTransition.DefaultDuration);

            if (mode == TransitionState.In)
            {
                TransitionManager.CurrentTransition.In(duration);
            }
            else
            {
                TransitionManager.CurrentTransition.Out(duration);
            }
        }

        // IsAwaiting
        public override bool IsAwaiting()
        {
            return TransitionManager.CurrentTransition.IsRunning;
        }
    }
}
