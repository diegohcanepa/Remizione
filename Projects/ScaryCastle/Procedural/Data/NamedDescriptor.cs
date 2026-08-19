using Engendro;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// NamedDescriptor
    /// </summary>
    public abstract class NamedDescriptor : INamedObject
    {
        private static readonly HashSet<string> descriptors = [];

        // Constructor
        protected NamedDescriptor(JsonElement element)
        {
            this.Name = element.GetProperty("name").GetString() ?? throw new InvalidOperationException("Name not found.");

            CodeContract.ValidName(this.Name, string.Empty);
            if (descriptors.Contains(Name))
                RaiseValidationError(this, $"The name '{Name}' cannot be used because it is already being used by another definition.");
            else
                descriptors.Add(Name);
        }

        // RaiseValidationError
        protected static void RaiseValidationError(INamedObject obj, string message, string? relatedProperty = null)
        {
            relatedProperty = relatedProperty == null ? string.Empty : "." + relatedProperty;
            throw new InvalidOperationException($"[{obj.Name}{relatedProperty}]: {message}");
        }

        // Name
        public string Name { get; }

        // ToString
        public override string ToString()
        {
            return Name;
        }
    }
}
