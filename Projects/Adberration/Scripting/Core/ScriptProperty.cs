using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using System;
using System.Reflection;

namespace Adberration.Scripting
{
    /// <summary>
    /// ScriptProperty
    /// </summary>
    public sealed class ScriptProperty : ScriptMember
    {
        #region Constructor

        // Constructor
        internal ScriptProperty(Session session, string name, PropertyInfo propertyInfo, CodingContext context)
            : base(session, name, context)
        {
            this.PropertyInfo = propertyInfo;

            var shouldCheckType = !propertyInfo.PropertyType.GetTypeInfo().IsEnum &&
                                   !typeof(Thing).IsAssignableFrom(propertyInfo.PropertyType) &&
                                   !typeof(Room).IsAssignableFrom(propertyInfo.PropertyType);

            if (shouldCheckType && !ScriptSyntax.SupportedPropertyTypes.Contains(propertyInfo.PropertyType))
            {
                throw new ArgumentException($"The type '{propertyInfo.PropertyType}' is not supported as a property.", nameof(propertyInfo));
            }
        }

        #endregion

        #region Private members

        // SetAtlasProperty
        private void SetAtlasProperty(Statement statement, object? instance, string value)
        {
            Atlas? parsedValue;
            if (value == ScriptSyntax.NullValue)
            {
                parsedValue = null;
            }
            else
            {
                parsedValue = Atlas.FindInstance(value);
                if (parsedValue is null)
                {
                    throw ScriptExceptionBuilder.AssetNotFound(statement, value);
                }
            }

            if (instance != null)
            {
                PropertyInfo.SetValue(instance, parsedValue);
            }
        }

        // SetBooleanProperty
        private void SetBooleanProperty(Statement statement, object? instance, string value)
        {
            var parsedValue = Parser.ParseBoolean(statement, value);
            if (instance != null)
            {
                PropertyInfo.SetValue(instance, parsedValue);
            }
        }

        // SetColorProperty
        private void SetColorProperty(Statement statement, object? instance, string value)
        {
            var parsedValue = Parser.ParseColor(statement, value);
            if (instance != null)
            {
                PropertyInfo.SetValue(instance, parsedValue);
            }
        }

        // SetDiceRollProperty
        private void SetDiceRollProperty(Statement statement, object? instance, string value)
        {
            var parsedValue = Parser.ParseQuotedString(statement, value);
            if (instance != null)
            {
                PropertyInfo.SetValue(instance, new DiceExpression(parsedValue));
            }
        }

        // SetEnumerationProperty
        private void SetEnumerationProperty(Statement statement, object? instance, string value)
        {
            var names = Enum.GetNames(PropertyInfo.PropertyType);

            var found = false;
            for (var i = 0; i < names.Length; i++)
            {
                if (names[i] == value)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                throw ScriptExceptionBuilder.InvalidValue(statement, value);
            }

            var parsedValue = Enum.Parse(PropertyInfo.PropertyType, value);
            if (instance != null)
            {
                PropertyInfo.SetValue(instance, parsedValue);
            }
        }

        // SetFlagConditionProperty
        private void SetFlagConditionProperty(Statement statement, object? instance, string value)
        {
            FlagCondition? parsedValue;
            if (value == ScriptSyntax.NullValue)
            {
                parsedValue = null;
            }
            else
            {
                parsedValue = Parser.ParseFlagCondition(statement, value);
            }

            if (instance != null)
            {
                PropertyInfo.SetValue(instance, parsedValue);
            }
        }

        // SetFloatProperty
        private void SetFloatProperty(Statement statement, object? instance, string value)
        {
            var parsedValue = Parser.ParseFloat(statement, value);
            if (instance != null)
            {
                PropertyInfo.SetValue(instance, parsedValue);
            }
        }

        // SetInt32Property
        private void SetInt32Property(Statement statement, object? instance, string value)
        {
            var parsedValue = Parser.ParseInt32(statement, value);
            if (instance != null)
            {
                PropertyInfo.SetValue(instance, parsedValue);
            }
        }

        // SetInt32RangeProperty
        private void SetInt32RangeProperty(Statement statement, object? instance, string value)
        {
            var parsedValue = Parser.ParseInt32Range(statement, value);
            if (instance != null)
            {
                PropertyInfo.SetValue(instance, parsedValue);
            }
        }

        // SetInt64Property
        private void SetInt64Property(Statement statement, object? instance, string value)
        {
            var parsedValue = Parser.ParseInt64(statement, value);
            if (instance != null)
            {
                PropertyInfo.SetValue(instance, parsedValue);
            }
        }

        // SetPolygonProperty
        private void SetPolygonProperty(Statement statement, object? instance, string value)
        {
            var parsedValue = Parser.ParsePolygon(statement, value);
            if (instance != null)
            {
                PropertyInfo.SetValue(instance, parsedValue);
            }
        }

        // SetRectangleProperty
        private void SetRectangleProperty(Statement statement, object? instance, string value)
        {
            var parsedValue = Parser.ParseRectangle(statement, value);
            if (instance != null)
            {
                PropertyInfo.SetValue(instance, parsedValue);
            }
        }

        // SetRectangleFProperty
        private void SetRectangleFProperty(Statement statement, object? instance, string value)
        {
            var parsedValue = Parser.ParseRectangleF(statement, value);
            if (instance != null)
            {
                PropertyInfo.SetValue(instance, parsedValue);
            }
        }

        // SetRoomProperty
        private void SetRoomProperty(Statement statement, object? instance, string value)
        {
            if (value == ScriptSyntax.NullValue)
            {
                if (instance != null)
                {
                    PropertyInfo.SetValue(instance, null);
                }
            }
            else
            {
                var room = Session.FindEntity<Room>(value) ?? throw ScriptExceptionBuilder.RoomNotFound(statement, value);
                if (instance != null)
                    PropertyInfo.SetValue(instance, room);
            }
        }

        // SetScriptProperty
        private void SetScriptProperty(Statement statement, object? instance, string value)
        {
            if (value == ScriptSyntax.NullValue)
            {
                if (instance != null)
                {
                    PropertyInfo.SetValue(instance, null);
                }
            }
            else
            {
                var routine = Session.ScriptLibrary.FindRoutine(value) ?? throw ScriptExceptionBuilder.ScriptNotFound(statement, value);
                if (instance != null)
                    PropertyInfo.SetValue(instance, routine);
            }
        }

        // SetSoundProperty
        private void SetSoundProperty(Statement statement, object? instance, string value)
        {
            var parsedValue = Parser.ParseSound(statement, value);
            if (instance != null)
                PropertyInfo.SetValue(instance, parsedValue);
        }

        // SetStringProperty
        private void SetStringProperty(Statement statement, object? instance, string value)
        {
            // QuotedString
            if (value.StartsWith(@"""", StringComparison.OrdinalIgnoreCase))
            {
                var parsedValue = Parser.ParseQuotedString(statement, value);
                if (instance != null)
                {
                    PropertyInfo.SetValue(instance, parsedValue);
                }
            }
            else
            {
                throw ScriptExceptionBuilder.InvalidString(statement, value);
            }
        }

        // SetThingProperty
        private void SetThingProperty(Statement statement, object? instance, string value)
        {
            if (value == ScriptSyntax.NullValue)
            {
                if (instance != null)
                {
                    PropertyInfo.SetValue(instance, null);
                }
            }
            else
            {
                if (ScriptSyntax.IsClonedName(value) && Session.State == GameSessionState.LoadingScripts)
                {
                    throw new ScriptException(statement, "Dynamic entities cannot be assigned during initialization.");
                }

                var thing = Session.FindEntity<Thing>(value) ?? throw ScriptExceptionBuilder.ThingNotFound(statement, value);
                if (instance != null)
                    PropertyInfo.SetValue(instance, thing);
            }
        }

        // SetValueCore
        private void SetValueCore(Statement statement, object? instance, string value)
        {
            // Atlas
            if (PropertyInfo.PropertyType == typeof(Atlas))
            {
                SetAtlasProperty(statement, instance, value);
            }

            // Boolean
            else if (PropertyInfo.PropertyType == typeof(bool))
            {
                SetBooleanProperty(statement, instance, value);
            }

            // Color
            else if (PropertyInfo.PropertyType == typeof(Color))
            {
                SetColorProperty(statement, instance, value);
            }

            // DiceRoll
            else if (PropertyInfo.PropertyType == typeof(DiceExpression))
            {
                SetDiceRollProperty(statement, instance, value);
            }

            // Enumeration
            else if (typeof(Enum).IsAssignableFrom(PropertyInfo.PropertyType))
            {
                SetEnumerationProperty(statement, instance, value);
            }

            // FlagCondition
            else if (PropertyInfo.PropertyType == typeof(FlagCondition))
            {
                SetFlagConditionProperty(statement, instance, value);
            }

            // Float
            else if (PropertyInfo.PropertyType == typeof(float))
            {
                SetFloatProperty(statement, instance, value);
            }

            // Int32
            else if (PropertyInfo.PropertyType == typeof(int))
            {
                SetInt32Property(statement, instance, value);
            }

            // Int32Range
            else if (PropertyInfo.PropertyType == typeof(Int32Range))
            {
                SetInt32RangeProperty(statement, instance, value);
            }

            // Int64
            else if (PropertyInfo.PropertyType == typeof(long))
            {
                SetInt64Property(statement, instance, value);
            }

            // Polygon
            else if (PropertyInfo.PropertyType == typeof(Polygon))
            {
                SetPolygonProperty(statement, instance, value);
            }

            // Rectangle
            else if (PropertyInfo.PropertyType == typeof(Rectangle))
            {
                SetRectangleProperty(statement, instance, value);
            }

            // RectangleF
            else if (PropertyInfo.PropertyType == typeof(RectangleF))
            {
                SetRectangleFProperty(statement, instance, value);
            }

            // Room
            else if (typeof(Room).IsAssignableFrom(PropertyInfo.PropertyType))
            {
                SetRoomProperty(statement, instance, value);
            }

            // Script
            else if (PropertyInfo.PropertyType == typeof(Script))
            {
                SetScriptProperty(statement, instance, value);
            }

            // Sound
            else if (PropertyInfo.PropertyType == typeof(Sound))
            {
                SetSoundProperty(statement, instance, value);
            }

            // String
            else if (PropertyInfo.PropertyType == typeof(string))
            {
                SetStringProperty(statement, instance, value);
            }

            // Thing
            else if (typeof(Thing).IsAssignableFrom(PropertyInfo.PropertyType))
            {
                SetThingProperty(statement, instance, value);
            }

            // Vector2
            else if (PropertyInfo.PropertyType == typeof(Vector2))
            {
                SetVector2Property(statement, instance, value);
            }
        }

        // SetVector2Property
        private void SetVector2Property(Statement statement, object? instance, string value)
        {
            var parsedValue = Parser.ParseVector2(statement, value);
            if (instance != null)
            {
                PropertyInfo.SetValue(instance, parsedValue);
            }
        }

        #endregion

        // GetValue
        public object? GetValue(object obj)
        {
            return PropertyInfo.GetValue(obj);
        }

        // IsBoolean
        public bool IsBoolean => PropertyType == typeof(bool);

        // IsEntity
        public bool IsEntity => typeof(Entity).IsAssignableFrom(PropertyType);

        // IsEnum
        public bool IsEnum => PropertyType.GetTypeInfo().IsEnum;

        // IsNumeric
        public bool IsNumeric => ScriptSyntax.SupportedNumericTypes.Contains(PropertyType);

        // IsString
        public bool IsString => PropertyType == typeof(string);

        // PropertyInfo
        public PropertyInfo PropertyInfo { get; }

        // PropertyType
        public Type PropertyType => PropertyInfo.PropertyType;

        // SetFakeValue
        public void SetFakeValue(Statement statement, string value)
        {
            SetValueCore(statement, null, value);
        }

        // SetValue
        public void SetValue(Statement statement, object instance, string value)
        {
            SetValueCore(statement, instance, value);
        }
    }
}
