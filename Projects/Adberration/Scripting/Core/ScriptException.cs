using System;

namespace Adberration.Scripting
{
    /// <summary>
    /// ScriptException
    /// </summary>
    [Serializable]
    public sealed class ScriptException : SystemException
    {
        // Constructor
        public ScriptException(string? message)
            : base(message)
        {
            this.ErrorMessage = message ?? string.Empty;
        }

        // Constructor
        public ScriptException(string? message, Exception innerException)
            : base(message, innerException)
        {
            this.ErrorMessage = message ?? string.Empty;
        }

        // Constructor
        public ScriptException(Script script, string message)
            : base(ComposeMessage(script, message))
        {
            this.ErrorMessage = message ?? string.Empty;
        }

        // Constructor
        public ScriptException(Script script, string sourceLine, string message)
            : base(ComposeMessage(script, sourceLine, message))
        {
            this.ErrorMessage = message ?? string.Empty;
        }

        // Constructor
        public ScriptException(Statement statement, string message)
            : base(ComposeMessage(statement, message))
        {
            this.ErrorMessage = message ?? string.Empty;
        }

        // ComposeMessage
        private static string ComposeMessage(Script script, string message)
        {
            return $"Script: {script.Signature}\nError: {message}";
        }

        // ComposeMessage
        private static string ComposeMessage(Script script, string sourceLine, string message)
        {
            return $"Script: {script.Signature}\nSource Line: {sourceLine}\nError: {message}";
        }

        // ComposeMessage
        private static string ComposeMessage(Statement statement, string message)
        {
            return $"Script: {statement.Script.Signature}\nSource Line: {statement.Source}\nError: {message}";
        }

        // ErrorMessage
        public string ErrorMessage { get; private set; }

        // GetMissingArgValueMessage
        public static string GetMissingArgValueMessage(string argName)
        {
            return $"Argument [{argName}] requires a value.";
        }
    }
}
