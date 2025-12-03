using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Remizione
{
    /// <summary>
    /// MetaItem
    /// </summary>
    public sealed class MetaItem
    {
        private static readonly Dictionary<string, MetaItem> items = [];
        private static bool loaded;

        #region Constructor

        // Constructor
        private MetaItem(JsonElement element)
        {
            // Name
            this.Name = element.GetProperty("name").GetString() ?? throw new InvalidDataException("Name not found.");

            CodeContract.ValidName(this.Name, string.Empty);

            Utils.AssertName(Name, this);

            // Name cannot be a realm 
            if (Enum.IsDefined(typeof(Realm), Name))
                throw new InvalidOperationException($"The name '{Name}' cannot be used because it is an item realm.");

            // Name cannot be a category
            if (Enum.IsDefined(typeof(ItemCategory), Name))
                throw new InvalidOperationException($"The name '{Name}' cannot be used because it is an item category.");

            // Action
            if (element.TryGetProperty("action", out JsonElement actionElement) && actionElement.GetString() is string actionValue)
                Action = Enum.Parse<ItemAction>(actionValue);

            // Category
            if (element.TryGetProperty("category", out JsonElement categoryElement) && categoryElement.GetString() is string categoryValue)
                Category = Enum.Parse<ItemCategory>(categoryValue);

            // CriticalChance
            if (element.TryGetProperty("criticalChance", out JsonElement criticalChanceElement))
                CriticalChance = criticalChanceElement.GetInt32();
            else
                CriticalChance = 1;

            // Damage
            if (element.TryGetProperty("damage", out JsonElement damageElement) && damageElement.GetString() is string damageValue)
                Damage = new(damageValue);

            // DamageType
            if (element.TryGetProperty("damageType", out JsonElement damageTypeElement) && damageTypeElement.GetString() is string damageTypeValue)
                DamageType = Enum.Parse<DamageType>(damageTypeValue);

            // Durability
            if (element.TryGetProperty("durability", out JsonElement durabilityElement))
                Durability = durabilityElement.GetInt32();

            // HP
            if (element.TryGetProperty("hp", out JsonElement hpElement) && hpElement.GetString() is string hpValue)
                HP = new(hpValue);

            // ImpactWord
            if (element.TryGetProperty("impactWord", out JsonElement impactWordElement) && impactWordElement.GetString() is string impactWordValue)
                ImpactWord = Enum.Parse<ImpactWordName>(impactWordValue);

            // IsStackable
            if (element.TryGetProperty("isStackable", out JsonElement isStackableElement))
                IsStackable = isStackableElement.GetBoolean();

            // Knockback
            if (element.TryGetProperty("knockback", out JsonElement knockbackElement) && knockbackElement.GetString() is string knockbackValue)
                Knockback = DataConverter.ToVector2(knockbackValue);

            // PassiveEffectCooldown
            if (element.TryGetProperty("passiveEffectCooldown", out JsonElement passiveEffectCooldownElement))
                PassiveEffectCooldown = passiveEffectCooldownElement.GetInt32();

            // PickupSound
            if (element.TryGetProperty("pickupSound", out JsonElement pickupSoundElement) && pickupSoundElement.GetString() is string pickupSoundValue)
                PickupSound = Sound.FindNotNull(pickupSoundValue);

            // PreventDiscard
            if (element.TryGetProperty("preventDiscard", out JsonElement preventDiscardElement))
                PreventDiscard = preventDiscardElement.GetBoolean();

            // Quality
            if (element.TryGetProperty("quality", out JsonElement qualityElement))
                Quality = qualityElement.GetInt32();

            // Range
            if (element.TryGetProperty("range", out JsonElement rangeElement))
                Range = rangeElement.GetInt32();

            // Realm
            if (element.TryGetProperty("realm", out JsonElement realmElement) && realmElement.GetString() is string realmValue)
                Realm = Enum.Parse<Realm>(realmValue);

            // SkillChance
            if (element.TryGetProperty("skillChance", out JsonElement skillChanceElement))
                SkillChance = skillChanceElement.GetInt32();

            // Sound
            if (element.TryGetProperty("sound", out JsonElement soundElement) && soundElement.GetString() is string soundValue)
                Sound = Sound.FindNotNull(soundValue);

            // StackMode
            if (element.TryGetProperty("stackMode", out JsonElement stackModeElement) && stackModeElement.GetString() is string stackModeValue)
                StackMode = Enum.Parse<StackMode>(stackModeValue);

            // Unlocked
            if (element.TryGetProperty("unlocked", out JsonElement unlockedElement))
                Unlocked = unlockedElement.GetBoolean();

            // Weight
            if (element.TryGetProperty("weight", out JsonElement weightElement))
                Weight = weightElement.GetSingle();
            else
                Weight = 1;

            this.LocalizedDescription = Localization.GetItemDescription(this);
            this.LocalizedDisplayName = Localization.GetItemName(this);
            this.Image = Atlases.UI.GetImage(Name);

            items.Add(Name, this);
        }

        #endregion

        #region Private members

        // CalculateKnockback
        private Vector2 CalculateKnockback(ActorSize size)
        {
            var knockbackBase = Knockback;

            return size switch
            {
                // Small
                ActorSize.Small => knockbackBase * 1.5f,
                // Medium
                ActorSize.Medium => knockbackBase * 1f,
                // Large
                ActorSize.Large => knockbackBase * .3f,
                _ => knockbackBase,
            };
        }

        #endregion

        #region Static members

        // AllItems
        public static IEnumerable<MetaItem> AllItems => items.Values;

        // Find
        public static MetaItem? Find(string name)
        {
            if (items.TryGetValue(name, out var result))
                return result;
            else
                return null;
        }

        // FindNotNull
        public static MetaItem FindNotNull(string name)
        {
            return Find(name) ?? throw new InvalidOperationException($"MetaItem '{name}' not found.");
        }

        // GetItems
        public static List<MetaItem> GetItems(ItemCategory category)
        {
            var result = new List<MetaItem>();

            foreach (var item in items.Values)
            {
                if (item.Category == category)
                    result.Add(item);
            }

            return result;
        }

        // GetItems
        public static List<MetaItem> GetItems(Realm realm)
        {
            var result = new List<MetaItem>();

            foreach (var item in items.Values)
            {
                if (item.Realm == realm)
                    result.Add(item);
            }

            return result;
        }

        // Load
        public static void Load(string fileName)
        {
            if (loaded)
                throw new InvalidOperationException("Data is already loaded.");

            Utils.LoadJsonData(fileName, (JsonElement element) => new MetaItem(element));

            loaded = true;
        }

        #endregion

        // Action
        public ItemAction Action { get; }

        // ApplyDamage
        public bool ApplyDamage(GameThing attacker, GameThing target)
        {
            if (Damage == null)
                return false;

            int damageAmount = Damage.Roll();

            var finalKnockback = Knockback;
            if (target is Actor actor)
                finalKnockback = CalculateKnockback(actor.BodySize);

            target.TakeDamage(attacker, damageAmount, DamageType, DiceExpression.Dice100.Roll() <= CriticalChance, finalKnockback, ImpactWord);

            return true;
        }

        // Category
        public ItemCategory Category { get; }

        // CoinItemName
        public const string CoinItemName = "Coin";

        // CriticalChance
        public int CriticalChance { get; }

        // Damage
        public DiceExpression? Damage { get; }

        // DamageType
        public DamageType DamageType { get; }

        // Durability
        public int Durability { get; }

        // HP
        public DiceExpression? HP { get; }

        // Image
        public AtlasImage? Image { get; }

        // ImpactWord
        public ImpactWordName ImpactWord { get; }

        // IsEquipment
        public bool IsEquipment => Category is ItemCategory.LeftHand or ItemCategory.RightHand or ItemCategory.Gadget;

        // IsPassive
        public bool IsPassive => PassiveEffectCooldown > 0;

        // IsStackable
        public bool IsStackable { get; }

        // Knockback
        public Vector2 Knockback { get; init; }

        // LocalizedDescription
        public string LocalizedDescription { get; }

        // LocalizedDisplayName
        public string LocalizedDisplayName { get; }

        // Name
        public string Name { get; }

        // PassiveEffectCooldown
        public int PassiveEffectCooldown { get; }

        // PickupSound
        public Sound? PickupSound { get; }

        // PreventDiscard
        public bool PreventDiscard { get; }

        // Quality
        public int Quality { get; }

        // Range
        public int Range { get; }

        // Realm
        public Realm Realm { get; }

        // SkillChance
        public int SkillChance { get; }

        // Sound
        public Sound? Sound { get; }

        // StackMode
        public StackMode StackMode { get; }

        // ToString
        public override string ToString()
        {
            return Name;
        }

        // Unlocked
        public bool Unlocked { get; }

        // Weight
        public float Weight { get; }
    }
}
