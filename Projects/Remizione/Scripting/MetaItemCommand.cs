using EngendroAdventure.Scripting;

namespace Remizione.Scripting
{
    // MetaItemCommand
    // Arguments: {Name} {Category} [#damage:DiceRoll] [#degradation-interval:Integer] [#faith:Integer] [#knockback:Vector2] [#maximum:Integer] [#modifier:Stat] [#range:Integer] [#sound:Name] [#spirit:Integer]
    internal sealed class MetaItemCommand : NonAwaitableCommand
    {
        // Constructor
        internal MetaItemCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 2, ActionArg, BonusArg, CategoryArg, CreationRoutineArg, DamageArg, DegradationIntervalArg, DurabilityArg, FaithArg, HPArg, KnockbackArg, MaximumArg, ModifierArg, PassiveArg, RangeArg, SoundArg, UpgradeHardnessArg, UseIntervalArg)
        {
            var name = Parser.ParseName(this, 0);
            var kind = Parser.ParseEnum<ItemKind>(this, 1);
            var action = Parser.ParseEnumArgument(this, ActionArg, ItemAction.None);
            var bonus = Parser.ParseInt32Argument(this, BonusArg);
            var baseDamage = Parser.ParseDiceExpressionArgument(this, DamageArg) ?? null;
            var creationRoutine = Parser.ParseRoutineArgument(this, CreationRoutineArg);
            var durability = Parser.ParseInt32Argument(this, DurabilityArg, -1);
            var degradationInterval = Parser.ParseInt32Argument(this, DegradationIntervalArg);
            var knockback = Parser.ParseVector2Argument(this, KnockbackArg);
            var maximum = Parser.ParseInt32Argument(this, MaximumArg);
            var modifier = Parser.ParseEnumArgument(this, ModifierArg, Stat.Strength);
            var fp = Parser.ParseDiceExpressionArgument(this, FaithArg);
            var hp = Parser.ParseDiceExpressionArgument(this, HPArg);
            var passive = HasArg(PassiveArg);
            var range = Parser.ParseInt32Argument(this, RangeArg);
            var sound = Parser.ParseSoundArgument(this, SoundArg);
            var upgradeHardness = Parser.ParseEnumArgument<UpgradeHardness>(this, UpgradeHardnessArg);
            var useInterval = Parser.ParseInt32Argument(this, UseIntervalArg);

            new MetaItem(name, kind, action, passive, baseDamage, modifier, bonus, knockback, maximum, hp, fp, range, durability, degradationInterval, upgradeHardness, sound, useInterval, creationRoutine);
        }
    }
}
