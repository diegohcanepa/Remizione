using Microsoft.Xna.Framework;
using System;

namespace Engendro
{
    /// <summary>
    /// Vector2Extensions
    /// </summary>
    public static class Vector2Extensions
    {
        extension(Vector2 vector1)
        {
            // AngleBetween
            public double AngleBetween(Vector2 vector2)
            {
                double sin = (vector1.X * vector2.Y) - (vector2.X * vector1.Y);
                double cos = (vector1.X * vector2.X) + (vector1.Y * vector2.Y);

                return Math.Atan2(sin, cos) * (180 / Math.PI);
            }
        }

        // Random
        public static Vector2 Random(this Vector2 origin, float minimumRadius, float maximumRadius)
        {
            return Random(origin, new Vector2(minimumRadius), new Vector2(maximumRadius));
        }

        // Random
        public static Vector2 Random(this Vector2 origin, Vector2 minRadius, Vector2 maxRadius)
        {
            // En C# moderno (.NET 6+), usamos Random.Shared para thread-safety y eficiencia.
            var rng = System.Random.Shared;

            // 1. Generamos un ángulo aleatorio entre 0 y 2*PI
            // MathHelper.TwoPi es la constante de MonoGame para 360 grados en radianes
            float angle = (float)(rng.NextDouble() * MathHelper.TwoPi);

            // 2. Calculamos seno y coseno.
            // System.Math devuelve double, así que hacemos cast a (float) para Vector2
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            // 3. Obtenemos un factor 't' entre 0 y 1 para la interpolación
            float t = (float)rng.NextDouble();

            // 4. Interpolamos (Lerp) el radio mínimo y máximo
            // MathHelper.Lerp es el equivalente directo en MonoGame
            float xRadius = MathHelper.Lerp(minRadius.X, maxRadius.X, t);
            float yRadius = MathHelper.Lerp(minRadius.Y, maxRadius.Y, t);

            // 5. Construimos el vector final
            return origin + new Vector2(cos * xRadius, sin * yRadius);
        }

        // Round
        public static Vector2 Round(this Vector2 value, int decimals)
        {
            value.X = value.X.Round(decimals);
            value.Y = value.Y.Round(decimals);

            return value;
        }
    }
}
