using System;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// ResistanceTable
    /// </summary>
    public sealed class ResistanceTable
    {
        private readonly Dictionary<DamageType, float> modifiers = [];
        private static readonly Dictionary<string, ResistanceTable> tables = [];

        // Constructor
        public ResistanceTable(float defaultModifier = 1)
        {
            foreach (DamageType type in Enum.GetValues<DamageType>())
            {
                modifiers[type] = defaultModifier;
            }
        }

        #region Static members

        // Find
        public static ResistanceTable? Find(string name)
        {
            return tables.TryGetValue(name, out var result) ? result : null;
        }

        // Register
        public static ResistanceTable Register(string name, float defaultModifier = 1)
        {
            if (tables.ContainsKey(name))
                throw new InvalidOperationException($"Table '{name}' already exists.");

            var result = new ResistanceTable(defaultModifier);
            tables[name] = result;

            return result;
        }

        #endregion

        // GetModifier
        public float GetModifier(DamageType type)
        {
            return modifiers.TryGetValue(type, out float modifier) ? modifier : 1;
        }

        // SetModifier
        public void SetModifier(DamageType type, float value)
        {
            modifiers[type] = value;
        }
    }
}
