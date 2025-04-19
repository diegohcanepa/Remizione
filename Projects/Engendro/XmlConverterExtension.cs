using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Engendro
{
    /// <summary>
    /// XmlConverterExtension
    /// </summary>
    public static class XmlConverterExtension
    {
        // ToColor
        public static Color ToColor(string value)
        {
            var values = value.Split(',');

            if (values.Length < 3 || values.Length > 4)
            {
                throw new ArgumentException("The specified value is not a valid color.", nameof(value));
            }

            var r = byte.Parse(values[0], CultureInfo.InvariantCulture);
            var g = byte.Parse(values[1], CultureInfo.InvariantCulture);
            var b = byte.Parse(values[2], CultureInfo.InvariantCulture);
            var a = values.Length == 4 ? byte.Parse(values[3], CultureInfo.InvariantCulture) : 255;

            Color color = new(r, g, b, a);

            return color;
        }

        // ToPolygon
        public static Polygon ToPolygon(string value)
        {
            var values = value.Split(';');
            Vector2[] points = new Vector2[values.Length];

            for (var i = 0; i < points.Length; i++)
            {
                points[i] = ToVector2(values[i]);
            }

            return new Polygon(points);
        }

        // ToRectangle
        public static Rectangle ToRectangle(string value)
        {
            var values = value.Split(',');
            var x = int.Parse(values[0].Trim(), CultureInfo.InvariantCulture);
            var y = int.Parse(values[1].Trim(), CultureInfo.InvariantCulture);
            var w = int.Parse(values[2].Trim(), CultureInfo.InvariantCulture);
            var h = int.Parse(values[3].Trim(), CultureInfo.InvariantCulture);

            return new Rectangle(x, y, w, h);
        }

        // ToRectangleF
        public static RectangleF ToRectangleF(string value)
        {
            var values = value.Split(',');
            var x = int.Parse(values[0].Trim(), CultureInfo.InvariantCulture);
            var y = int.Parse(values[1].Trim(), CultureInfo.InvariantCulture);
            var w = int.Parse(values[2].Trim(), CultureInfo.InvariantCulture);
            var h = int.Parse(values[3].Trim(), CultureInfo.InvariantCulture);

            return new RectangleF(x, y, w, h);
        }

        // ToString
        public static string ToString(IList items, string separator)
        {
            List<string> list = [];

            for (var i = 0; i < items.Count; i++)
            {
                if (items[i]?.ToString() is string value)
                {
                    list.Add(value);
                }
            }

            return string.Join(separator, list.ToArray());
        }

        // ToString
        public static string ToString(Vector2 vector)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0},{1}", vector.X, vector.Y);
        }

        // ToString
        public static string ToString(Polygon polygon)
        {
            var values = new string[polygon.Vertices.Count];

            for (var i = 0; i < polygon.Vertices.Count; i++)
            {
                values[i] = ToString(polygon.Vertices[i]);
            }

            return string.Join(";", values);
        }

        // ToString
        public static string ToString(Rectangle rectangle)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3}", rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        // ToString
        public static string ToString(RectangleF rectangle)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3}", rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        // ToString
        public static string ToString(Color color)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3}", color.R, color.G, color.B, color.A);
        }

        // ToVector2
        public static Vector2 ToVector2(string value)
        {
            var coords = value.Split(',');
            var x = float.Parse(coords[0].Trim(), CultureInfo.InvariantCulture);
            var y = float.Parse(coords[1].Trim(), CultureInfo.InvariantCulture);
            return new Vector2(x, y);
        }
    }
}