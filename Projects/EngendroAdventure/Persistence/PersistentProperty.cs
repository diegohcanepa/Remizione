using Engendro;
using EngendroAdventure.Persistence;
using System;
using System.Reflection;

namespace EngendroAdventure
{
    /// <summary>
    /// PersistentProperty
    /// </summary>
    public sealed class PersistentProperty : INamedObject
    {
        // Constructor
        internal PersistentProperty(PersistentType persistentType, string propertyName)
        {
            this.PersistentType = persistentType;
            this.Name = propertyName;
            this.PropertyInfo = persistentType.Type.GetRuntimeProperty(propertyName) ?? throw new InvalidOperationException("Property does not exist.");

            if (!PersistenceModel.IsTypeSupported(PropertyInfo.PropertyType))
                throw new InvalidOperationException("Unsupported type.");

            // If property is read-only but it is not one of the suppprted classes
            if (!PropertyInfo.CanWrite && (PropertyInfo.PropertyType.IsValueType || PropertyInfo.PropertyType == typeof(string)))
                throw new InvalidOperationException($"Property '{propertyName}' cannot be persistent because it is read-only.");
        }

        // Name
        public string Name { get; }

        // PersistentType
        public PersistentType PersistentType { get; }

        // PropertyInfo
        public PropertyInfo PropertyInfo { get; }

        // ToString
        public override string ToString()
        {
            return Name;
        }
    }
}
