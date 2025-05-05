using EngendroAdventure.Scripting;

namespace Remizione.Scripting
{
    // Verbs
    // Arguments: {Verb}[,Verb...]
    internal sealed class VerbsCommand : NonAwaitableCommand
    {
        // Constructor
        internal VerbsCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            var thing = AssertEntityNotNull<GameThing>(Script.EntityName);
            var verbs = Parser.ParseEnums<Verb>(this, 0);
            thing.AddVerbs(verbs);
        }
    }
}
