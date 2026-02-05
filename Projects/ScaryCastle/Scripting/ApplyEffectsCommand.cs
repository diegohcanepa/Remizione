using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // ApplyEffectsCommand
    // Arguments: {Source:GameThing} {Target:GameThing}
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class ApplyEffectsCommand : NonAwaitableCommand
    {
        // Constructor
        internal ApplyEffectsCommand(Script script, string source, StatementBody args)
            : base(script, source, args, 2)
        {
            AssertEntity<GameThing>(0);
            AssertEntity<GameThing>(1);
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (AssertEntity<GameThing>(0) is GameThing source && AssertEntity<GameThing>(1) is GameThing target)
                EffectDescriptor.Apply(source, target);
        }
    }
}
