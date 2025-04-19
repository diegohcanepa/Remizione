namespace EngendroAdventure.Scripting
{
    // RandomPositionCommand
    // Arguments: {Thing[,Thing]} polygon {Polygon}
    internal sealed class RandomPositionCommand : NonAwaitableCommand
    {
        // Constructor
        internal RandomPositionCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 3)
        {
            Parser.ParseEntities<Thing>(this, 0);
            AssertKeyword(1, "polygon");
            Parser.ParsePolygon(this, 2);
        }

        // OnExecute
        protected override void OnExecute()
        {
            var things = Parser.ParseEntities<Thing>(this, 0);
            var polygon = Parser.ParsePolygon(this, 2);

            for (var i = 0; i < things.Length; i++)
            {
                if (things[i] is Thing thing)
                {
                    thing.Position = polygon.RandomPoint();
                }
            }
        }
    }
}
