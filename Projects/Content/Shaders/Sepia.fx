// =========================================================================
// BlackAndWhiteWithExclusions.fx
// Shader de B&W con exclusión de sangre y verdes amarillentos
// =========================================================================

sampler TextureSampler : register(s0);

// 1.0 = B&W completo | 0.0 = Color original de la escena
float intensity = 1.0;

// Convierte un color a escala de grises
float4 BlackAndWhite(float4 color, float bwIntensity)
{
    float grey = dot(color.rgb, float3(0.299, 0.587, 0.114));
    float3 finalColor = lerp(color.rgb, float3(grey, grey, grey), bwIntensity);
    return float4(finalColor, color.a);
}

float4 PixelShaderFunction(float4 pos : SV_POSITION, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
    float4 texColor = tex2D(TextureSampler, texCoord) * color;

    // 1. Detección de paleta de sangre
    bool isBlood = (texColor.r > texColor.g * 2.5) &&
                   (texColor.r > texColor.b * 2.5) &&
                   (texColor.r >= 0.10 && texColor.r <= 0.45);

    // 2. Detección de verde amarillento (Yellow-Green)
    // - El canal G es el más alto (o muy cercano al R).
    // - El canal R tiene al menos el 50% de la fuerza de G (aporta la componente amarilla).
    // - El canal B es significativamente más bajo que G (evita cianes o blancos).
    bool isYellowGreen = (texColor.g > 0.15) &&
                         (texColor.g >= texColor.r * 0.8) &&
                         (texColor.r >= texColor.g * 0.4) &&
                         (texColor.g > texColor.b * 1.8);

    // Si es un pixel de sangre o verde amarillento, se preserva su color
    if (isBlood || isYellowGreen)
    {
        return texColor;
    }

    // Aplica el efecto Blanco y Negro
    float bwIntensity = saturate(intensity);
    return BlackAndWhite(texColor, bwIntensity);
}

technique BlackAndWhiteTechnique
{
    pass P0
    {
        PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
    }
}