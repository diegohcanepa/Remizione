using Adberration.Scripting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Adberration
{
    /// <summary>
    /// AotTypeRegistry
    /// </summary>
    public static class AotTypeRegistry
    {
        private static readonly Dictionary<string, AotTypeEntry> types = [];

        // Find
        public static AotTypeEntry? Find(string keyName)
        {
            return types.TryGetValue(keyName, out var result) ? result : null;
        }

        // Get
        public static AotTypeEntry Get(string keyName)
        {
            return types[keyName];
        }

        // Register
        public static void Register([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type)
        {
            Register(type.Name, type);
        }

        // Register
        public static void Register(string keyName, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type, CodingContext context = CodingContext.Execution)
        {
            if (types.ContainsKey(keyName))
                return;

            types.Add(keyName, new AotTypeEntry(keyName, type, context));
        }

        // Types
        public static IEnumerable<AotTypeEntry> Types => types.Values;
    }
}
