Texture2D SpriteTexture;
SamplerState SpriteTextureSampler
{
    Texture = <SpriteTexture>;
    Filter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

float GrainAmount = 0.09f;
float2 CameraOffset = float2(0.0f, 0.0f); // Posición X,Y de la cámara en el mundo
float2 Scale = float2(8.0f, 8.0f); // Escala/zoom de pantalla (ej: si 320x180 escala a 1280x720, Scale es 4)

struct VertexShaderOutput
{
    float4 Position : SV_POSITION; // Coordenadas en píxeles reales del monitor
    float4 Color : COLOR0;
    float2 TextureCoordinate : TEXCOORD0;
};

float Hash21(float2 p)
{
    float3 p3 = frac(float3(p.xyx) * float3(0.1031, 0.1030, 0.0973));
    p3 += dot(p3, p3.yzx + 33.33);
    return frac((p3.x + p3.y) * p3.z);
}

float4 MainPS(VertexShaderOutput input) : SV_TARGET
{
    float4 color = SpriteTexture.Sample(SpriteTextureSampler, input.TextureCoordinate) * input.Color;
    
    if (color.a == 0.0)
        return color;

    // 1. input.Position.xy calcula el ruido a resolución nativa de monitor (grano fino de alta definición).
    // 2. Escalamos CameraOffset por Scale y aplicamos floor() para truncar a píxeles enteros de pantalla.
    //    Esto iguala la velocidad de desplazamiento del grano con el scroll del pixel art y elimina el temblor.
    float2 worldScreenPos = floor(input.Position.xy + floor(CameraOffset * Scale));

    float noise = (Hash21(worldScreenPos) - 0.5) * GrainAmount;
    color.rgb += noise * color.a;

    return color;
}

technique GrainTechnique
{
    pass P0
    {
        PixelShader = compile ps_4_0 MainPS();
    }
};