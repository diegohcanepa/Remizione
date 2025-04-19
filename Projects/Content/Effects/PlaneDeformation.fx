/*
By using a sinusoidal function it becomes possible to introduce a wave-like pattern of sampling,
providing a ‘Heat-haze’ from hot objects.
*/

#if __PSSL__
Texture2D<float4> Texture0 : register(t0);
SamplerState Texture0Sampler : register(s0);
#elif NSWITCH
Texture2D<float4> Texture0 : register(t0);
sampler Texture0Sampler : register(s0);
#elif SM5
Texture2D Texture0 : register(t0);
sampler ColorMapSampler : register(s0);
#else
sampler ColorMapSampler : register(s0);
#endif

float offsetX = 0.0f;
float offsetY = 0.0f;
float scaleX = 10.0;
float scaleY = 10.0;
float magnitude = 0.0;

/*
This effect is nearly the same as the Water-Effect, except only the .x component of texCoord is used
*/
float4 PixelShaderWavy(
#if !__PSSL__
	float4 pos : SV_POSITION, 
#endif
	float4 color : COLOR0, float2 texCoord : TEXCOORD0) :
#if __PSSL__
	S_TARGET_OUTPUT
#else
	SV_TARGET0
#endif
{
	texCoord.x += sin(scaleX * (texCoord.x + offsetX)) * magnitude;
	texCoord.y += cos(scaleY * (texCoord.y + offsetY)) * magnitude;
#if NSWITCH || __PSSL__
	color *= Texture0.Sample(Texture0Sampler, texCoord);
#elif SM5
	color *= Texture0.Sample(ColorMapSampler, texCoord);
#else
	color *= tex2D(ColorMapSampler, texCoord);
#endif

	return color;
}

technique wavy
{
	pass P0
	{
#if __PSSL__
		PixelShader = compile sce_ps_orbis PixelShaderWavy();
#elif SM5 || NSWITCH
		PixelShader = compile ps_5_0 PixelShaderWavy();
#elif SM4
		PixelShader = compile ps_4_0_level_9_1 PixelShaderWavy();
#elif SM3
		PixelShader = compile ps_3_0 PixelShaderWavy();
#else
		PixelShader = compile ps_2_0 PixelShaderWavy();
#endif
	}
}