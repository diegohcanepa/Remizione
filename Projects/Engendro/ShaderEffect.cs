using Microsoft.Xna.Framework.Graphics;
using System;

namespace Engendro
{
    /// <summary>
    /// ShaderEffect
    /// </summary>
    public abstract class ShaderEffect
    {
        // Constructor
        public ShaderEffect(string assetName)
        {
            Effect = EngendroGame.Instance.Content.Load<Effect>(assetName) ?? throw new InvalidOperationException($"Asset not found: {assetName}.");
        }

        // Effect
        public Effect Effect { get; }
    }
}
