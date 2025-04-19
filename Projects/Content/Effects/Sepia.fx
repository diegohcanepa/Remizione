sampler TextureSampler : register(s0);

float intensity = 1;

float4 Sepia(float4 color, float sepiaIntensity)
{
	float3 sepiaColor = float3(1.2, 0.85, 0.55); // Valores iniciales para un tono sepia cálido
    float grey = dot(color.rgb, float3(0.299, 0.587, 0.114));
    float3 finalColor = lerp(grey * sepiaColor, color.rgb, sepiaIntensity);
    return float4(finalColor, color.a);
}

float4 PixelShaderFunction(float4 pos : SV_POSITION, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
	color = tex2D(TextureSampler, texCoord);

	// Calcula la intensidad de sepia basada en la progresión de la transición
    float sepiaIntensity = saturate(1.0 - intensity);

    return Sepia(color, sepiaIntensity);
}

technique colorReduction
{
	pass P0
	{
		#if SM4
			PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
		#elif SM3
			PixelShader = compile ps_3_0 PixelShaderFunction();
		#else
			PixelShader = compile ps_2_0 PixelShaderFunction();
		#endif
	}
}