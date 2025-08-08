using System;

namespace Adberration.Scripting
{
    /// <summary>
    /// PropertyExpression
    /// </summary>
    public sealed class PropertyExpression
    {
        private readonly ScriptProperty? property;

        // Constructor
        public PropertyExpression(Statement statement, string value)
        {
            // Empty value
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            // $
            if (value.StartsWith(ScriptSyntax.SessionPropertyAlias))
            {
                value = value.Replace(ScriptSyntax.SessionPropertyAlias, ScriptSyntax.SessionKeyword + ".");
            }

            // Entity context script
            else if (statement.Script.HasCapability(ScriptCapability.EntityContext))
            {
                if (!value.Contains(ScriptSyntax.MemberSeparator))
                {
                    value = statement.Script.EntityName + ScriptSyntax.MemberSeparator + value;
                }
                else if (value.StartsWith(ScriptSyntax.ThisKeyword + ScriptSyntax.MemberSeparator))
                {
                    value = value.Substring(ScriptSyntax.ThisKeyword.Length);
                    value = statement.Script.EntityName + value;
                }
            }

            var isSessionProperty = value.StartsWith(ScriptSyntax.SessionKeyword + ".");

            if (isSessionProperty)
            {
            }
            else
            {
            }

            var tokens = value.Split('.');

            // Session.Property
            if (tokens[0] == ScriptSyntax.SessionKeyword && tokens.Length == 2)
            {
                Instance = statement.Script.Session;
                property = statement.Script.Session.ScriptEnvironment.GetSessionProperty(tokens[1]);
            }

            // Session.Entity.Property
            else if (tokens[0] == ScriptSyntax.SessionKeyword && tokens.Length == 3)
            {
                if (statement.Script.Session.ScriptEnvironment.GetSessionProperty(tokens[1]) is ScriptProperty entityProperty)
                {
                    property = statement.Script.Session.ScriptEnvironment.GetProperty(entityProperty.PropertyType, tokens[2]);
                    Instance = entityProperty.GetValue(statement.Script.Session);
                }
            }

            // Entity.Property
            else
            {
                if (statement.Script.Session.GetEntity<Entity>(tokens[0]) is Entity entity)
                {
                    Instance = entity;
                    property = statement.Script.Session.ScriptEnvironment.GetProperty(Instance.GetType(), tokens[1]);
                }
            }

            if (property == null)
                throw new ScriptException($"'{value}' might be referring to an unregistered class or is not a valid property reference expression.");
        }

        // Instance
        public object? Instance { get; }

        // Property
        public ScriptProperty Property => property ?? throw new InvalidOperationException();
    }
}
