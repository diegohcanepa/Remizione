using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Engendro
{
    /// <summary>
    /// AotTypeRegistry
    /// </summary>
    public static class AotTypeRegistry
    {
        private static readonly Dictionary<string, RegisteredType> types = [];

        // Find
        public static RegisteredType? Find(string keyName)
        {
            if (types.TryGetValue(keyName, out var result))
                return result;
            else
                return null;
        }

        // Get
        public static RegisteredType Get(string keyName)
        {
            return types[keyName];
        }

        // Register
        public static void Register(Type type)
        {
            Register(type.Name, type);
        }

        // Register
        public static void Register(string keyName, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type)
        {
            if (types.ContainsKey(keyName))
                return;

            types.Add(keyName, new RegisteredType(keyName, type));
        }

        // Types
        public static IEnumerable<RegisteredType> Types => types.Values;

        /// <summary>
        /// RegisteredType
        /// </summary>
        public sealed class RegisteredType
        {
            // Constructor
            internal RegisteredType(string keyName, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type)
            {
                this.Type = type;
                this.KeyName = keyName;
            }

            // KeyName
            public string KeyName { get; }

            // Type
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
            public Type Type { get; }
        }
    }
}
