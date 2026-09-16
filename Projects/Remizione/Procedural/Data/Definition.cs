using Engendro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace Remizione
{
    /// <summary>
    /// Definition
    /// </summary>
    public abstract class Definition : INamedObject
    {
        private static readonly List<Definition> all = [];
        private readonly List<EffectDescriptor> effectDescriptors = [];
        private static readonly HashSet<string> usedNames = new(StringComparer.Ordinal);

        #region Constructor

        // Constructor
        protected Definition(JsonElement element, NameValidationRule nameValidationRule = NameValidationRule.Strict)
        {
            this.Name = element.GetProperty("name").GetString() ?? throw new InvalidOperationException("Name not found.");

            CodeContract.ValidName(this.Name, string.Empty);

            if (nameValidationRule == NameValidationRule.Strict)
            {
                // Name cannot be a category
                if (Enum.IsDefined(typeof(ItemCategory), Name))
                    RaiseValidationError(this, $"The name '{Name}' cannot be used because it is an item category.");
            }

            if (nameValidationRule != NameValidationRule.AllowDuplicates)
            {
                if (usedNames.Contains(Name))
                    RaiseValidationError(this, $"The name '{Name}' cannot be used because it is already being used by another definition.");
                else
                    usedNames.Add(Name);
            }

            // IsUnlockedByDefault
            this.IsUnlockedByDefault = element.GetBool("isUnlockedByDefault", false);

            // SpawnWeight
            SpawnWeight = element.GetFloat("spawnWeight", 1);

            // Effects
            if (element.TryGetProperty("effects", out JsonElement effectsArray))
            {
                foreach (var effectJson in effectsArray.EnumerateArray())
                {
                    effectDescriptors.Add(new(effectJson));
                }
            }

            EffectDescriptors = effectDescriptors.AsReadOnly();

            all.Add(this);
        }

        #endregion

        #region Protected members

        // OnRefreshLocalizedValues
        protected virtual void OnRefreshLocalizedValues()
        {
        }

        // RaiseValidationError
        protected static void RaiseValidationError(INamedObject obj, string message, string? relatedProperty = null)
        {
            relatedProperty = relatedProperty == null ? string.Empty : "." + relatedProperty;
            throw new InvalidOperationException($"[{obj.Name}{relatedProperty}]: {message}");
        }

        #endregion

        // All
        public static ReadOnlyCollection<Definition> All { get; } = all.AsReadOnly();

        // EffectDescriptors
        public ReadOnlyCollection<EffectDescriptor> EffectDescriptors { get; }

        // IsDefined
        public static bool IsDefined(string name)
        {
            return usedNames.Contains(name);
        }

        // IsUnlockedByDefault
        public bool IsUnlockedByDefault { get; }

        // Name
        public string Name { get; }

        // RefreshLocalizedValues
        public void RefreshLocalizedValues()
        {
            OnRefreshLocalizedValues();
        }

        // SpawnWeight
        public Ratio SpawnWeight { get; }

        // ToString
        public override string ToString()
        {
            return Name;
        }
    }
}
