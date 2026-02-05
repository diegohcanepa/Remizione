using Adberration.Scripting;
using Microsoft.Xna.Framework;

namespace ScaryCastle.Scripting
{
    // TakeDamageCommand
    // Arguments: {Source:GameThing} {Target:GameThing} {DamageType} {Amount:Integer} [#word:ImpactWordName]
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class TakeDamageCommand : NonAwaitableCommand
    {
        // Constructor
        internal TakeDamageCommand(Script script, string source, StatementBody args)
            : base(script, source, args, 5, WordArg)
        {
            AssertEntity<GameThing>(0);
            AssertEntity<GameThing>(1);
            Parser.ParseEnum<DamageType>(this, 3);
            Parser.ParseInt32(this, 4);
            Parser.ParseEnumArgument<ImpactWordName>(this, WordArg, ImpactWordName.None);
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is not GameSession session)
                return;

            if (AssertEntity<GameThing>(0) is not GameThing sourceThing)
                return;

            if (AssertEntity<GameThing>(1) is not GameThing targetThing)
                return;

            var damageType = Parser.ParseEnum<DamageType>(this, 3);
            var amount = Parser.ParseInt32(this, 4);
            var wordName = Parser.ParseEnumArgument<ImpactWordName>(this, WordArg, ImpactWordName.None);

            targetThing.TakeDamage(sourceThing, damageType, amount, wordName, Vector2.Zero);
        }
    }
}
