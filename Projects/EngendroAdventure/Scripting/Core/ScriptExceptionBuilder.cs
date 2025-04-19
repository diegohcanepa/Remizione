using System;

namespace EngendroAdventure.Scripting
{
    /// <summary>
    /// ScriptExceptionBuilder
    /// </summary>
    public static class ScriptExceptionBuilder
    {
        // AnimationNotActive
        public static ScriptException AnimationNotActive(Statement statement)
        {
            return new(statement, $"No active animation. You must use the animation command first.");
        }

        // AssetNotFound
        public static ScriptException AssetNotFound(Statement statement, string assetName)
        {
            return new(statement, $"The asset '{assetName}' does not exist.");
        }

        // EndlessLoop
        public static ScriptException EndlessLoop(Statement statement)
        {
            return new(statement, "Endless loop condition detected.");
        }

        // InvalidString
        public static ScriptException InvalidString(Statement statement, string value)
        {
            return new(statement, $"'{value}' is not a valid string.");
        }

        // InvalidValue
        public static ScriptException InvalidValue(Statement statement, string value)
        {
            return new(statement, $"'{value}' is not a valid value.");
        }

        // RoomNotFound
        public static ScriptException RoomNotFound(Statement statement, string roomName)
        {
            return new(statement, $"Room '{roomName}' does not exist.");
        }

        // ScriptNotFound
        public static ScriptException ScriptNotFound(Statement statement, string scriptName)
        {
            return new(statement, $"Script '{scriptName}' does not exist.");
        }

        // SoundNotFound
        public static ScriptException SoundNotFound(Statement statement, string name)
        {
            return new(statement, $"Sound '{name}' does not exist.");
        }

        // ThingNotFound
        public static ScriptException ThingNotFound(Statement statement, string name)
        {
            return new(statement, $"Thing '{name}' does not exist.");
        }

        // UndeclaredFlag
        public static ScriptException UndeclaredFlag(Statement statement, string flag)
        {
            return new(statement, $"Flag '{flag}' is not declared.");
        }

        // UndeclaredTag
        public static ScriptException UndeclaredTag(Statement statement, string tag)
        {
            return new(statement, $"Tag '{tag}' is not declared.");
        }

        // UnrecognizedConditionalOperator
        public static ScriptException UnrecognizedConditionalOperator(Statement statement, string op)
        {
            return new(statement, $"'{op}' is not a valid conditional operator.");
        }

        // UnrecognizedEntity
        public static ScriptException UnrecognizedEntity(Statement statement, string entityName)
        {
            return new(statement, $"Entity '{entityName}' does not exist.");
        }

        // ValueParseError
        public static ScriptException ValueParseError(Statement statement, string? value, Type expectedType)
        {
            value ??= string.Empty;
            return new(statement, $"'{value}' cannot be parsed as '{expectedType.Name}' value.");
        }

        // ValueOutOfRange
        public static ScriptException ValueOutOfRange(Statement statement, float min, float max)
        {
            return new(statement, $"Value must be between {min} and {max}.");
        }
    }
}
