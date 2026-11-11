using System.Collections.Generic;

namespace Adberration.Scripting
{
    // ReloadCommand
    // Arguments: {Entity[,...]}]
    internal sealed class ReloadCommand : NonAwaitableCommand
    {
        // Constructor
        internal ReloadCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            Parser.ParseEntities<Entity>(this, 0);
        }

        // OnExecute
        protected override void OnExecute()
        {
            List<Entity?> list = [.. Parser.ParseEntities<Entity>(this, 0)];

            for (var i = 0; i < list.Count; i++)
            {
                if (list[i] is Entity entity)
                    entity.Reload();
            }
        }
    }
}
