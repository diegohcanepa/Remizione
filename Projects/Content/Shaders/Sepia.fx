// =========================================================================
// SepiaWithBloodExclusion.fx
// Shader de sepia global con exclusión de paleta de sangre (#511212, etc.)
// =========================================================================

sampler TextureSampler : register(s0);

// 1.0 = Sepia completo | 0.0 = Color original de la escena
float intensity = 1.0;

// Convierte un color a tono sepia
float4 Sepia(float4 color, float sepiaIntensity)
{
    float3 sepiaColor = float3(1.2, 0.85, 0.55);
    float grey = dot(color.rgb, float3(0.299, 0.587, 0.114));
    
    // Interpolación: de color original a escala sepia según sepiaIntensity
    float3 finalColor = lerp(color.rgb, grey * sepiaColor, sepiaIntensity);
    return float4(finalColor, color.a);
}

float4 PixelShaderFunction(float4 pos : SV_POSITION, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
    // Lectura de la textura combinada con el color del vértice de SpriteBatch
    float4 texColor = tex2D(TextureSampler, texCoord) * color;

    // Detección de paleta de sangre (#511212, #3f0e0e, #260909, #330b0b)
    // - El canal R domina fuertemente sobre G y B (al menos 2.5 veces mayor).
    // - El canal R está acotado en el rango [0.10, 0.45] en float saturado (equivalente a 25.5 - 114.75 en byte).
    bool isBlood = (texColor.r > texColor.g * 2.5) &&
                   (texColor.r > texColor.b * 2.5) &&
                   (texColor.r >= 0.10 && texColor.r <= 0.45);

    // Si es un pixel de sangre, se devuelve intacto ignorando el sepia
    if (isBlood)
    {
        return texColor;
    }

    // Calcula y aplica el efecto sepia
    float sepiaIntensity = saturate(intensity);
    return Sepia(texColor, sepiaIntensity);
}

technique colorReduction
{
    pass P0
    {
        PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
    }
}