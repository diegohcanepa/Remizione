using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ScaryCastle.Effects
{
    /// <summary>
    /// ColorReductionEffect
    /// </summary>
    public sealed class ColorReductionEffect : ShaderEffect
    {
        private readonly EffectParameter rParameter;
        private readonly EffectParameter gParameter;
        private readonly EffectParameter bParameter;
        private readonly EffectParameter aParameter;

        // Constrcutor
        public ColorReductionEffect()
            : base("Effects/ColorReduction")
        {
            rParameter = Effect.Parameters["r"] ?? throw new InvalidOperationException();
            gParameter = Effect.Parameters["g"] ?? throw new InvalidOperationException();
            bParameter = Effect.Parameters["b"] ?? throw new InvalidOperationException();
            aParameter = Effect.Parameters["a"] ?? throw new InvalidOperationException();
        }

        // SetColor
        public void SetColor(float value)
        {
            SetColor(value, value, value, value);
        }

        // SetColor
        public void SetColor(Vector4 value)
        {
            SetColor(value.X, value.Y, value.Z, value.W);
        }

        // SetColor
        public void SetColor(float r, float g, float b, float a)
        {
            rParameter.SetValue(r);
            gParameter.SetValue(g);
            bParameter.SetValue(b);
            aParameter.SetValue(a);
        }
    }
}
