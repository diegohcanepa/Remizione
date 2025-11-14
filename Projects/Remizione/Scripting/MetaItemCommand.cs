using Adberration.Scripting;

namespace Remizione.Scripting
{
    // MetaItemCommand
    // Arguments: {Name} {Category:InventoryCategory} [#allow-empty] [#chance:Integer] [#damage:DiceRoll] [#degradation-interval:Integer] [#knockback:Vector2] [#maximum:Integer] [#modifier:Stat] [#prevent-discard] [#range:Integer] [#sound:Name]
    internal sealed class MetaItemCommand : NonAwaitableCommand
    {
        // Constructor
        internal MetaItemCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 2, ActionArg, BaseWeightArg, CriticalChanceArg, DamageArg, DamageIntensityArg, DamageTypeArg, DurabilityArg, ExcludeTagsArg, HPArg, ImpactWordArg, KnockbackArg, PassiveEffectCooldownArg, PickupSoundArg, PreventDiscardArg, QualityArg, RangeArg, RealmArg, RequiredTagsArg, SkillChanceArg, SoundArg, SouvenirArg, StackModeArg, TagsArg, UnlockedArg)
        {
            var name = Parser.ParseName(this, 0);
            var category = Parser.ParseEnum<ItemCategory>(this, 1);

            var tagsArgValue = Parser.ParseArgumentValue(this, TagsArg) ?? string.Empty;
            var tags = string.IsNullOrWhiteSpace(tagsArgValue) ? [] : Parser.ParseEnums<LootTag>(this, tagsArgValue);

            var requiredTagsArgValue = Parser.ParseArgumentValue(this, RequiredTagsArg) ?? string.Empty;
            var requiredTags = string.IsNullOrWhiteSpace(requiredTagsArgValue) ? [] : Parser.ParseEnums<LootTag>(this, requiredTagsArgValue);

            var excludeTagsArgValue = Parser.ParseArgumentValue(this, ExcludeTagsArg) ?? string.Empty;
            var excludeTags = string.IsNullOrWhiteSpace(excludeTagsArgValue) ? [] : Parser.ParseEnums<LootTag>(this, excludeTagsArgValue);

            _ = new MetaItem(name, category, tags, requiredTags, excludeTags)
            {
                Action = Parser.ParseEnumArgument(this, ActionArg, ItemAction.None),
                BaseWeight = Parser.ParseFloatArgument(this, DurabilityArg, 1),
                CriticalChance = Parser.ParseInt32Argument(this, CriticalChanceArg),
                Damage = Parser.ParseDiceExpressionArgument(this, DamageArg) ?? null,
                DamageType = Parser.ParseEnumArgument(this, DamageTypeArg, DamageType.Physical),
                Durability = Parser.ParseInt32Argument(this, DurabilityArg, -1),
                HP = Parser.ParseDiceExpressionArgument(this, HPArg),
                ImpactWord = Parser.ParseEnumArgument(this, ImpactWordArg, ImpactWordName.None),
                IsSouvenir = HasArg(SouvenirArg),
                Knockback = Parser.ParseVector2Argument(this, KnockbackArg),
                PassiveEffectCooldown = Parser.ParseInt32Argument(this, PassiveEffectCooldownArg),
                PickupSound = Parser.ParseSoundArgument(this, PickupSoundArg),
                PreventDiscard = HasArg(PreventDiscardArg),
                Quality = Parser.ParseInt32Argument(this, QualityArg),
                Range = Parser.ParseInt32Argument(this, RangeArg),
                Realm = Parser.ParseEnumArgument(this, RealmArg, ItemRealm.Earthly),
                SkillChance = Parser.ParseInt32Argument(this, SkillChanceArg),
                Sound = Parser.ParseSoundArgument(this, SoundArg),
                StackMode = Parser.ParseEnumArgument(this, StackModeArg, StackMode.None),
                Unlocked = HasArg(UnlockedArg)
            };
        }
    }
}
