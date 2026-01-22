using Microsoft.Xna.Framework;
using System;
using System.Text.Json;

namespace Engendro
{
    /// <summary>
    /// JsonExtensions
    /// </summary>
    public static class JsonExtensions
    {
        extension(JsonElement element)
        {
            // GetBool
            public bool? GetBool(string propertyName)
            {
                if (element.TryGetProperty(propertyName, out JsonElement prop))
                    return prop.GetBoolean();

                return null;
            }

            // GetBool
            public bool GetBool(string propertyName, bool defaultValue)
            {
                if (element.TryGetProperty(propertyName, out JsonElement prop))
                    return prop.GetBoolean();

                return defaultValue;
            }

            // GetEnum
            public TEnum? GetEnum<TEnum>(string propertyName) where TEnum : struct, Enum
            {
                if (element.TryGetProperty(propertyName, out JsonElement prop) && prop.GetString() is string value)
                    return Enum.Parse<TEnum>(value);

                return null;
            }

            // GetEnum
            public TEnum GetEnum<TEnum>(string propertyName, TEnum defaultValue) where TEnum : struct, Enum
            {
                if (element.TryGetProperty(propertyName, out JsonElement prop) && prop.GetString() is string value)
                {
                    if (Enum.TryParse<TEnum>(value, false, out var result))
                        return result;
                }

                return defaultValue;
            }

            // GetFloat
            public float? GetFloat(string propertyName)
            {
                if (element.TryGetProperty(propertyName, out JsonElement prop))
                    return prop.GetSingle();
                else
                    return null;
            }

            // GetFloat
            public float GetFloat(string propertyName, float defaultValue)
            {
                if (element.TryGetProperty(propertyName, out JsonElement prop))
                    return prop.GetSingle();

                return defaultValue;
            }

            // GetInt32
            public int? GetInt32(string propertyName)
            {
                if (element.TryGetProperty(propertyName, out JsonElement prop))
                    return prop.GetInt32();
                else
                    return null;
            }

            // GetInt32
            public int GetInt32(string propertyName, int defaultValue)
            {
                if (element.TryGetProperty(propertyName, out JsonElement prop))
                    return prop.GetInt32();

                return defaultValue;
            }

            // GetObject
            public TObject? GetObject<TObject>(string propertyName, Func<string, TObject> parser) where TObject : class
            {
                if (element.TryGetProperty(propertyName, out JsonElement prop) && prop.GetString() is string value)
                    return parser(value);

                return null;
            }

            // GetString
            public string GetString(string propertyName, string defaultValue = "")
            {
                if (element.TryGetProperty(propertyName, out JsonElement prop))
                    return prop.GetString() ?? defaultValue;

                return defaultValue;
            }

            // GetVector2
            public Vector2? GetVector2(string propertyName)            
            {
                if (element.TryGetProperty(propertyName, out JsonElement prop) && prop.GetString() is string value)
                    return DataConvert.ToVector2(value);
                else
                    return null;
            }

            // GetVector2
            public Vector2 GetVector2(string propertyName, Vector2 defaultValue)
            {
                if (element.TryGetProperty(propertyName, out JsonElement prop) && prop.GetString() is string value)
                    return DataConvert.ToVector2(value);

                return defaultValue;
            }
        }
    }
}