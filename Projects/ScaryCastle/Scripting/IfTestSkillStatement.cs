using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // IfTestSkillStatement
    // Syntax: {Prop} {Item}
    internal sealed class IfTestSkillStatement : SelectionStatement
    {
        // Constructor
        internal IfTestSkillStatement(Script script, string source, StatementBody args)
            : base(script, StatementType.If, source, args, 3)
        {
            AssertEntity<Prop>(0);
            AssertKeyword(1, "with");
            Script.AssertItemDefinition(Body.Clauses[2]);
        }

        // Evaluate
        public sealed override bool Evaluate()
        {
            if (Session is not GameSession session || session.Player == null)
                return false;

            if (AssertEntity<Prop>(0) is Prop prop)
            {
                if (session.PlayerInventory.Find(Body.Clauses[2]) is Item item)
                {
                    return prop.TestSkillChance(session.Player, item);
                }
            }

            return false;
        }
    }
}
