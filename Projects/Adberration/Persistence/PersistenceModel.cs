using Adberration.Persistence;
using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;

namespace Adberration
{
    /// <summary>
    /// PersistenceModel
    /// </summary>
    public class PersistenceModel
    {
        private readonly Collection<PersistentType> trackedTypes = [];

        #region Constructor

        // Constructor
        protected PersistenceModel(string version)
        {
            this.TrackedTypes = new(trackedTypes);
            this.Version = version;
        }

        #endregion

        #region Private members

        // GetPersistentTypeHierarchy
        private PersistentType[] GetPersistentTypeHierarchy(object obj)
        {
            Stack<PersistentType> stack = new();

            var t = obj.GetType();
            while (true)
            {
                if (Find(t) is PersistentType persistentType)
                {
                    stack.Push(persistentType);
                    if (persistentType.PersistenceScope == PersistentTypeScope.DeclaredOnly)
                    {
                        break;
                    }
                }

                t = t.BaseType;
                if (t == null || !typeof(Entity).IsAssignableFrom(t))
                    break;
            }

            return [.. stack];
        }

        #endregion

        #region Internal members

        // Deserialize
        internal void Deserialize(Entity entity, XmlNode input)
        {
            // Properties
            foreach (var persistentType in GetPersistentTypeHierarchy(entity))
            {
                foreach (var property in persistentType.TrackedProperties)
                {
                    PropertySerializer.Deserialize(property, entity, input);
                }
            }
        }

        // Serialize
        internal void Serialize(object obj, XmlWriter output)
        {
            foreach (var persistentType in GetPersistentTypeHierarchy(obj))
            {
                foreach (var property in persistentType.TrackedProperties)
                {
                    PropertySerializer.Serialize(property, obj, output);
                }
            }
        }

        #endregion

        // Empty
        public static PersistenceModel Empty = new(string.Empty);

        // Find
        public PersistentType? Find(Type type)
        {
            for (var i = 0; i < trackedTypes.Count; i++)
            {
                if (trackedTypes[i].Type == type)
                    return trackedTypes[i];
            }

            return null;
        }

        // IsTypeSupported
        public static bool IsTypeSupported(Type type)
        {
            if (type == typeof(bool))
                return true;

            if (type == typeof(Color))
                return true;

            if (typeof(Entity).IsAssignableFrom(type))
                return true;

            if (typeof(Enum).IsAssignableFrom(type))
                return true;

            if (type == typeof(int))
                return true;

            if (type == typeof(long))
                return true;

            if (type == typeof(Polygon))
                return true;

            if (type == typeof(Rectangle))
                return true;

            if (type == typeof(RectangleF))
                return true;

            if (type == typeof(float))
                return true;

            if (type == typeof(string))
                return true;

            if (type == typeof(TimeSpan))
                return true;

            if (type == typeof(Vector2))
                return true;

            return false;
        }

        // Track
        public PersistentType Track(Type type)
        {
            return Track(type, PersistentTypeScope.InheritedAndDeclared);
        }

        // Track
        public PersistentType Track(Type type, PersistentTypeScope persistenceScope)
        {
            if (Find(type) != null)
                throw new ArgumentException("Type already exists.", nameof(type));

            PersistentType result = new(type, persistenceScope);
            trackedTypes.Add(result);
            return result;
        }

        // TrackedTypes
        public ReadOnlyCollection<PersistentType> TrackedTypes { get; }

        // Version
        public string Version { get; }
    }
}
