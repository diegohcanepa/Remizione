using Adberration;
using Engendro;
using Microsoft.Xna.Framework;
using System.Text;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// FeaturedActorDescriptor
    /// </summary>
    public sealed class FeaturedActorDescriptor
    {
        public FeaturedActorDescriptor(JsonElement element)
        {
            this.Name = element.GetString("name");
            this.Position = element.GetVector2("position", Vector2.Zero);
            this.FacingDirection = element.GetEnum("facingDirection", FacingDirection.Right);
        }

        // Name
        public string Name { get; }

        // FacingDirection
        public FacingDirection FacingDirection { get; }

        // Position
        public Vector2 Position { get; }
    }
}
