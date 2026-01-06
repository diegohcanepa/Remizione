using System;
using System.Diagnostics.CodeAnalysis;

namespace Engendro
{
    /// <summary>
    /// AotTypeEntry
    /// </summary>
    public sealed class AotTypeEntry
    {
        // Constructor
        internal AotTypeEntry(string keyName, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type)
        {
            KeyName = keyName;
            Type = type;
        }

        // KeyName
        public string KeyName { get; }

        // Type
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        public Type Type { get; }
    }
}
