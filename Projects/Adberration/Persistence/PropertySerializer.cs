using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Reflection;
using System.Xml;

namespace Adberration
{
    /// <summary>
    /// PropertySerializer
    /// </summary>
    internal static class PropertySerializer
    {
        // SetValue
        internal static void SetValue(PropertyInfo propertyInfo, Entity entity, string? value)
        {
            if (!propertyInfo.CanWrite)
                return;

            // Bool
            if (propertyInfo.PropertyType == typeof(bool))
            {
                propertyInfo.SetValue(entity, value != null && XmlConvert.ToBoolean(value));
                return;
            }

            // Color
            if (propertyInfo.PropertyType == typeof(Color))
            {
                propertyInfo.SetValue(entity, value == null ? Sprite.DefaultColor : XmlConverterExtension.ToColor(value));
                return;
            }

            // Entity
            if (typeof(Entity).IsAssignableFrom(propertyInfo.PropertyType))
            {
                var entityValue = value == null ? null : entity.Session.GetEntity(value);
                propertyInfo.SetValue(entity, entityValue);
                return;
            }

            // Enumeration
            if (typeof(Enum).IsAssignableFrom(propertyInfo.PropertyType))
            {
                var enumValue = value ?? "0";
                propertyInfo.SetValue(entity, Enum.Parse(propertyInfo.PropertyType, enumValue));
                return;
            }

            // Int32
            if (propertyInfo.PropertyType == typeof(int))
            {
                propertyInfo.SetValue(entity, value == null ? 0 : XmlConvert.ToInt32(value));
                return;
            }

            // Int64
            if (propertyInfo.PropertyType == typeof(long))
            {
                propertyInfo.SetValue(entity, value == null ? 0 : XmlConvert.ToInt64(value));
                return;
            }

            // Polygon
            if (propertyInfo.PropertyType == typeof(Polygon))
            {
                propertyInfo.SetValue(entity, value == null ? null : XmlConverterExtension.ToPolygon(value));
                return;
            }

            // Rectangle
            if (propertyInfo.PropertyType == typeof(Rectangle))
            {
                propertyInfo.SetValue(entity, value == null ? Rectangle.Empty : XmlConverterExtension.ToRectangle(value));
                return;
            }

            // RectangleF
            if (propertyInfo.PropertyType == typeof(RectangleF))
            {
                propertyInfo.SetValue(entity, value == null ? RectangleF.Empty : XmlConverterExtension.ToRectangleF(value));
                return;
            }

            // Single
            if (propertyInfo.PropertyType == typeof(float))
            {
                propertyInfo.SetValue(entity, value == null ? 0 : XmlConvert.ToSingle(value));
                return;
            }

            // Size
            if (propertyInfo.PropertyType == typeof(Size))
            {
                propertyInfo.SetValue(entity, value == null ? Size.Empty : XmlConverterExtension.ToSize(value));
                return;
            }

            // String
            if (propertyInfo.PropertyType == typeof(string))
            {
                propertyInfo.SetValue(entity, value);
                return;
            }

            // TimeSpan
            if (propertyInfo.PropertyType == typeof(TimeSpan))
            {
                propertyInfo.SetValue(entity, value == null ? TimeSpan.Zero : XmlConvert.ToTimeSpan(value));
                return;
            }

            // Vector2
            if (propertyInfo.PropertyType == typeof(Vector2))
            {
                propertyInfo.SetValue(entity, value == null ? Vector2.Zero : XmlConverterExtension.ToVector2(value));
                return;
            }
        }

        // Deserialize
        internal static void Deserialize(PersistentProperty persistentProperty, Entity entity, XmlNode entityXmlNode)
        {
            if (entityXmlNode.Attributes?[persistentProperty.Name]?.Value is string attributeValue)
                SetValue(persistentProperty.PropertyInfo, entity, attributeValue);
        }

        // Serialize
        internal static void Serialize(PersistentProperty persistentProperty, object obj, XmlWriter output)
        {
            var propertyInfo = persistentProperty.PropertyInfo;
            var storageName = propertyInfo.Name;
            var propValue = propertyInfo.GetValue(obj);

            if (propValue == null)
                return;

            // Bool
            if (propertyInfo.PropertyType == typeof(bool))
            {
                if (propValue is bool value)
                    output.WriteAttributeString(storageName, XmlConvert.ToString(value));

                return;
            }

            // Color
            if (propertyInfo.PropertyType == typeof(Color))
            {
                if (propValue is Color value)
                    output.WriteAttributeString(storageName, XmlConverterExtension.ToString(value));

                return;
            }

            // Entity
            if (typeof(Entity).IsAssignableFrom(propertyInfo.PropertyType))
            {
                if (propValue is Entity entity && entity.Persistent)
                    output.WriteAttributeString(storageName, entity != null ? entity.Name : string.Empty);

                return;
            }

            // Enumeration
            if (typeof(Enum).IsAssignableFrom(propertyInfo.PropertyType))
            {
                output.WriteAttributeString(storageName, XmlConvert.ToString((int)propValue));
                return;
            }

            // Int32
            if (propertyInfo.PropertyType == typeof(int))
            {
                if (propValue is int value)
                    output.WriteAttributeString(storageName, XmlConvert.ToString(value));

                return;
            }

            // Int64
            if (propertyInfo.PropertyType == typeof(long))
            {
                if (propValue is long value)
                    output.WriteAttributeString(storageName, XmlConvert.ToString(value));

                return;
            }

            // Polygon
            if (propertyInfo.PropertyType == typeof(Polygon))
            {
                if (propValue is Polygon polygon)
                    output.WriteAttributeString(storageName, XmlConverterExtension.ToString(polygon));

                return;
            }

            // Rectangle
            if (propertyInfo.PropertyType == typeof(Rectangle))
            {
                if (propValue is Rectangle value)
                    output.WriteAttributeString(storageName, XmlConverterExtension.ToString(value));

                return;
            }

            // RectangleF
            if (propertyInfo.PropertyType == typeof(RectangleF))
            {
                if (propValue is RectangleF value)
                    output.WriteAttributeString(storageName, XmlConverterExtension.ToString(value));

                return;
            }

            // Single
            if (propertyInfo.PropertyType == typeof(float))
            {
                if (propValue is float value)
                    output.WriteAttributeString(storageName, XmlConvert.ToString(value));

                return;
            }

            // Size
            if (propertyInfo.PropertyType == typeof(Size))
            {
                if (propValue is Size value)
                    output.WriteAttributeString(storageName, XmlConverterExtension.ToString(value));

                return;
            }

            // String
            if (propertyInfo.PropertyType == typeof(string))
            {
                if (propValue is string value && value.Length > 0)
                    output.WriteAttributeString(storageName, value ?? string.Empty);

                return;
            }

            // TimeSpan
            if (propertyInfo.PropertyType == typeof(TimeSpan))
            {
                if (propValue is TimeSpan value)
                    output.WriteAttributeString(storageName, XmlConvert.ToString(value));

                return;
            }

            // Vector2
            if (propertyInfo.PropertyType == typeof(Vector2))
            {
                if (propValue is Vector2 value)
                    output.WriteAttributeString(storageName, XmlConverterExtension.ToString(value));

                return;
            }

            throw new InvalidOperationException("The property type cannot be serialized.");
        }
    }
}
