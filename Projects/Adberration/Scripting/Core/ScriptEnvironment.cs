using Engendro;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Adberration.Scripting
{
    /// <summary>
    /// ScriptEnvironment
    /// </summary>
    internal sealed class ScriptEnvironment
    {
        #region Private fields

        private readonly Dictionary<string, string> constants = [];
        private readonly Dictionary<string, Counter> counters = [];
        private FrozenDictionary<Type, ScriptEntity> entities = FrozenDictionary<Type, ScriptEntity>.Empty;
        private readonly Dictionary<string, Flag> flags = [];
        private readonly Session session;
        private FrozenDictionary<string, ScriptMethod> sessionMethods = FrozenDictionary<string, ScriptMethod>.Empty;
        private FrozenDictionary<string, ScriptProperty> sessionProperties = FrozenDictionary<string, ScriptProperty>.Empty;
        private FrozenDictionary<string, ScriptStatement> statements = FrozenDictionary<string, ScriptStatement>.Empty;

        #endregion

        #region Constructor

        // Constructor
        internal ScriptEnvironment(Session session)
        {
            this.session = session;
        }

        #endregion

        #region Private members

        // CreateThingClone
        private Thing? CreateThingClone(string declaredName, string instanceName)
        {
            CodeContract.NotDisposed(nameof(Session), session.IsDisposed);

            if (!ScriptSyntax.IsClonedName(instanceName))
                throw new InvalidOperationException($"'{instanceName}' must contains the clone identifier (*).");

            IsCreatingClone = true;

            // Get declaration script from library
            var declarationScript = session.ScriptLibrary.FindScript(ScriptType.Thing, declaredName) ?? throw new InvalidOperationException($"'{declaredName}' cannot be cloned. Use the Clonable keyword.");

            // Check if thing is cloneable
            if (!declarationScript.Cloneable)
                throw new InvalidOperationException($"'{declaredName}' is not cloneable.");

            if (declarationScript.Persistent && ScriptSyntax.IsRuntimeName(instanceName))
                throw new InvalidOperationException($"Cannot create persistent entities at runtime.");

            declarationScript.SetTargetEntity(instanceName);

            // Declaration
            Thing? result = declarationScript.CreateEntity() as Thing;
            declarationScript.Compile();
            session.ScriptProcessor.RunScript(declarationScript);

            IsCreatingClone = false;

            return result;
        }

        // CreateCloneName
        private string CreateCloneName(string typeName)
        {
            var counter = 1;
            while (true)
            {
                var result = typeName + ScriptSyntax.CloneSuffix + counter.ToString(CultureInfo.InvariantCulture) + ScriptSyntax.RuntimeNameSuffix;
                if (session.FindEntity(result) == null)
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
                CodingContext.EntityDeclaration => scriptType is ScriptType.Room or ScriptType.Thing,

                // Declaration
                CodingContext.Declaration => scriptType == ScriptType.Declaration,

                // Instantiation
                CodingContext.Instantiation => scriptType is ScriptType.Cloning or ScriptType.Room or ScriptType.Thing,

                // Initialization
                CodingContext.Initialization => scriptType == ScriptType.Initialization,

                // Execution
                CodingContext.Execution => scriptType is ScriptType.Enter or ScriptType.Outcome or
                                           ScriptType.Routine or ScriptType.NewSession or ScriptType.Load or
                                           ScriptType.Unload or ScriptType.Initialization,
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
            Dictionary<string, ScriptStatement> statementsDict = [];
            var statementType = typeof(Statement);
            foreach (var entry in AotTypeRegistry.Types)
            {
                // Check if type is an entity
                if (!statementType.IsAssignableFrom(entry.Type))
                    continue;

                var instance = new ScriptStatement(session, entry.KeyName, entry.Type, entry.Context);
                statementsDict.Add(entry.KeyName, instance);
            }
            statements = statementsDict.ToFrozenDictionary();

            // Entities
            Dictionary<Type, ScriptEntity> entitiesDict = [];
            var entityType = typeof(Entity);
            foreach (var typeInfo in AotTypeRegistry.Types)
            {
                // Check if type is an entity
                if (!entityType.IsAssignableFrom(typeInfo.Type))
                    continue;

                var instance = new ScriptEntity(session, typeInfo.Type);
                entitiesDict.Add(instance.Type, instance);
            }
            entities = entitiesDict.ToFrozenDictionary();

            // Register session properties
            Dictionary<string, ScriptProperty> sessionPropertiesDict = [];
            foreach (var propertyInfo in session.GetType().GetRuntimeProperties())
            {
                var attribute = propertyInfo.GetCustomAttribute<ScriptPropertyAttribute>();
                if (attribute != null)
                {
                    ScriptProperty property = new(session, propertyInfo.Name, propertyInfo, attribute.Context);

                    // Ensure property does not exists (hiding member feature 'new' keyword)
                    if (!sessionProperties.ContainsKey(property.Name))
                        sessionPropertiesDict[propertyInfo.Name] = property;
                }
            }
            sessionProperties = sessionPropertiesDict.ToFrozenDictionary();

            // Register session methods
            Dictionary<string, ScriptMethod> sessionMethodsDict = [];
            foreach (var methodInfo in session.GetType().GetRuntimeMethods())
            {
                var attribute = methodInfo.GetCustomAttribute<ScriptMethodAttribute>();
                if (attribute != null)
                {
                    ScriptMethod method = new(session, methodInfo.Name, methodInfo, attribute.Context);
                    sessionMethodsDict[method.Name] = method;
                }
            }
            sessionMethods = sessionMethodsDict.ToFrozenDictionary();
        }

        // CheckCodingContext
        internal static void CheckCodingContext(string memberName, CodingContext context, ScriptType scriptType)
        {
            if (!IsValidCodingContext(context, scriptType))
                throw new InvalidOperationException($"Coding context out of scope. The valid context for '{memberName}' is '{context}'.");
        }

        // CreateThingClone
        internal Thing CreateThingClone(string declaredName, string instanceName, bool persistent)
        {
            if (session.State == GameSessionState.Uninitialized)
                throw new InvalidOperationException("Game session not initialized.");

            if (session.State != GameSessionState.Idle && session.State != GameSessionState.AwaitingScripts && session.ScriptLibrary.CompilationPhase != CompilationPhase.Cloning)
                throw new InvalidOperationException("Invalid session state.");

            if (string.IsNullOrWhiteSpace(instanceName))
                instanceName = CreateCloneName(declaredName);

            var result = CreateThingClone(declaredName, instanceName) ?? throw new InvalidOperationException($"Failed to create runtime clone from'{declaredName}'.");

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
        internal string GetConstantValue(string name)
        {
            return constants[name];
        }

        // FindCounter
        internal Counter? FindCounter(string name)
        {
            return counters.TryGetValue(name, out var value) ? value : null;
        }

        // FindEntityType
        internal Type? FindEntityType(string name)
        {
            foreach (var keyValue in entities)
            {
                if (keyValue.Key.Name == name)
                    return keyValue.Key;
            }

            return null;
        }

        // FindFlag
        internal Flag? FindFlag(string name)
        {
            return flags.TryGetValue(name, out var value) ? value : null;
        }

        // FindMethod
        internal ScriptMethod? FindMethod(Type type, string name)
        {
            if (type == session.GetType())
                return FindSessionMethod(name);
            else
                return entities.TryGetValue(type, out var value) ? value.GetMethod(name) : null;
        }

        // FindProperty
        internal ScriptProperty? FindProperty(Type type, string name)
        {
            if (type == session.GetType())
                return FindSessionProperty(name);
            else
                return entities.TryGetValue(type, out var value) ? value.GetProperty(name) : null;
        }

        // FindScriptEntity
        internal ScriptEntity? FindScriptEntity(string name)
        {
            foreach (var item in entities.Values)
            {
                if (item.Type.Name == name)
                    return item;
            }

            return null;
        }

        // FindSessionMethod
        internal ScriptMethod? FindSessionMethod(string name)
        {
            return sessionMethods.TryGetValue(name, out var value) ? value : null;
        }

        // FindSessionProperty
        internal ScriptProperty? FindSessionProperty(string name)
        {
            return sessionProperties.TryGetValue(name, out var value) ? value : null;
        }

        // GetCounters
        internal Counter[] GetCounters()
        {
            return [.. counters.Values];
        }

        // GetFlags
        internal Flag[] GetFlags()
        {
            return [.. flags.Values];
        }

        // IsActive
        internal bool IsActive { get; private set; }

        // IsConstant
        internal static bool IsConstant(string value)
        {
            return value.StartsWith(ScriptSyntax.ConstantPrefix, StringComparison.OrdinalIgnoreCase);
        }

        // IsConstantDeclared
        internal bool IsConstantDeclared(string name)
        {
            return constants.ContainsKey(name);
        }

        // IsCreatingClone
        internal bool IsCreatingClone { get; private set; }

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
        internal bool IsFlagDeclared(string name)
        {
            return flags.ContainsKey(name);
        }

        // IsReservedWord
        internal static bool IsReservedWord(string value)
        {
            return Enum.IsDefined(typeof(ScriptType), value);
        }

        #endregion
    }
}