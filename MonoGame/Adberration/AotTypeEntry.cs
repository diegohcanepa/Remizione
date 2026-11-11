using Adberration.Scripting;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Adberration
{
    /// <summary>
    /// AotTypeEntry
    /// </summary>
    public sealed class AotTypeEntry
    {
        // Constructor
        internal AotTypeEntry(string keyName, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type, CodingContext context = CodingContext.Any)
        {
            KeyName = keyName;
            Type = type;
            Context = context;
        }

        // Context
        public CodingContext Context { get; }

        // KeyName
        public string KeyName { get; }

        // Type
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        public Type Type { get; }
    }
}
