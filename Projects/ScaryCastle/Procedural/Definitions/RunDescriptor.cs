using Engendro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// RunDescriptor
    /// </summary>
    public sealed class RunDescriptor : INamedObject
    {
        private readonly List<FloorDescriptor> floors = [];

        #region Constructor

        // Constructor
        public RunDescriptor(JsonElement element)
        {
            Index = element.GetProperty("runIndex").GetInt32();
            Name = $"Run_{Index}";

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

            Data.Add(this);
        }

        #endregion

        // Data
        public static DataContainer<RunDescriptor> Data { get; } = new(element => new RunDescriptor(element));

        // Floors
        public ReadOnlyCollection<FloorDescriptor> Floors { get; }

        // Index
        public int Index { get; }

        // Name
        public string Name { get; }
    }
}
