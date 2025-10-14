using Adberration.Scripting;

namespace Remizione.Scripting
{
    // MetaItemCommand
    // Arguments: {Name} {Category:InventoryCategory} [#allow-empty] [#chance:Integer] [#damage:DiceRoll] [#degradation-interval:Integer] [#knockback:Vector2] [#maximum:Integer] [#modifier:Stat] [#prevent-discard] [#range:Integer] [#sound:Name]
    internal sealed class MetaItemCommand : NonAwaitableCommand
    {
        // Constructor
        internal MetaItemCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 2, ActionArg, AllowEmptyArg, CriticalChanceArg, DamageArg, DamageIntensityArg, DamageTypeArg, DurabilityArg, HPArg, ImpactWordArg, KnockbackArg, PassiveEffectCooldownArg, PreventDiscardArg, RangeArg, ReplenishAmountArg, SkillChanceArg, SoundArg)
        {
            var name = Parser.ParseName(this, 0);
            var category = Parser.ParseEnum<InventoryCategory>(this, 1);

            _ = new MetaItem(name, category)
            {
                Action = Parser.ParseEnumArgument(this, ActionArg, ItemAction.None),
                AllowEmpty = HasArg(AllowEmptyArg),
                CriticalChance = Parser.ParseInt32Argument(this, CriticalChanceArg),
                Damage = Parser.ParseDiceExpressionArgument(this, DamageArg) ?? null,
                DamageType = Parser.ParseEnumArgument(this, DamageTypeArg, DamageType.Physical),
                Durability = Parser.ParseInt32Argument(this, DurabilityArg, -1),
                HP = Parser.ParseDiceExpressionArgument(this, HPArg),
                ImpactWord = Parser.ParseEnumArgument(this, ImpactWordArg, ImpactWordName.None),
                Knockback = Parser.ParseVector2Argument(this, KnockbackArg),
                PassiveEffectCooldown = Parser.ParseInt32Argument(this, PassiveEffectCooldownArg),
                PreventDiscard = HasArg(PreventDiscardArg),
                Range = Parser.ParseInt32Argument(this, RangeArg),
                ReplenishAmount = Parser.ParseInt32Argument(this, ReplenishAmountArg),
                SkillChance = Parser.ParseInt32Argument(this, SkillChanceArg),
                Sound = Parser.ParseSoundArgument(this, SoundArg),
            };
        }
    }
}
