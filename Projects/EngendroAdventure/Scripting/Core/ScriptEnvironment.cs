using Engendro;
using EngendroAdventure.Scripting.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace EngendroAdventure.Scripting
{
    /// <summary>
    /// ScriptEnvironment
    /// </summary>
    internal sealed class ScriptEnvironment
    {
        #region Private fields

        private readonly Dictionary<string, string> constants = [];
        private readonly Dictionary<string, Counter> counters = [];
        private readonly Dictionary<Type, ScriptEntity> entities = [];
        private readonly Dictionary<string, Flag> flags = [];
        private readonly Session session;
        private readonly Dictionary<string, ScriptMethod> sessionMethods = [];
        private readonly Dictionary<string, ScriptProperty> sessionProperties = [];
        private readonly Dictionary<string, ScriptStatement> statements = [];

        #endregion

        #region Constructor

        // Constructor
        internal ScriptEnvironment(Session session)
        {
            this.session = session;
        }

        #endregion

        #region Private members

        // CreateDynamicThingCore
        private Thing? CreateDynamicThingCore(string staticName, string instanceName)
        {
            CodeContract.NotDisposed(nameof(Session), session.IsDisposed);

            if (!ScriptSyntax.IsDynamicName(instanceName))
                throw new InvalidOperationException($"'{instanceName}' must contains the dynamic identifier (*).");

            IsCreatingDynamicEntity = true;

            // Get declaration script from library
            var declarationScript = session.ScriptLibrary.GetScript(ScriptType.Thing, staticName) ?? throw new InvalidOperationException($"'{staticName}' cannot have dynamic instances. Use the Instantible keyword.");

            // Check if thing is instantiable
            if (!declarationScript.Instantiable)
                throw new InvalidOperationException($"'{staticName}' is not instantiable.");

            if (declarationScript.Persistent && ScriptSyntax.IsRuntimeName(instanceName))
                throw new InvalidOperationException($"Cannot create persistent entities at runtime.");

            declarationScript.SetTargetEntity(instanceName);

            // Declaration
            Thing? result = declarationScript.CreateEntity() as Thing;
            declarationScript.Compile();
            session.ScriptProcessor.RunScript(declarationScript);

            IsCreatingDynamicEntity = false;

            return result;
        }

        // CreateDynamicEntityName
        private string CreateDynamicEntityName(string typeName)
        {
            var counter = 1;
            while (true)
            {
                var result = typeName + ScriptSyntax.DynamicSuffix + counter.ToString(CultureInfo.InvariantCulture) + ScriptSyntax.RuntimeNameSuffix;
                if (session.GetEntity(result) == null)
                    return result;

                counter++;
            }
        }

        // IsValidCodingContext
        private static bool IsValidCodingContext(CodingContext context, ScriptType scriptType)
        {
            return context switch
            {
                // Any
                CodingContext.Any => true,

                // EntityDeclaration
                CodingContext.EntityDeclaration => scriptType == ScriptType.Room || scriptType == ScriptType.Thing,

                // Declaration
                CodingContext.Declaration => scriptType == ScriptType.Declaration,

                // Instantiation
                CodingContext.Instantiation => scriptType == ScriptType.Instantiation || scriptType == ScriptType.Room || scriptType == ScriptType.Thing,

                // Initialization
                CodingContext.Initialization => scriptType == ScriptType.Initialization,

                // Execution
                CodingContext.Execution => scriptType == ScriptType.EnterRoom || scriptType == ScriptType.Enter || scriptType == ScriptType.Outcome ||
                                           scriptType == ScriptType.Routine || scriptType == ScriptType.NewSession || scriptType == ScriptType.Load ||
                                           scriptType == ScriptType.Unload || scriptType == ScriptType.Initialization,
                // Default
                _ => false,
            };
            ;
        }

        #endregion

        #region Internal members

        // Activate
        internal void Activate()
        {
            if (IsActive)
                return;

            IsActive = true;

            // Statements
            foreach (var statement in ScriptRegistry.Statements)
            {
                var instance = new ScriptStatement(session, statement.Name, statement.Type, statement.Context);
                statements.Add(statement.Name, instance);
            }

            // Entities
            foreach (var entity in ScriptRegistry.Entities)
            {
                var instance = new ScriptEntity(session, entity.Type);
                entities.Add(instance.Type, instance);
            }

            // Register session properties
            foreach (var propertyInfo in session.GetType().GetRuntimeProperties())
            {
                var attribute = propertyInfo.GetCustomAttribute<ScriptPropertyAttribute>();
                if (attribute != null)
                {
                    ScriptProperty property = new(session, propertyInfo.Name, propertyInfo, attribute.Context);

                    // Ensure property does not exists (hiding member feaure 'new' keyword)
                    if (!sessionProperties.ContainsKey(property.Name))
                        sessionProperties[propertyInfo.Name] = property;
                }
            }

            // Register session methods
            foreach (var methodInfo in session.GetType().GetRuntimeMethods())
            {
                var attribute = methodInfo.GetCustomAttribute<ScriptMethodAttribute>();
                if (attribute != null)
                {
                    ScriptMethod method = new(session, methodInfo.Name, methodInfo, attribute.Context);
                    sessionMethods[method.Name] = method;
                }
            }
        }

        // CheckCodingContext
        internal static void CheckCodingContext(string memberName, CodingContext context, ScriptType scriptType)
        {
            if (!IsValidCodingContext(context, scriptType))
                throw new InvalidOperationException($"Coding context out of scope. The valid context for '{memberName}' is '{context}'.");
        }

        // CreateDynamicThing
        internal Thing CreateDynamicThing(string staticName, string instanceName, bool persistent)
        {
            if (session.State == GameSessionState.Uninitialized)
                throw new InvalidOperationException("Game session not initialized.");

            if (session.State != GameSessionState.Idle && session.ScriptLibrary.CompilationPhase != CompilationPhase.Instantiation)
                throw new InvalidOperationException();

            if (string.IsNullOrWhiteSpace(instanceName))
                instanceName = CreateDynamicEntityName(staticName);

            var result = CreateDynamicThingCore(staticName, instanceName) ?? throw new InvalidOperationException("Cannot create dynamic entity.");

            if (persistent)
                result.Persistent = persistent;

            result.Initialize();

            return result;
        }

        // CreateEntity
        internal Entity? CreateEntity(string typeName, string entityName)
        {
            foreach (var keyValue in entities)
            {
                if (keyValue.Key.Name == typeName)
                    return keyValue.Value.CreateInstance(entityName);
            }

            return null;
        }

        // CreateStatement
        internal Statement CreateStatement(string name, Script script, string source, StatementBody body)
        {
            if (!statements.TryGetValue(name, out var statement))
                throw new ArgumentException($"Unrecognized command '{name}'.");

            CheckCodingContext(statement.Name, statement.Context, script.ScriptType);

            var result = statement.CreateInstance(script, source, body) ?? throw new InvalidOperationException("Cannot create statement.");

            return result;
        }

        // DeclareConstant
        internal void DeclareConstant(string name, string value)
        {
            NameValidator.Validate(name);
            constants.Add(name, value);
        }

        // DeclareCounter
        internal void DeclareCounter(string name, int value, bool persistent)
        {
            if (counters.ContainsKey(name))
                throw new InvalidOperationException($"Duplicated counter declaration: '{name}'.");

            counters.Add(name, new Counter(name, value, persistent));
        }

        // DeclareFlag
        internal void DeclareFlag(string name, bool value, bool persistent)
        {
            if (flags.ContainsKey(name))
                throw new InvalidOperationException($"A flag named '{name}' is already declared.");

            flags.Add(name, new Flag(name, value, persistent));
        }

        // GetConstantValue
        internal string GetConstantValue(string name) => constants[name];

        // GetCounter
        internal Counter? GetCounter(string name) => counters.TryGetValue(name, out var value) ? value : null;

        // GetCounters
        internal Counter[] GetCounters() => counters.Values.ToArray();

        // GetEntityType
        internal Type? GetEntityType(string name)
        {
            foreach (var keyValue in entities)
            {
                if (keyValue.Key.Name == name)
                    return keyValue.Key;
            }

            return null;
        }

        // GetFlag
        internal Flag? GetFlag(string name)
        {
            return flags.TryGetValue(name, out var value) ? value : null;
        }

        // GetFlags
        internal Flag[] GetFlags() => flags.Values.ToArray();

        // GetMethod
        internal ScriptMethod? GetMethod(Type type, string name)
        {
            if (type == session.GetType())
                return GetSessionMethod(name);
            else
                return entities.TryGetValue(type, out var value) ? value.GetMethod(name) : null;
        }

        // GetProperty
        internal ScriptProperty? GetProperty(Type type, string name)
        {
            if (type == session.GetType())
                return GetSessionProperty(name);
            else
                return entities.TryGetValue(type, out var value) ? value.GetProperty(name) : null;
        }

        // GetScriptEntity
        internal ScriptEntity? GetScriptEntity(string name)
        {
            foreach (var item in entities.Values)
            {
                if (item.Type.Name == name)
                    return item;
            }

            return null;
        }

        // GetSessionMethod
        internal ScriptMethod? GetSessionMethod(string name)
        {
            return sessionMethods.TryGetValue(name, out var value) ? value : null;
        }

        // GetSessionProperty
        internal ScriptProperty? GetSessionProperty(string name)
        {
            return sessionProperties.TryGetValue(name, out var value) ? value : null;
        }

        // IsActive
        internal bool IsActive { get; private set; }

        // IsConstant
        internal static bool IsConstant(string value)
        {
            return value.StartsWith(ScriptSyntax.ConstantPrefix, StringComparison.OrdinalIgnoreCase);
        }

        // IsConstantDeclared
        internal bool IsConstantDeclared(string name) => constants.ContainsKey(name);

        // IsCreatingDynamicEntity
        internal bool IsCreatingDynamicEntity { get; private set; }

        // IsEntityTypeRegistered
        internal bool IsEntityTypeRegistered(string name)
        {
            foreach (var keyValue in entities)
            {
                if (keyValue.Key.Name == name)
                    return true;
            }

            return false;
        }

        // IsFlagDeclared
        internal bool IsFlagDeclared(string name) => flags.ContainsKey(name);

        // IsReservedWord
        internal static bool IsReservedWord(string value) => Enum.IsDefined(typeof(ScriptType), value);

        // ScriptRegistry
        internal ScriptRegistry ScriptRegistry { get; } = new ScriptRegistry();

        #endregion
    }
}