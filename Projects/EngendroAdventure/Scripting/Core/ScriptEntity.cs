using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace EngendroAdventure.Scripting
{
    /// <summary>
    /// ScriptEntity
    /// </summary>
    internal sealed class ScriptEntity
    {
        private readonly Dictionary<string, ScriptMethod> methods = [];
        private readonly Dictionary<string, ScriptProperty> properties = [];

        // Constructor
        internal ScriptEntity(Session session, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type)
        {
            this.Session = session;
            this.Type = type;

            // Check if type is an entity
            if (!typeof(Entity).IsAssignableFrom(type))
                throw new ArgumentException("The supplied type is not an entity.", nameof(type));

            // Register properties
            foreach (var propertyInfo in type.GetRuntimeProperties())
            {
                var attribute = propertyInfo.GetCustomAttribute<ScriptPropertyAttribute>();
                if (attribute != null)
                {
                    ScriptProperty property = new(session, propertyInfo.Name, propertyInfo, attribute.Context);
                    properties[propertyInfo.Name] = property;
                }
            }

            // Register methods
            foreach (var methodInfo in type.GetRuntimeMethods())
            {
                var attribute = methodInfo.GetCustomAttribute<ScriptMethodAttribute>();
                if (attribute != null)
                {
                    ScriptMethod method = new(session, methodInfo.Name, methodInfo, attribute.Context);
                    methods[method.Name] = method;
                }
            }
        }

        // CreateInstance
        internal Entity? CreateInstance(string entityName)
        {
            return Activator.CreateInstance(Type, Session, entityName) as Entity;
        }

        // Session
        internal Session Session { get; }

        // GetMethod
        internal ScriptMethod? GetMethod(string name)
        {
            return methods.TryGetValue(name, out var value) ? value : null;
        }

        // GetProperty
        internal ScriptProperty? GetProperty(string name)
        {
            return properties.TryGetValue(name, out var value) ? value : null;
        }

        // ToString
        public override string? ToString()
        {
            return Type.FullName;
        }

        // Type
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        internal Type Type { get; }
    }
}
