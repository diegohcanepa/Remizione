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
            if (Session.GetEntity<IsometricProp>(Body.Clauses[0]) == null)
                throw new ScriptException(this, $"The prop {Body.Clauses[0]} does not exist.");
        }

        // OnExecute
        protected override void OnExecute()
        {
            /*
            if (Session is GameSession session && session.Room is ProceduralRoom room)
            {
                var staticProp = Session.GetEntity<IsometricProp>(Body.Clauses[0]);
                if (staticProp == null)
                    return;

                if (room.PlaceDynamicProp(staticProp) is GameThing thing)
                {
                    var tween = new Vector2Tween() { StartDelay = 250 };
                    tween.Start(TweenStyle.CubicIn, Vector2.Zero, Vector2.One, 250);
                    thing.Tweens.ScaleTween = tween;
                    session.Environment.Lightning.Show(thing.Position - new Vector2(0, 5));
                    session.Player?.Inventory.RemoveSelected();
                }
                else
                {
                    session.HUD.Message.Show(HUDMessageKind.CannotPlaceItem, true);
                    return;
                }
            }
            */
        }
    }
}