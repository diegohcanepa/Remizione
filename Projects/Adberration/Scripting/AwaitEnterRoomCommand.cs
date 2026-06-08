namespace Adberration.Scripting
{
    // AwaitEnterRoomCommand
    // Syntax: {Room}
    [ForceAwait]
    internal sealed class AwaitEnterRoomCommand : AwaitableCommand
    {
        private Room? room;

        // Constructor
        internal AwaitEnterRoomCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            AssertEntity<Room>(0);
        }

        // OnExecute
        protected override void OnExecute()
        {
            room = AssertEntity<Room>(0);
            if (room != null)
            {
                if (!Session.EnterRoom(room))
                {
                    room = null;
                }
            }
        }

        // OnExecutionCompleted
        protected override void OnExecutionCompleted()
        {
            base.OnExecutionCompleted();
            room = null;
        }

        // IsAwaiting
        public override bool IsAwaiting() => room != null && room != Session.PreviousRoom && Session.IsEnteringRoom(room);
    }
}
