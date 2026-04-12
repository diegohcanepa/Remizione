using Adberration.Scripting;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace ScaryCastle.Scripting
{
    // LiftThrowableCommand
    // Syntax: {Source:Actor} {Target:Prop}
    internal sealed class LiftThrowableCommand : NonAwaitableCommand
    {
        // Constructor
        internal LiftThrowableCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 2)
        {
            AssertEntity<Actor>(0);
            AssertEntity<Prop>(1);
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is not GameSession session)
                return;

            if (AssertEntity<Actor>(0) is Actor actor && AssertEntity<Prop>(1) is Prop prop)
            {
                actor.ActiveThrowable = prop;
                if (actor.EffortShortSound is Sound sound)
                    sound.Play();
            }
        }

        #endregion
    }
}