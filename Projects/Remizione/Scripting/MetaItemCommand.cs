using EngendroAdventure.Scripting;

namespace Remizione.Scripting
{
    // MetaItemCommand
    // Arguments: {Name} {Category:MetaItemCategory} [#damage:DiceRoll] [#degradation-interval:Integer] [#faith:Integer] [#knockback:Vector2] [#maximum:Integer] [#modifier:Stat] [#range:Integer] [#sound:Name] [#sacrifice-reward:DerivedStat] [#sacrifice-reward-amount:Integer] [#spirit:Integer]
    internal sealed class MetaItemCommand : NonAwaitableCommand
    {
        // Constructor
        internal MetaItemCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 2, AllowEmptyArg, BonusArg, DamageArg, DurabilityArg, FaithArg, HPArg, KnockbackArg, MaximumArg, ModifierArg, PassiveEffectCooldownArg, RangeArg, SacrificeRewardArg, SacrificeRewardAmountArg, SoundArg)
        {
            var name = Parser.ParseName(this, 0);
            var category = Parser.ParseEnum<MetaItemCategory>(this, 1);
            var bonus = Parser.ParseInt32Argument(this, BonusArg);
            var baseDamage = Parser.ParseDiceExpressionArgument(this, DamageArg) ?? null;
            var durability = Parser.ParseInt32Argument(this, DurabilityArg, -1);
            var knockback = Parser.ParseVector2Argument(this, KnockbackArg);
            var maximum = Parser.ParseInt32Argument(this, MaximumArg);
            var modifier = Parser.ParseEnumArgument(this, ModifierArg, Stat.Strength);
            var fp = Parser.ParseDiceExpressionArgument(this, FaithArg);
            var hp = Parser.ParseDiceExpressionArgument(this, HPArg);
            var passiveEffectCooldown = Parser.ParseInt32Argument(this, PassiveEffectCooldownArg);
            var range = Parser.ParseInt32Argument(this, RangeArg);
            var sacrificeReward = Parser.ParseEnumArgument(this, SacrificeRewardArg, DerivedStat.Faith);
            var sacrificeRewardAmount = Parser.ParseInt32Argument(this, SacrificeRewardAmountArg, 1);
            var sound = Parser.ParseSoundArgument(this, SoundArg);

            _ = new MetaItem(name, category, passiveEffectCooldown, baseDamage, modifier, bonus, knockback, maximum, hp, fp, range, durability, sound)
            {
                AllowEmpty = HasArg(AllowEmptyArg),
                SacrificeReward = sacrificeReward,
                SacrificeRewardAmount = sacrificeRewardAmount
            };
        }
    }
}
