using EngendroAdventure.Scripting;

namespace Remizione.Scripting
{
    // MetaItemCommand
    // Arguments: {Name} {Category:MetaItemCategory} [#damage:DiceRoll] [#degradation-interval:Integer] [#faith:Integer] [#knockback:Vector2] [#maximum:Integer] [#modifier:Stat] [#range:Integer] [#sound:Name] [#spirit:Integer]
    internal sealed class MetaItemCommand : NonAwaitableCommand
    {
        // Constructor
        internal MetaItemCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 2, ActionArg, AllowEmptyArg, BonusArg, CraftArg, DamageArg, DurabilityArg, FaithArg, HPArg, KnockbackArg, MaximumArg, ModifierArg, PassiveEffectCooldownArg, RangeArg, SoundArg)
        {
            var name = Parser.ParseName(this, 0);
            var category = Parser.ParseEnum<MetaItemCategory>(this, 1);
            var maximum = Parser.ParseInt32Argument(this, MaximumArg);

            _ = new MetaItem(name, category, maximum)
            {
                Action = Parser.ParseEnumArgument(this, ActionArg, ItemAction.UseWith),
                AllowEmpty = HasArg(AllowEmptyArg),
                BaseDamage = Parser.ParseDiceExpressionArgument(this, DamageArg) ?? null,
                Bonus = Parser.ParseInt32Argument(this, BonusArg),
                Durability = Parser.ParseInt32Argument(this, DurabilityArg, -1),
                Faith = Parser.ParseDiceExpressionArgument(this, FaithArg),
                HP = Parser.ParseDiceExpressionArgument(this, HPArg),
                Knockback = Parser.ParseVector2Argument(this, KnockbackArg),
                Modifier = Parser.ParseEnumArgument(this, ModifierArg, Stat.Strength),
                PassiveEffectCooldown = Parser.ParseInt32Argument(this, PassiveEffectCooldownArg),
                Range = Parser.ParseInt32Argument(this, RangeArg),
                Sound = Parser.ParseSoundArgument(this, SoundArg),
                Tickets = Parser.ParseDiceExpressionArgument(this, TicketsArg)
            };
        }
    }
}
