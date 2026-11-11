float outlineThickness = 2.0f; // Grosor del outline en píxeles (ajusta este valor)
float4 outlineColor = float4(1.0f, 1.0f, 1.0f, 1.0f); // Color del outline

Texture2D SpriteTexture;
float2 textureSize; // Tamaño de la textura (ancho, alto)

sampler2D SpriteTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 pos = input.TextureCoordinates;
    float4 col = tex2D(SpriteTextureSampler, pos) * input.Color;
    
    float centerAlpha = col.a;

    // Calcula el tamaño del texel en coordenadas normalizadas
    float2 texelSize = float2(outlineThickness / textureSize.x, outlineThickness / textureSize.y);

    // Muestrea los píxeles vecinos
    float leftAlpha = tex2D(SpriteTextureSampler, pos + float2(-texelSize.x, 0.0f)).a;
    float rightAlpha = tex2D(SpriteTextureSampler, pos + float2(texelSize.x, 0.0f)).a;
    float upAlpha = tex2D(SpriteTextureSampler, pos + float2(0.0f, -texelSize.y)).a;
    float downAlpha = tex2D(SpriteTextureSampler, pos + float2(0.0f, texelSize.y)).a;

    // Si el píxel central es transparente pero alguno de los vecinos no lo es, dibuja el outline
    if (centerAlpha == 0.0f && (leftAlpha > 0.0f || rightAlpha > 0.0f || upAlpha > 0.0f || downAlpha > 0.0f))
    {
        col = outlineColor;
    }

    return col;
}

technique OutlineEffect
{
    pass P0
    {
        PixelShader = compile ps_4_0_level_9_1 MainPS();
    }
};