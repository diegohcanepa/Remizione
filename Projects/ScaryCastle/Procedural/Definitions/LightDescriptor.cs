using Engendro;
using Microsoft.Xna.Framework;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// LightDescriptor
    /// </summary>
    public sealed class LightDescriptor
    {
        // Constructor
        public LightDescriptor(JsonElement element)
        {
            this.Color = element.GetColor("color", Color.White);
            this.Position = element.GetVector2("position", Vector2.Zero);
            this.Scale = element.GetVector2("scale", Vector2.One);
        }

        // Color
        public Color Color { get; }

        // Position
        public Vector2 Position { get; }

        // Scale
        public Vector2 Scale { get; }
    }
}
