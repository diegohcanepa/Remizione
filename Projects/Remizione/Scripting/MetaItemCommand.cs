using EngendroAdventure.Scripting;

namespace Remizione.Scripting
{
    // MetaItemCommand
    // Arguments: {Name} {Category:InventoryCategory} [#damage:DiceRoll] [#degradation-interval:Integer] [#knockback:Vector2] [#maximum:Integer] [#modifier:Stat] [#range:Integer] [#sound:Name]
    internal sealed class MetaItemCommand : NonAwaitableCommand
    {
        // Constructor
        internal MetaItemCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 2, ActionArg, AllowEmptyArg, BonusArg, CraftArg, DamageArg, DurabilityArg, HPArg, ImpactWordArg, KnockbackArg, MaximumArg, ModifierArg, PassiveEffectCooldownArg, RangeArg, SoundArg)
        {
            var name = Parser.ParseName(this, 0);
            var category = Parser.ParseEnum<InventoryCategory>(this, 1);
            var maximum = Parser.ParseInt32Argument(this, MaximumArg);

            _ = new MetaItem(name, category, maximum)
            {
                Action = Parser.ParseEnumArgument(this, ActionArg, ItemAction.UseWith),
                AllowEmpty = HasArg(AllowEmptyArg),
                BaseDamage = Parser.ParseDiceExpressionArgument(this, DamageArg) ?? null,
                Bonus = Parser.ParseInt32Argument(this, BonusArg),
                Durability = Parser.ParseInt32Argument(this, DurabilityArg, -1),
                HP = Parser.ParseDiceExpressionArgument(this, HPArg),
                ImpactWord = Parser.ParseEnumArgument(this, ImpactWordArg, ImpactWordKind.None),
                Knockback = Parser.ParseVector2Argument(this, KnockbackArg),
                Modifier = Parser.ParseEnumArgument(this, ModifierArg, StatModifier.None),
                PassiveEffectCooldown = Parser.ParseInt32Argument(this, PassiveEffectCooldownArg),
                Range = Parser.ParseInt32Argument(this, RangeArg),
                Sound = Parser.ParseSoundArgument(this, SoundArg),
            };
        }
    }
}
