using Engendro;
using Adberration.Persistence;
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
    public abstract class PersistenceModel
    {
        private readonly Collection<PersistentType> mappedTypes = [];

        #region Constructor

        // Constructor
        protected PersistenceModel(string version)
        {
            this.MappedTypes = new ReadOnlyCollection<PersistentType>(mappedTypes);
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
                if (GetMappedType(t) is PersistentType persistentType)
                {
                    stack.Push(persistentType);
                    if (persistentType.PersistenceScope == PersistentTypeScope.DeclaredOnly)
                    {
                        break;
                    }
                }

                t = t.BaseType;
                if (t == null || !typeof(Entity).IsAssignableFrom(t))
                {
                    break;
                }
            }

            return stack.ToArray();
        }

        #endregion

        #region Internal members

        // Deserialize
        internal void Deserialize(Entity entity, XmlNode input)
        {
            // Properties
            foreach (var persistentType in GetPersistentTypeHierarchy(entity))
            {
                foreach (var property in persistentType.MappedProperties)
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
                foreach (var property in persistentType.MappedProperties)
                {
                    PropertySerializer.Serialize(property, obj, output);
                }
            }
        }

        #endregion

        // GetMappedType
        public PersistentType? GetMappedType(Type type)
        {
            for (var i = 0; i < mappedTypes.Count; i++)
            {
                if (mappedTypes[i].Type == type)
                {
                    return mappedTypes[i];
                }
            }

            return null;
        }

        // IsTypeSupported
        public static bool IsTypeSupported(Type type)
        {
            if (type == typeof(bool))
            {
                return true;
            }

            if (type == typeof(Color))
            {
                return true;
            }

            if (typeof(Entity).IsAssignableFrom(type))
            {
                return true;
            }

            if (typeof(Enum).IsAssignableFrom(type))
            {
                return true;
            }

            if (type == typeof(int))
            {
                return true;
            }

            if (type == typeof(long))
            {
                return true;
            }

            if (type == typeof(Polygon))
            {
                return true;
            }

            if (type == typeof(Rectangle))
            {
                return true;
            }

            if (type == typeof(RectangleF))
            {
                return true;
            }

            if (type == typeof(float))
            {
                return true;
            }

            if (type == typeof(string))
            {
                return true;
            }

            if (type == typeof(TimeSpan))
            {
                return true;
            }

            if (type == typeof(Vector2))
            {
                return true;
            }

            return false;
        }

        // MappedTypes
        public ReadOnlyCollection<PersistentType> MappedTypes { get; }

        // MapType
        public PersistentType MapType(Type type)
        {
            return MapType(type, PersistentTypeScope.InheritedAndDeclared);
        }

        // MapType
        public PersistentType MapType(Type type, PersistentTypeScope persistenceScope)
        {
            if (GetMappedType(type) != null)
            {
                throw new ArgumentException("Type already mapped.", nameof(type));
            }

            PersistentType result = new(type, persistenceScope);
            mappedTypes.Add(result);
            return result;
        }

        // Version
        public string Version { get; }
    }
}
