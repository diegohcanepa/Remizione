/* 
Different forms of color enhancement, e.g. strengthening or weakening the red-green-blue channels, 
can be applied to provide different effects, such as: 
producing a ‘night-time’ color scheme, scene transitions, etc.
colored flashes for events, e.g. red flash or red tint for low health, ‘power’ effects, etc.

This reduction may be used to make a texture transparent.
*/

sampler ColorMapSampler : register(s0);

float r = 0.5f;
float g = 0.5f;
float b = 0.5f;
float a = 0.5f;

float4 PixelShaderColorReduction(float4 pos : SV_POSITION, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
	color *= tex2D(ColorMapSampler, texCoord);
	color.r *= r;
	color.g *= g;
	color.b *= b;
	color.a *= a;
	return color;
}


technique colorReduction
{
	pass P0
	{
		#if SM4
			PixelShader = compile ps_4_0_level_9_1 PixelShaderColorReduction();
		#elif SM3
			PixelShader = compile ps_3_0 PixelShaderColorReduction();
		#else
			PixelShader = compile ps_2_0 PixelShaderColorReduction();
		#endif
	}
}