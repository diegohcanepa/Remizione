// Shader parameters
Texture2D SpriteTexture;
sampler2D SpriteTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
    MinFilter = Point;
    MagFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

// Uniforms adjustable from code
float ScanlineIntensity = 0.07; // Scanline intensity (0.0 - 1.0)
float ScanlineCount = 150.0; // Number of scanlines
float Curvature = 0; // Curvature intensity (0.0 - 0.5)
float ChromaticAberration = 0.0002; // Chromatic aberration intensity
float NoiseIntensity = 0.005; // Noise intensity (0.0 - 1.0)

// Vertex Shader structure
struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

// Function to apply CRT distortion
float2 ApplyCRTDistortion(float2 uv, float curvature)
{
    // Center UV coordinates around (0,0)
    float2 centeredUV = uv - 0.5;
    
    // Calculate distance to center
    float dist = length(centeredUV);
    
    // Apply radial distortion
    float distortion = 1.0 + curvature * (dist * dist);
    return centeredUV * distortion + 0.5;
}

// Pixel Shader
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // Apply CRT distortion to base texture coordinates
    float2 distortedUV = ApplyCRTDistortion(input.TextureCoordinates, Curvature);
    
    // Check bounds
    if (distortedUV.x < 0.0 || distortedUV.x > 1.0 ||
        distortedUV.y < 0.0 || distortedUV.y > 1.0)
    {
        return float4(0.0, 0.0, 0.0, 1.0); // Black for areas outside texture
    }
    
    // Chromatic aberration: sample RGB channels with slight offsets
    float2 redOffset = distortedUV + float2(ChromaticAberration, 0.0);
    float2 greenOffset = distortedUV;
    float2 blueOffset = distortedUV - float2(ChromaticAberration, 0.0);
    
    float red = tex2D(SpriteTextureSampler, redOffset).r;
    float green = tex2D(SpriteTextureSampler, greenOffset).g;
    float blue = tex2D(SpriteTextureSampler, blueOffset).b;
    float4 color = float4(red, green, blue, 1.0);
    
    // Calculate scanline with animation
    float scanline = sin((input.TextureCoordinates.y * ScanlineCount) * 3.14159 * 2.0);
    float scanlineEffect = 1.0 - (ScanlineIntensity * (1.0 + scanline) * 0.5);
    
    // Apply RGB tint
    float3 rgbEffect = float3(
        color.r * (1.0 - ScanlineIntensity * 0.1),
        color.g * (1.0 - ScanlineIntensity * 0.05),
        color.b * (1.0 - ScanlineIntensity * 0.15)
    );
    
    // Combine effects
    color.rgb = rgbEffect * scanlineEffect;
    
    return color * input.Color;
}

// Technique
technique ScanlineEffect
{
    pass Pass0
    {
        PixelShader = compile ps_4_0 PixelShaderFunction();
    }
}