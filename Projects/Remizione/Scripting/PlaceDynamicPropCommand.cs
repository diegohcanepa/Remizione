using EngendroAdventure.Scripting;

namespace Remizione.Scripting
{
    // PlaceDynamicPropCommand
    // Syntax: {StaticName}
    internal sealed class PlaceDynamicPropCommand : NonAwaitableCommand
    {
        // Constructor
        internal PlaceDynamicPropCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            if (Session.GetEntity<Prop>(Body.Clauses[0]) == null)
                throw new ScriptException(this, $"The prop {Body.Clauses[0]} does not exist.");
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is GameSession session && session.Room != null)
            {
                var staticProp = Session.GetEntity<Prop>(Body.Clauses[0]);
                if (staticProp == null)
                    return;

                if (!session.Room.PlaceDynamicPropAt(staticProp))
                {
                    session.HUD.Log.Show(LogMessage.EnoughOfThat, true);
                    return;
                }
            }
        }
    }
}