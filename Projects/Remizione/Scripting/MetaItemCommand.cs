using Engendro;
using EngendroAdventure.Scripting;

namespace Remizione.Scripting
{
    // MetaItemCommand
    // Arguments: {Name} category {ItemCategory} [#action:ItemAction] [#damage:DiceRoll] [#fp:Integer] [#hp:Integer] [#knockback:Vector2] [#maximum:Integer] [#range:Integer] [#stamina:Integer]
    internal sealed class MetaItemCommand : NonAwaitableCommand
    {
        // Constructor
        internal MetaItemCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 3, ActionArg, AngerArg, DamageArg, FPArg, HPArg, ImpactWordArg, KnockbackArg, MaximumArg, MaximumLevelArg, RangeArg)
        {
            var name = ParseItemName(this, 0);
            AssertKeyword(1, "category");
            var anger = Parser.ParseInt32Argument(this, AngerArg);
            var category = Parser.ParseEnum<ItemCategory>(this, 2);
            var action = Parser.ParseEnumArgument<ItemAction>(this, ActionArg, ItemAction.None);
            var damage = Parser.ParseDiceRollArgument(this, DamageArg) ?? DiceRoll.Empty;
            var knockback = Parser.ParseVector2Argument(this, KnockbackArg);
            var maximum = Parser.ParseInt32Argument(this, MaximumArg);
            var fp = Parser.ParseInt32Argument(this, FPArg);
            var hp = Parser.ParseInt32Argument(this, HPArg);
            var range = Parser.ParseInt32Argument(this, RangeArg);

            var metaItem = new MetaItem(name, category, action, damage, knockback, maximum, hp, fp, range, anger);
            MetaItem.Register(metaItem);
        }

        // ParseItemName
        internal static ItemName ParseItemName(Statement statement, int clauseIndex)
        {
            var result = Parser.ParseEnum<ItemName>(statement, clauseIndex);
            if (result == ItemName.None)
                throw new ScriptException(statement, "Item must have a name.");

            return result;
        }
    }
}
