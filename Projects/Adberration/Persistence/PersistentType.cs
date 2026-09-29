using Engendro.Collections;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Adberration.Persistence
{
    /// <summary>
    /// PersistentType
    /// </summary>
    public sealed class PersistentType
    {
        private readonly NamedCollection<PersistentProperty> trackedProperties = [];

        #region Constructor

        // Constructor
        internal PersistentType([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type, PersistentTypeScope persistenceScope)
        {
            this.Type = type;
            this.PersistenceScope = persistenceScope;
            this.TrackedProperties = new NamedReadOnlyCollection<PersistentProperty>(trackedProperties);

            if (!typeof(Entity).GetTypeInfo().IsAssignableFrom(type))
                throw new ArgumentException("Type is not an entity type.", nameof(type));
        }

        #endregion

        // PersistenceScope
        public PersistentTypeScope PersistenceScope { get; }

        // Track
        public PersistentProperty Track(string propertyName)
        {
            if (propertyName is (nameof(Entity.Name)) or (nameof(Entity.Parent)))
                throw new InvalidOperationException($"The property '{propertyName}' is implicitly persisted as part of the persistent model.");

            if (trackedProperties.Contains(propertyName))
                throw new ArgumentException($"Property '{propertyName}' is already tracked.", nameof(propertyName));

            PersistentProperty result = new(this, propertyName);
            trackedProperties.Add(result);
            return result;
        }

        // TrackedProperties
        public NamedReadOnlyCollection<PersistentProperty> TrackedProperties { get; }

        // Type
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        public Type Type { get; }
    }
}
