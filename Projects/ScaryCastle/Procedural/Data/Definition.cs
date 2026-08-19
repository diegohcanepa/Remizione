using Engendro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// Definition
    /// </summary>
    public abstract class Definition : INamedObject
    {
        private static readonly HashSet<string> definitions = [];
        private readonly List<EffectDescriptor> effectDescriptors = [];

        #region Constructor

        // Constructor
        protected Definition(JsonElement element, bool uniqueName = true)
        {
            this.Name = element.GetProperty("name").GetString() ?? throw new InvalidOperationException("Name not found.");

            CodeContract.ValidName(this.Name, string.Empty);

            // Name cannot be a realm 
            if (Enum.IsDefined(typeof(Realm), Name))
                RaiseValidationError(this, $"The name '{Name}' cannot be used because it is an item realm.");

            // Name cannot be a category
            if (Enum.IsDefined(typeof(ItemCategory), Name))
                RaiseValidationError(this, $"The name '{Name}' cannot be used because it is an item category.");

            if (uniqueName)
            {
                if (definitions.Contains(Name))
                    RaiseValidationError(this, $"The name '{Name}' cannot be used because it is already being used by another definition.");
                else
                    definitions.Add(Name);
            }

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
        }

        #endregion

        #region Protected members

        // RaiseValidationError
        protected static void RaiseValidationError(INamedObject obj, string message, string? relatedProperty = null)
        {
            relatedProperty = relatedProperty == null ? string.Empty : "." + relatedProperty;
            throw new InvalidOperationException($"[{obj.Name}{relatedProperty}]: {message}");
        }

        #endregion

        // EffectDescriptors
        public ReadOnlyCollection<EffectDescriptor> EffectDescriptors { get; }

        // Name
        public string Name { get; }

        // SpawnWeight
        public Ratio SpawnWeight { get; }

        // ToString
        public override string ToString()
        {
            return Name;
        }
    }
}
