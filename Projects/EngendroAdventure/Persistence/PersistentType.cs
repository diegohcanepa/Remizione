using Engendro;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace EngendroAdventure.Persistence
{
    /// <summary>
    /// PersistentType
    /// </summary>
    public sealed class PersistentType
    {
        private readonly NamedObjectCollection<PersistentProperty> mappedProperties = [];

        // Constructor
        internal PersistentType(Type type, PersistentTypeScope persistenceScope)
        {
            this.Type = type;
            this.PersistenceScope = persistenceScope;
            this.MappedProperties = new NamedObjectReadOnlyCollection<PersistentProperty>(mappedProperties);

            if (!typeof(Entity).GetTypeInfo().IsAssignableFrom(type))
                throw new ArgumentException("Type is not an entity type.", nameof(type));
        }

        // Map
        public PersistentProperty Map(string propertyName)
        {
            if (propertyName == nameof(Entity.Name) || propertyName == nameof(Entity.Parent))
            {
                throw new InvalidOperationException($"The property '{propertyName}' is implicitly persisted as part of the persistent model.");
            }

            if (mappedProperties.Contains(propertyName))
            {
                throw new ArgumentException($"Property '{propertyName}' is already mapped.", nameof(propertyName));
            }

            PersistentProperty result = new(this, propertyName);
            mappedProperties.Add(result);
            return result;
        }

        // MappedProperties
        public NamedObjectReadOnlyCollection<PersistentProperty> MappedProperties { get; }

        // PersistenceScope
        public PersistentTypeScope PersistenceScope { get; }

        // Type
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        public Type Type { get; }
    }
}
