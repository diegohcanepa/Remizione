using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Adberration.Scripting
{
    /// <summary>
    /// Parser
    /// </summary>
    public static class Parser
    {
        #region Private members

        // AssertClauseIndexArguments
        private static void AssertClauseIndexArguments(Statement statement, int clauseIndex)
        {
            if (clauseIndex < 0 || clauseIndex >= statement.Body.Clauses.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(clauseIndex));
            }
        }

        #endregion

        // ParseArgumentValue
        public static string? ParseArgumentValue(Statement statement, string argName)
        {
            string? result = null;

            if (statement.Body.Args.FindArg(argName) is StatementArg arg)
            {
                if (string.IsNullOrWhiteSpace(arg.Value))
                    throw new ScriptException(statement, ScriptException.GetMissingArgValueMessage(argName));

                result = arg.Value;
            }

            return result;
        }

        // ParseBoolean
        public static bool ParseBoolean(Statement statement, int clauseIndex)
        {
            AssertClauseIndexArguments(statement, clauseIndex);
            return ParseBoolean(statement, statement.Body.Clauses[clauseIndex]);
        }

        // ParseBoolean
        public static bool ParseBoolean(Statement statement, string value)
        {
            if (!bool.TryParse(value, out var result))
                throw ScriptExceptionBuilder.ValueParseError(statement, value, typeof(bool));

            return result;
        }

        // ParseBooleanArgument
        public static bool ParseBooleanArgument(Statement statement, string argName)
        {
            return ParseBooleanArgument(statement, argName, false);
        }

        // ParseBooleanArgument
        public static bool ParseBooleanArgument(Statement statement, string argName, bool defaultValue)
        {
            var result = defaultValue;
            if (ParseArgumentValue(statement, argName) is string value)
            {
                result = ParseBoolean(statement, value);
            }

            return result;
        }

        // ParseColor
        public static Color ParseColor(Statement statement, int clauseIndex)
        {
            AssertClauseIndexArguments(statement, clauseIndex);
            return ParseColor(statement, statement.Body.Clauses[clauseIndex]);
        }

        // ParseColor
        public static Color ParseColor(Statement statement, string value)
        {
            const string blackColor = "Black";
            const string transparentColor = "Transparent";
            const string whiteColor = "White";

            if (value == blackColor)
                return Color.White;
            else if (value == whiteColor)
                return Color.White;
            else if (value == transparentColor)
                return Color.Transparent;

            var values = value.Split(',');

            if (!values.Length.IsBetween(3, 4))
                throw ScriptExceptionBuilder.ValueParseError(statement, value, typeof(Color));

            var r = ParseInt32(statement, values[0]);
            var g = ParseInt32(statement, values[1]);
            var b = ParseInt32(statement, values[2]);
            float a = values.Length == 4 ? ParseInt32(statement, values[3]) : 255;

            return new Color(r, g, b) * (a / 255);
        }

        // ParseColorArgument
        public static Color ParseColorArgument(Statement statement, string argName)
        {
            return ParseColorArgument(statement, argName, Color.White);
        }

        // ParseColorArgument
        public static Color ParseColorArgument(Statement statement, string argName, Color defaultValue)
        {
            var result = defaultValue;
            if (ParseArgumentValue(statement, argName) is string value)
                result = ParseColor(statement, value);

            return result;
        }

        // ParseComparisonOperator
        public static ComparisonOperator ParseComparisonOperator(Statement statement, int clauseIndex)
        {
            return ParseComparisonOperator(statement, statement.Body.Clauses[clauseIndex]);
        }

        // ParseComparisonOperator
        public static ComparisonOperator ParseComparisonOperator(Statement statement, string value)
        {
            if (value == ScriptSyntax.EqualityOp)
                return ComparisonOperator.Equality;

            if (value == ScriptSyntax.InequalityOp)
                return ComparisonOperator.Inequality;

            if (value == ScriptSyntax.LessOp)
                return ComparisonOperator.LessThan;

            if (value == ScriptSyntax.LessThanOrEqualOp)
                return ComparisonOperator.LessThanOrEqual;

            if (value == ScriptSyntax.GreaterThanOp)
                return ComparisonOperator.GreaterThan;

            if (value == ScriptSyntax.GreaterThanOrEqualOp)
                return ComparisonOperator.GreaterThanOrEqual;

            throw ScriptExceptionBuilder.UnrecognizedConditionalOperator(statement, value);
        }

        // ParseDiceExpression
        public static DiceExpression ParseDiceExpression(Statement statement, string expression)
        {
            if (DiceExpression.TryParse(expression, out var diceExpression) && diceExpression != null)
                return diceExpression;
            else
                throw new ScriptException(statement, $"'{diceExpression}' is not a valid dice expression.");
        }

        // ParseDiceExpressionArgument
        public static DiceExpression? ParseDiceExpressionArgument(Statement statement, string argName)
        {
            var expresion = ParseArgumentValue(statement, argName) ?? string.Empty;

            if (expresion.Length > 0)
                expresion = RemoveQuotes(expresion);
            else
                return null;

            return ParseDiceExpression(statement, expresion);
        }

        // ParseEntities
        public static T[] ParseEntities<T>(Statement statement, int clauseIndex) where T : Entity
        {
            AssertClauseIndexArguments(statement, clauseIndex);
            return ParseEntities<T>(statement, statement.Body.Clauses[clauseIndex]);
        }

        // ParseEntities
        public static T[] ParseEntities<T>(Statement statement, string value) where T : Entity
        {
            var targets = value.Split(',');
            List<T> result = [];

            for (var i = 0; i < targets.Length; i++)
            {
                if (ParseEntity<T>(statement, targets[i]) is T entity)
                    result.Add(entity);
            }

            return [.. result];
        }

        // ParseEntitiesArgument
        public static T[] ParseEntitiesArgument<T>(Statement statement, string argName, T[] defaultValue) where T : Entity
        {
            var result = defaultValue;

            if (statement.Body.Args.FindArg(argName) is StatementArg arg && arg.Value != null)
            {
                if (!arg.HasValue)
                    throw new ScriptException(statement, ScriptException.GetMissingArgValueMessage(argName));

                result = ParseEntities<T>(statement, arg.Value);
            }

            return result;
        }

        // ParseEntity
        public static T? ParseEntity<T>(Statement statement, int clauseIndex) where T : Entity
        {
            AssertClauseIndexArguments(statement, clauseIndex);
            return ParseEntity<T>(statement, statement.Body.Clauses[clauseIndex]);
        }

        // ParseEntity
        public static T? ParseEntity<T>(Statement statement, string name) where T : Entity
        {
            const string sessionPrefix = "Session.";

            // 'null' keyword
            if (name == ScriptSyntax.NullValue)
                return null;

            // 'this' keyword
            if (name == ScriptSyntax.ThisKeyword)
            {
                if (!statement.Script.HasCapability(ScriptCapability.EntityContext))
                    throw new ScriptException($"The '{ScriptSyntax.ThisKeyword}' keyword is not valid in this context.");

                return statement.Session.FindEntity<T>(statement.Script.EntityName);
            }

            // $Property
            if (name.StartsWith(ScriptSyntax.SessionPropertyAlias, StringComparison.Ordinal))
                name = name.Replace(ScriptSyntax.SessionPropertyAlias, sessionPrefix);

            // Session property
            if (name.StartsWith(sessionPrefix, StringComparison.Ordinal))
            {
                name = name.Substring(sessionPrefix.Length);

                if (statement.Session.ScriptEnvironment.FindSessionProperty(name) is ScriptProperty sessionProperty)
                {
                    if (!typeof(Entity).IsAssignableFrom(sessionProperty.PropertyType))
                        throw new ScriptException($"'{name}' must be an entity type.");

                    return sessionProperty.GetValue(statement.Session) as T;
                }
                else
                {
                    throw new ScriptException($"'{name}' is not a valid session property.");
                }
            }

            var result = statement.Session.FindEntity<T>(name) ?? throw ScriptExceptionBuilder.UnrecognizedEntity(statement, name);
            return result;
        }

        // ParseEntityArgument
        public static T? ParseEntityArgument<T>(Statement statement, string argName, T? defaultValue) where T : Entity
        {
            var result = defaultValue;

            if (statement.Body.Args.FindArg(argName) is StatementArg arg && arg.Value != null)
            {
                if (!arg.HasValue)
                    throw new ScriptException(statement, ScriptException.GetMissingArgValueMessage(argName));

                result = ParseEntity<T>(statement, arg.Value);
            }

            return result;
        }

        // ParseEnum
        public static T ParseEnum<T>(Statement statement, int clauseIndex)
            where T : struct
        {
            AssertClauseIndexArguments(statement, clauseIndex);
            return ParseEnum<T>(statement, statement.Body.Clauses[clauseIndex]);
        }

        // ParseEnum
        public static T ParseEnum<T>(Statement statement, string value) where T : struct
        {
            if (!Enum.TryParse(value, out T result) || !Enum.IsDefined(typeof(T), result))
                throw ScriptExceptionBuilder.ValueParseError(statement, value, typeof(T));

            return result;
        }

        // ParseEnumArgument
        public static T ParseEnumArgument<T>(Statement statement, string argName) where T : struct
        {
            return ParseEnumArgument(statement, argName, default(T));
        }

        // ParseEnumArgument
        public static T ParseEnumArgument<T>(Statement statement, string argName, T defaultValue)
            where T : struct
        {
            var result = defaultValue;
            if (ParseArgumentValue(statement, argName) is string value)
                result = ParseEnum<T>(statement, value);

            return result;
        }

        // ParseEnums
        public static T[] ParseEnums<T>(Statement statement, int clauseIndex) where T : struct
        {
            return ParseEnums<T>(statement, statement.Body.Clauses[clauseIndex]);
        }

        // ParseEnums
        public static T[] ParseEnums<T>(Statement statement, string value) where T : struct
        {
            var values = value.Split(',');
            T[] result = new T[values.Length];

            for (var i = 0; i < values.Length; i++)
            {
                result[i] = ParseEnum<T>(statement, values[i]);
            }

            return result;
        }

        // ParseFlags
        public static string[] ParseFlags(Statement statement, string value)
        {
            var names = value.Split(',');
            for (var i = 0; i < names.Length; i++)
            {
                var name = names[i];
                if (name.StartsWith(ScriptSyntax.LogicalNegation, StringComparison.Ordinal))
                    name = name.Substring(1);

                if (!statement.Session.ScriptEnvironment.IsFlagDeclared(name))
                    throw ScriptExceptionBuilder.UndeclaredFlag(statement, name);

                ParseName(statement, name);
            }

            return names;
        }

        // ParseFlagCondition
        public static FlagCondition? ParseFlagCondition(Statement statement, int clauseIndex)
        {
            AssertClauseIndexArguments(statement, clauseIndex);
            return ParseFlagCondition(statement, statement.Body.Clauses[clauseIndex]);
        }

        // ParseFlagCondition
        public static FlagCondition? ParseFlagCondition(Statement statement, string value)
        {
            var flags = ParseFlags(statement, value);
            return flags != null && flags.Length > 0 ? statement.Session.CreateFlagCondition(flags) : null;
        }

        // ParseFlagConditionArgument
        public static FlagCondition? ParseFlagConditionArgument(Statement statement, string argName)
        {
            var flags = ParseFlagsArgument(statement, argName);
            return flags != null && flags.Length > 0 ? statement.Session.CreateFlagCondition(flags) : null;
        }

        // ParseFlagsArgument
        public static string[]? ParseFlagsArgument(Statement statement, string argName)
        {
            string[]? result = null;
            if (ParseArgumentValue(statement, argName) is string value)
                result = ParseFlags(statement, value);

            return result;
        }

        // ParseFloat
        public static float ParseFloat(Statement statement, int clauseIndex)
        {
            AssertClauseIndexArguments(statement, clauseIndex);
            return ParseFloat(statement, statement.Body.Clauses[clauseIndex]);
        }

        // ParseFloat
        public static float ParseFloat(Statement statement, string value)
        {
            if (!float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
                throw ScriptExceptionBuilder.ValueParseError(statement, value, typeof(float));

            return result;
        }

        // ParseFloatArgument
        public static float ParseFloatArgument(Statement statement, string argName)
        {
            return ParseFloatArgument(statement, argName, 0);
        }

        // ParseFloatArgument
        public static float ParseFloatArgument(Statement statement, string argName, float defaultValue)
        {
            var result = defaultValue;
            if (ParseArgumentValue(statement, argName) is string value)
                result = ParseFloat(statement, value);

            return result;
        }

        // ParseFloatArray
        public static float[] ParseFloatArray(Statement statement, int clauseIndex)
        {
            AssertClauseIndexArguments(statement, clauseIndex);
            return ParseFloatArray(statement, statement.Body.Clauses[clauseIndex]);
        }

        // ParseFloatArray
        public static float[] ParseFloatArray(Statement statement, string value)
        {
            var values = value.Split(',');
            var numbers = new float[values.Length];
            for (var i = 0; i < values.Length; i++)
            {
                numbers[i] = ParseFloat(statement, values[i]);
            }

            return numbers;
        }

        // ParseFloatRange
        public static FloatRange ParseFloatRange(Statement statement, int clauseIndex)
        {
            AssertClauseIndexArguments(statement, clauseIndex);
            return ParseFloatRange(statement, statement.Body.Clauses[clauseIndex]);
        }

        // ParseFloatRange
        public static FloatRange ParseFloatRange(Statement statement, string value)
        {
            if (!FloatRange.TryParse(value, out var result))
                throw ScriptExceptionBuilder.ValueParseError(statement, value, typeof(FloatRange));

            return result;
        }

        // ParseFloatRangeArgument
        public static FloatRange ParseFloatRangeArgument(Statement statement, string argName)
        {
            return ParseFloatRangeArgument(statement, argName, FloatRange.Empty);
        }

        // ParseFloatRangeArgument
        public static FloatRange ParseFloatRangeArgument(Statement statement, string argName, FloatRange defaultValue)
        {
            var result = defaultValue;

            if (ParseArgumentValue(statement, argName) is string value)
                result = ParseFloatRange(statement, value);

            return result;
        }

        // ParseImageArgument
        public static AtlasImage? ParseImageArgument(Statement statement, string argName, Atlas atlas)
        {
            AtlasImage? result = null;

            if (statement.Body.Args.FindArg(argName) is StatementArg arg && arg.Value != null)
            {
                if (!arg.HasValue)
                    throw new ScriptException(statement, ScriptException.GetMissingArgValueMessage(argName));

                result = atlas.FindImage(arg.Value);

                if (result == null)
                    throw ScriptExceptionBuilder.AssetNotFound(statement, arg.Value);
            }

            return result;
        }

        // ParseInputBinding
        public static InputBinding ParseInputBinding(Statement statement, int clauseIndex)
        {
            AssertClauseIndexArguments(statement, clauseIndex);
            return ParseInputBinding(statement, statement.Body.Clauses[clauseIndex]);
        }

        // ParseInputBinding
        public static InputBinding ParseInputBinding(Statement statement, string value)
        {
            var result = InputManager.FindBinding(value) ?? throw ScriptExceptionBuilder.ValueParseError(statement, value, typeof(InputBinding));
            return result;
        }

        // ParseInt32
        public static int ParseInt32(Statement statement, int clauseIndex)
        {
            AssertClauseIndexArguments(statement, clauseIndex);
            return ParseInt32(statement, statement.Body.Clauses[clauseIndex]);
        }

        // ParseInt32
        public static int ParseInt32(Statement statement, string value)
        {
            if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
                throw ScriptExceptionBuilder.ValueParseError(statement, value, typeof(int));

            return result;
        }

        // ParseInt32Argument
        public static int ParseInt32Argument(Statement statement, string argName)
        {
            return ParseInt32Argument(statement, argName, 0);
        }

        // ParseInt32Argument
        public static int ParseInt32Argument(Statement statement, string argName, int defaultValue)
        {
            var result = defaultValue;

            if (ParseArgumentValue(statement, argName) is string value)
                result = ParseInt32(statement, value);

            return result;
        }

        // ParseInt32Array
        public static int[] ParseInt32Array(Statement statement, int clauseIndex)
        {
            AssertClauseIndexArguments(statement, clauseIndex);
            return ParseInt32Array(statement, statement.Body.Clauses[clauseIndex]);
        }

        // ParseInt32Array
        public static int[] ParseInt32Array(Statement statement, string value)
        {
            var values = value.Split(',');
            var numbers = new int[values.Length];
            for (var i = 0; i < values.Length; i++)
            {
                numbers[i] = ParseInt32(statement, values[i]);
            }

            return numbers;
        }

        // ParseInt32ArrayArgument
        public static int[] ParseInt32ArrayArgument(Statement statement, string argName)
        {
            var result = Array.Empty<int>();
            if (ParseArgumentValue(statement, argName) is string value)
                result = ParseInt32Array(statement, value);

            return result;
        }

        // ParseInt32Range
        public static Int32Range ParseInt32Range(Statement statement, int clauseIndex)
        {
            AssertClauseIndexArguments(statement, clauseIndex);
            return ParseInt32Range(statement, statement.Body.Clauses[clauseIndex]);
        }

        // ParseInt32Range
        public static Int32Range ParseInt32Range(Statement statement, string value)
        {
            if (!Int32Range.TryParse(value, out var result))
                throw ScriptExceptionBuilder.ValueParseError(statement, value, typeof(Int32Range));

            return result;
        }

        // ParseInt32RangeArgument
        public static Int32Range ParseInt32RangeArgument(Statement statement, string argName)
        {
            return ParseInt32RangeArgument(statement, argName, Int32Range.Empty);
        }

        // ParseInt32RangeArgument
        public static Int32Range ParseInt32RangeArgument(Statement statement, string argName, Int32Range defaultValue)
        {
            var result = defaultValue;

            if (ParseArgumentValue(statement, argName) is string value)
                result = ParseInt32Range(statement, value);

            return result;
        }

        // ParseInt64
        public static long ParseInt64(Statement statement, int clauseIndex)
        {
            AssertClauseIndexArguments(statement, clauseIndex);
            return ParseInt64(statement, statement.Body.Clauses[clauseIndex]);
        }

        // ParseInt64
        public static long ParseInt64(Statement statement, string value)
        {
            if (!long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
                throw ScriptExceptionBuilder.ValueParseError(statement, value, typeof(long));

            return result;
        }

        // ParseName
        public static string ParseName(Statement statement, int clauseIndex)
        {
            AssertClauseIndexArguments(statement, clauseIndex);
            return ParseName(statement, statement.Body.Clauses[clauseIndex]);
        }

        // ParseName
        public static string ParseName(Statement statement, string name)
        {
            var error = NameValidator.Validate(name);

            if (NameValidator.GetErrorMessage(error) is string errorMessage)
                throw new ScriptException(statement, errorMessage);

            return name;
        }

        // ParseNameArgument
        public static string? ParseNameArgument(Statement statement, string argName)
        {
            return ParseArgumentValue(statement, argName);
        }

        // ParseNames
        public static string[] ParseNames(Statement statement, int clauseIndex)
        {
            AssertClauseIndexArguments(statement, clauseIndex);
            return ParseNames(statement, statement.Body.Clauses[clauseIndex]);
        }

        // ParseNames
        public static string[] ParseNames(Statement statement, string value)
        {
            var names = value.Split(',');
            for (var i = 0; i < names.Length; i++)
            {
                ParseName(statement, names[i]);
            }

            return names;
        }

        // ParseNamesArgument
        public static string[] ParseNamesArgument(Statement statement, string argName, string defaultValue)
        {
            ParseName(statement, defaultValue);

            string[]? result = null;

            if (statement.Body.Args.FindArg(argName) is StatementArg arg && arg.Value != null)
            {
                if (!arg.HasValue)
                    throw new ScriptException(statement, ScriptException.GetMissingArgValueMessage(argName));

                result = ParseNames(statement, arg.Value);
            }

            return result == null || result.Length == 0 ? [defaultValue] : result;
        }

        // ParseQuotedString
        public static string ParseQuotedString(Statement statement, int clauseIndex)
        {
            AssertClauseIndexArguments(statement, clauseIndex);
            return ParseQuotedString(statement, statement.Body.Clauses[clauseIndex]);
        }

        // ParseQuotedString
        public static string ParseQuotedString(Statement statement, string value)
        {
            const string quote = @"""";

            if (value == ScriptSyntax.NullValue)
                return string.Empty;

            value = value.Trim();
            if (!value.StartsWith(quote, StringComparison.OrdinalIgnoreCase) || !value.EndsWith(quote, StringComparison.OrdinalIgnoreCase))
                throw ScriptExceptionBuilder.InvalidString(statement, value);

            var result = RemoveQuotes(value).Replace(ScriptSyntax.StringQuoteTag, @"""");

            return result;
        }

        // ParseQuotedStringArgument
        public static string ParseQuotedStringArgument(Statement statement, string argName)
        {
            return ParseQuotedStringArgument(statement, argName, string.Empty);
        }

        // ParseInt32Argument
        public static string ParseQuotedStringArgument(Statement statement, string argName, string defaultValue)
        {
            var result = defaultValue;

            if (ParseArgumentValue(statement, argName) is string value)
                result = ParseQuotedString(statement, value);

            return result;
        }

        // ParsePolygon
        public static Polygon ParsePolygon(Statement statement, int clauseIndex)
        {
            AssertClauseIndexArguments(statement, clauseIndex);
            return ParsePolygon(statement, statement.Body.Clauses[clauseIndex]);
        }

        // ParsePolygon
        public static Polygon ParsePolygon(Statement statement, string value)
        {
            var vertices = ParseVector2Array(statement, value);

            return new Polygon(vertices);
        }

        // ParsePosition
        public static Vector2 ParsePosition(Statement statement, int clauseIndex, Vector2 defaultValue)
        {
            AssertClauseIndexArguments(statement, clauseIndex);
            return ParsePosition(statement, statement.Body.Clauses[clauseIndex], defaultValue);
        }

        // ParsePosition
        public static Vector2 ParsePosition(Statement statement, string value, Vector2 defaultValue)
        {
            return ParseVector2(statement, value, defaultValue);
        }

        // ParseRatio
        public static float ParseRatio(Statement statement, int clauseIndex)
        {
            AssertClauseIndexArguments(statement, clauseIndex);
            return ParseRatio(statement, statement.Body.Clauses[clauseIndex]);
        }

        // ParseRatio
        public static float ParseRatio(Statement statement, string value)
        {
            if (!float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
                throw ScriptExceptionBuilder.ValueParseError(statement, value, typeof(float));

            if (!result.IsBetween(0, 1))
                ScriptExceptionBuilder.ValueOutOfRange(statement, 0, 1);

            return result;
        }

        // ParseRatioArgument
        public static float ParseRatioArgument(Statement statement, string argName)
        {
            return ParseRatioArgument(statement, argName, 0);
        }

        // ParseRatioArgument
        public static float ParseRatioArgument(Statement statement, string argName, float defaultValue)
        {
            var result = defaultValue;
            if (ParseArgumentValue(statement, argName) is string value)
                result = ParseRatio(statement, value);

            return result;
        }

        // ParseRectangle
        public static Rectangle ParseRectangle(Statement statement, int clauseIndex)
        {
            AssertClauseIndexArguments(statement, clauseIndex);
            return ParseRectangle(statement, statement.Body.Clauses[clauseIndex]);
        }

        // ParseRectangle
        public static Rectangle ParseRectangle(Statement statement, string value)
        {
            var coords = value.Split(',');
            if (coords.Length != 4)
                throw ScriptExceptionBuilder.ValueParseError(statement, value, typeof(Rectangle));

            var x = ParseInt32(statement, coords[0]);
            var y = ParseInt32(statement, coords[1]);
            var width = ParseInt32(statement, coords[2]);
            var height = ParseInt32(statement, coords[3]);

            return new Rectangle(x, y, width, height);
        }

        // ParseRectangleArgument
        public static Rectangle ParseRectangleArgument(Statement statement, string argName)
        {
            return ParseRectangleArgument(statement, argName, Rectangle.Empty);
        }

        // ParseRectangleArgument
        public static Rectangle ParseRectangleArgument(Statement statement, string argName, Rectangle defaultValue)
        {
            var result = defaultValue;
            if (ParseArgumentValue(statement, argName) is string value)
                result = ParseRectangle(statement, value);

            return result;
        }

        // ParseRectangleF
        public static RectangleF ParseRectangleF(Statement statement, int clauseIndex)
        {
            AssertClauseIndexArguments(statement, clauseIndex);
            return ParseRectangle(statement, statement.Body.Clauses[clauseIndex]);
        }

        // ParseRectangleF
        public static RectangleF ParseRectangleF(Statement statement, string value)
        {
            var coords = value.Split(',');
            if (coords.Length != 4)
                throw ScriptExceptionBuilder.ValueParseError(statement, value, typeof(RectangleF));

            var x = ParseFloat(statement, coords[0]);
            var y = ParseFloat(statement, coords[1]);
            var width = ParseFloat(statement, coords[2]);
            var height = ParseFloat(statement, coords[3]);

            return new RectangleF(x, y, width, height);
        }

        // ParseRoutine
        public static Script? ParseRoutine(Statement statement, int clauseIndex)
        {
            AssertClauseIndexArguments(statement, clauseIndex);
            return ParseRoutine(statement, statement.Body.Clauses[clauseIndex]);
        }

        // ParseRoutine
        public static Script? ParseRoutine(Statement statement, string name)
        {
            if (name == ScriptSyntax.NullValue)
                return null;

            ParseName(statement, name);

            // Get script
            if (statement.Session.ScriptLibrary.FindRoutine(name) is not Script routine)
                throw new ScriptException(statement, $"Unrecognized routine '{name}'.");

            return routine;
        }

        // ParseRoutineArgument
        public static Script? ParseRoutineArgument(Statement statement, string argName)
        {
            return ParseRoutineArgument(statement, argName, null);
        }

        // ParseRoutineArgument
        public static Script? ParseRoutineArgument(Statement statement, string argName, Script? defaultValue)
        {
            var result = defaultValue;
            if (ParseArgumentValue(statement, argName) is string value)
                result = ParseRoutine(statement, value);

            return result;
        }

        // ParseSound
        public static Sound? ParseSound(Statement statement, string name)
        {
            if (name == ScriptSyntax.NullValue)
                return null;

            Sound? result = Sound.Find(name) ?? throw ScriptExceptionBuilder.SoundNotFound(statement, name);
            return result;
        }

        // ParseSoundArgument
        public static Sound? ParseSoundArgument(Statement statement, string argName)
        {
            Sound? result = null;

            if (ParseArgumentValue(statement, argName) is string value)
                result = ParseSound(statement, value);

            return result;
        }

        // ParseStringArgument
        public static string ParseStringArgument(Statement statement, string argName)
        {
            return ParseStringArgument(statement, argName, string.Empty);
        }

        // ParseStringArgument
        public static string ParseStringArgument(Statement statement, string argName, string defaultValue)
        {
            var result = defaultValue;

            if (statement.Body.Args.FindArg(argName) is StatementArg arg && arg.Value != null)
            {
                if (!arg.HasValue)
                    throw new ScriptException(statement, ScriptException.GetMissingArgValueMessage(argName));

                result = arg.Value;
            }

            return result;
        }

        // ParseVector2
        public static Vector2 ParseVector2(Statement statement, int clauseIndex)
        {
            AssertClauseIndexArguments(statement, clauseIndex);
            return ParseVector2(statement, clauseIndex, null);
        }

        // ParseVector2
        public static Vector2 ParseVector2(Statement statement, int clauseIndex, Vector2? defaultValue)
        {
            AssertClauseIndexArguments(statement, clauseIndex);
            return ParseVector2(statement, statement.Body.Clauses[clauseIndex], defaultValue);
        }

        // ParseVector2
        public static Vector2 ParseVector2(Statement statement, string value)
        {
            return ParseVector2(statement, value, null);
        }

        // ParseVector2
        public static Vector2 ParseVector2(Statement statement, string value, Vector2? defaultValue)
        {
            var values = value.Split(',');

            float x, y;

            // X
            var xValue = values[0].Trim();
            if (xValue == "x")
                x = defaultValue.HasValue ? defaultValue.Value.X : 0;
            else
                x = ParseFloat(statement, values[0]);

            if (values.Length == 1)
                return new Vector2(x);

            // Y
            var yValue = values[1].Trim();
            if (yValue == "y")
                y = defaultValue.HasValue ? defaultValue.Value.Y : 0;
            else
                y = ParseFloat(statement, values[1]);

            return new Vector2(x, y);
        }

        // ParseVector2Array
        public static Vector2[] ParseVector2Array(Statement statement, int clauseIndex)
        {
            AssertClauseIndexArguments(statement, clauseIndex);
            return ParseVector2Array(statement, statement.Body.Clauses[clauseIndex]);
        }

        // ParseVector2Array
        public static Vector2[] ParseVector2Array(Statement statement, string value)
        {
            var points = value.Split(ScriptSyntax.ArgumentListSeparator1);
            Vector2[] waypoints = new Vector2[points.Length];
            for (var i = 0; i < waypoints.Length; i++)
            {
                waypoints[i] = ParseVector2(statement, points[i]);
            }

            return waypoints;
        }

        // ParseVector2Argument
        public static Vector2 ParseVector2Argument(Statement statement, string argName)
        {
            return ParseVector2Argument(statement, argName, Vector2.Zero);
        }

        // ParseVector2Argument
        public static Vector2 ParseVector2Argument(Statement statement, string argName, Vector2 defaultValue)
        {
            var result = defaultValue;

            if (ParseArgumentValue(statement, argName) is string value)
                result = ParseVector2(statement, value);

            return result;
        }

        // ParseMethodExpression
        public static MethodExpression ParseMethodExpression(Statement statement, string value, bool validateContext)
        {
            MethodExpression result = new(statement.Script, value);
            if (validateContext)
                ScriptEnvironment.CheckCodingContext(result.Method.Name, result.Method.Context, statement.Script.ScriptType);

            return result;
        }

        // ParsePropertyExpression
        public static PropertyExpression ParsePropertyExpression(Statement statement, string value, bool validateContext)
        {
            PropertyExpression result = new(statement, value);
            if (validateContext)
                ScriptEnvironment.CheckCodingContext(result.Property.Name, result.Property.Context, statement.Script.ScriptType);

            return result;
        }

        // RemoveQuotes
        public static string RemoveQuotes(string value)
        {
            const char quote = '"';

            value = value.Trim();
            if (value[0] == quote)
                value = value.Substring(1);

            if (value.LastIndexOf(quote) != -1)
                value = value.Substring(0, value.Length - 1);

            return value;
        }
    }
}