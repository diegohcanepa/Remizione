using Engendro;
using EngendroAdventure.Scripting;

namespace Remizione.Scripting
{
    // MetaItemCommand
    // Arguments: {Name} category {ItemCategory} [#damage:DiceRoll] [#faith:Integer] [#knockback:Vector2] [#maximum:Integer] [#modifier:Stat] [#range:Integer] [#sound:Name] [#spirit:Integer]
    internal sealed class MetaItemCommand : NonAwaitableCommand
    {
        // Constructor
        internal MetaItemCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 3, DamageArg, DurabilityArg, FaithArg, KnockbackArg, MaximumArg, ModifierArg, RangeArg, SoundArg, SpiritArg, UpgradeHardnessArg)
        {
            var name = Parser.ParseName(this, 0);
            AssertKeyword(1, "category");
            var category = Parser.ParseEnum<ItemCategory>(this, 2);
            var damage = Parser.ParseDiceRollArgument(this, DamageArg) ?? DiceRoll.Empty;
            var durability = Parser.ParseInt32Argument(this, DurabilityArg);
            var knockback = Parser.ParseVector2Argument(this, KnockbackArg);
            var maximum = Parser.ParseInt32Argument(this, MaximumArg);
            var modifier = Parser.ParseEnumArgument<Stat>(this, ModifierArg, Stat.Strength);
            var fp = Parser.ParseInt32Argument(this, FaithArg);
            var hp = Parser.ParseInt32Argument(this, SpiritArg);
            var range = Parser.ParseInt32Argument(this, RangeArg);
            var sound = Parser.ParseSoundArgument(this, SoundArg);
            var upgradeHardness = Parser.ParseEnumArgument<UpgradeHardness>(this, UpgradeHardnessArg);

            new MetaItem(name, category, damage, modifier, knockback, maximum, hp, fp, range, durability, upgradeHardness);
        }
    }
}
