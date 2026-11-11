using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace Engendro
{
    /// <summary>
    /// ShaderEffect
    /// </summary>
    public abstract class ShaderEffect
    {
        // Constructor
        public ShaderEffect(string shaderName)
        {
            shaderName = Path.Combine("Shaders", shaderName);
            Effect = EngendroGame.Instance.Content.Load<Effect>(shaderName) ?? throw new InvalidOperationException($"Asset not found: {shaderName}.");
        }

        // Effect
        public Effect Effect { get; }
    }
}
