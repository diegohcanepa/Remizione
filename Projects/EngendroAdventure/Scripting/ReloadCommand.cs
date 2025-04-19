namespace EngendroAdventure.Scripting
{
    // ReloadCommand
    // Arguments: {Entity}[,Entity...]
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
            var targets = Parser.ParseEntities<Entity>(this, 0);

            for (var i = 0; i < targets.Length; i++)
            {
                targets[i]?.Reload();
            }
        }
    }
}
