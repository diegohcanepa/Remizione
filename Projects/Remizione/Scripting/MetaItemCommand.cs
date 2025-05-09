using Engendro;
using EngendroAdventure.Scripting;

namespace Remizione.Scripting
{
    // MetaItemCommand
    // Arguments: {Name} category {ItemCategory} [#action:ItemAction] [#damage:DiceRoll] [#faith:Integer] [#hp:Integer] [#knockback:Vector2] [#maximum:Integer] [#modifier:Stat] [#range:Integer] [#stamina:Integer]
    internal sealed class MetaItemCommand : NonAwaitableCommand
    {
        // Constructor
        internal MetaItemCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 3, ActionArg, DamageArg, DurabilityArg, FaithArg, HPArg, ImpactWordArg, KnockbackArg, MaximumArg, MaximumLevelArg, ModifierArg, RangeArg)
        {
            var name = Parser.ParseName(this, 0);
            AssertKeyword(1, "category");
            var category = Parser.ParseEnum<ItemCategory>(this, 2);
            var action = Parser.ParseEnumArgument<ItemAction>(this, ActionArg, ItemAction.None);
            var damage = Parser.ParseDiceRollArgument(this, DamageArg) ?? DiceRoll.Empty;
            var durability = Parser.ParseInt32Argument(this, DurabilityArg);
            var knockback = Parser.ParseVector2Argument(this, KnockbackArg);
            var maximum = Parser.ParseInt32Argument(this, MaximumArg);
            var modifier = Parser.ParseEnumArgument<Stat>(this, ModifierArg, Stat.Strength);
            var fp = Parser.ParseInt32Argument(this, FaithArg);
            var hp = Parser.ParseInt32Argument(this, HPArg);
            var range = Parser.ParseInt32Argument(this, RangeArg);
            
            new MetaItem(name, category, action, damage, modifier, knockback, maximum, hp, fp, range, durability);
        }
    }
}
