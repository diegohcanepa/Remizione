using Engendro;
using EngendroAdventure.Scripting;
using Microsoft.Xna.Framework;

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
            if (Session is GameSession session && session.Room != null)
            {
                var staticProp = Session.GetEntity<IsometricProp>(Body.Clauses[0]);
                if (staticProp == null)
                    return;

                if (session.Room.PlaceDynamicProp(staticProp) is GameThing thing)
                {
                    thing.Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.CubicIn, Vector2.Zero, Vector2.One, 250);
                    session.Environment.Lightning.Show(thing.Position - new Vector2(0, 5));
                }
                else
                {
                    session.HUD.Log.Show(LogMessage.CannoPlaceItem, true);
                    return;
                }
            }
        }
    }
}