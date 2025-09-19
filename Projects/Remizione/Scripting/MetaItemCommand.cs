using Adberration.Scripting;

namespace Remizione.Scripting
{
    // MetaItemCommand
    // Arguments: {Name} {Category:InventoryCategory} [#allow-empty] [#chance:Integer] [#damage:DiceRoll] [#degradation-interval:Integer] [#knockback:Vector2] [#maximum:Integer] [#modifier:Stat] [#prevent-discard] [#range:Integer] [#sound:Name]
    internal sealed class MetaItemCommand : NonAwaitableCommand
    {
        // Constructor
        internal MetaItemCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 2, ActionArg, AllowEmptyArg, CriticalChanceArg, DamageArg, DamageKindArg, DurabilityArg, HealthArg, ImpactWordArg, KnockbackArg, MaximumArg, PassiveEffectCooldownArg, PreventDiscardArg, RangeArg, ReplenishPerRoomArg, SkillChanceArg, SoundArg)
        {
            var name = Parser.ParseName(this, 0);
            var category = Parser.ParseEnum<InventoryCategory>(this, 1);
            var maximum = Parser.ParseInt32Argument(this, MaximumArg);

            _ = new MetaItem(name, category, maximum)
            {
                Action = Parser.ParseEnumArgument(this, ActionArg, ItemAction.None),
                AllowEmpty = HasArg(AllowEmptyArg),
                CriticalChance = Parser.ParseInt32Argument(this, CriticalChanceArg),
                Damage = Parser.ParseDiceExpressionArgument(this, DamageArg) ?? null,
                DamageKind = Parser.ParseEnumArgument(this, DamageKindArg, DamageKind.None),
                Durability = Parser.ParseInt32Argument(this, DurabilityArg, -1),
                Health = Parser.ParseDiceExpressionArgument(this, HealthArg),
                ImpactWord = Parser.ParseEnumArgument(this, ImpactWordArg, ImpactWordName.None),
                Knockback = Parser.ParseVector2Argument(this, KnockbackArg),
                PassiveEffectCooldown = Parser.ParseInt32Argument(this, PassiveEffectCooldownArg),
                PreventDiscard = HasArg(PreventDiscardArg),
                Range = Parser.ParseInt32Argument(this, RangeArg),
                ReplenishPerRoom = HasArg(ReplenishPerRoomArg),
                SkillChance = Parser.ParseInt32Argument(this, SkillChanceArg),
                Sound = Parser.ParseSoundArgument(this, SoundArg),
            };
        }
    }
}
