using EngendroAdventure.Scripting;

namespace Remizione.Scripting
{
    // MetaItemCommand
    // Arguments: {Name} {Category} [#damage:DiceRoll] [#degradation-interval:Integer] [#faith:Integer] [#knockback:Vector2] [#maximum:Integer] [#modifier:Stat] [#range:Integer] [#sound:Name] [#spirit:Integer]
    internal sealed class MetaItemCommand : NonAwaitableCommand
    {
        // Constructor
        internal MetaItemCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 2, ActionArg, AllowEmptyArg, BonusArg, CategoryArg, CreationRoutineArg, DamageArg, DurabilityArg, FaithArg, HPArg, KnockbackArg, MaximumArg, ModifierArg, PassiveArg, RangeArg, SacrificeRewardArg, SoundArg, UseIntervalArg)
        {
            var name = Parser.ParseName(this, 0);
            var category = Parser.ParseEnum<ItemContainerCategory>(this, 1);
            var action = Parser.ParseEnumArgument(this, ActionArg, ItemAction.None);
            var bonus = Parser.ParseInt32Argument(this, BonusArg);
            var baseDamage = Parser.ParseDiceExpressionArgument(this, DamageArg) ?? null;
            var creationRoutine = Parser.ParseRoutineArgument(this, CreationRoutineArg);
            var durability = Parser.ParseInt32Argument(this, DurabilityArg, -1);
            var knockback = Parser.ParseVector2Argument(this, KnockbackArg);
            var maximum = Parser.ParseInt32Argument(this, MaximumArg);
            var modifier = Parser.ParseEnumArgument(this, ModifierArg, Stat.Strength);
            var fp = Parser.ParseDiceExpressionArgument(this, FaithArg);
            var hp = Parser.ParseDiceExpressionArgument(this, HPArg);
            var passive = HasArg(PassiveArg);
            var range = Parser.ParseInt32Argument(this, RangeArg);
            var sacrificeReward = Parser.ParseEnumArgument(this, SacrificeRewardArg, SacrificeReward.Faith);
            var sound = Parser.ParseSoundArgument(this, SoundArg);
            var useInterval = Parser.ParseInt32Argument(this, UseIntervalArg);

            _ = new MetaItem(name, category, action, passive, baseDamage, modifier, bonus, knockback, maximum, hp, fp, range, durability, sound, useInterval, creationRoutine)
            {
                AllowEmpty = HasArg(AllowEmptyArg),
                SacrificeReward = sacrificeReward
            };
        }
    }
}
