using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// RunDefinition
    /// </summary>
    public sealed class RunDefinition : Definition
    {
        private readonly List<FloorDescriptor> floors = [];

        #region Constructor

        // Constructor
        public RunDefinition(JsonElement element)
            : base(element, NameValidationRule.Unique)
        {
            // Floors
            if (element.TryGetProperty("floors", out JsonElement floorsElement))
            {
                foreach (var floor in floorsElement.EnumerateArray())
                {
                    floors.Add(new FloorDescriptor(floor));
                }
            }

            Floors = floors.AsReadOnly();

            if (Floors.Count == 0)
                throw new InvalidOperationException("Run descriptor must specify at least 1 floor.");
        }

        #endregion

        // Floors
        public ReadOnlyCollection<FloorDescriptor> Floors { get; }
    }
}
