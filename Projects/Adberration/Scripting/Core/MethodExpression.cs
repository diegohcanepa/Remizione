using System;

namespace Adberration.Scripting
{
    /// <summary>
    /// MethodExpression
    /// </summary>
    public sealed class MethodExpression
    {
        private readonly ScriptMethod? method;

        // Constructor
        public MethodExpression(Script script, string value)
        {
            // Empty value
            if (string.IsNullOrWhiteSpace(value))
                return;

            if (value.StartsWith(ScriptSyntax.SessionPropertyAlias))
                value = value.Replace(ScriptSyntax.SessionPropertyAlias, "Session.");

            value = value.Replace("()", string.Empty);

            var tokens = value.Split('.');

            // (in context)
            if (script.HasCapability(ScriptCapability.EntityContext))
            {
                if (tokens.Length == 1)
                {
                    value = script.EntityName + ScriptSyntax.MemberSeparator + value;
                    tokens = value.Split('.');
                }

                // 'this' keyword
                else if (tokens.Length == 2 && tokens[0] == ScriptSyntax.ThisKeyword)
                {
                    tokens[0] = script.EntityName;
                }
            }

            // Session.Method
            if (tokens[0] == ScriptSyntax.SessionKeyword && tokens.Length == 2)
            {
                Instance = script.Session;
                method = script.Session.ScriptEnvironment.GetSessionMethod(tokens[1]);
            }

            // Session.Entity.Method
            else if (tokens[0] == ScriptSyntax.SessionKeyword && tokens.Length == 3)
            {
                if (script.Session.ScriptEnvironment.GetSessionProperty(tokens[1]) is ScriptProperty entityProperty)
                {
                    method = script.Session.ScriptEnvironment.GetMethod(entityProperty.PropertyType, tokens[2]);
                    Instance = entityProperty.GetValue(script.Session);
                }
            }

            // Entity.Method
            else
            {
                if (script.Session.GetEntity<Entity>(tokens[0]) is Entity entity)
                {
                    Instance = entity;
                    method = script.Session.ScriptEnvironment.GetMethod(Instance.GetType(), tokens[1]);
                }
            }

            if (method == null)
                throw new ScriptException($"'{value}' is not a valid method reference expression.");
        }

        // Instance
        public object? Instance { get; }

        // Method
        public ScriptMethod Method => method ?? throw new InvalidOperationException();
    }
}
