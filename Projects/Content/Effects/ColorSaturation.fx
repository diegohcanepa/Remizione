/* 
Different forms of color enhancement, e.g. strengthening or weakening the red-green-blue channels, 
can be applied to provide different effects, such as: 
producing a ‘night-time’ color scheme, scene transitions, etc.
colored flashes for events, e.g. red flash or red tint for low health, ‘power’ effects, etc.
*/


sampler ColorMapSampler : register(s0);

float r = 0.5f;
float g = 0.5f;
float b = 0.5f;
float a = 0.5f;

/*
Colors will be automatically clamped to a maximum/minimum value of 1 and 0.
Any combination of the color.rgba channels can be modified in order to
achieve multi-color effects.
*/
float4 PixelShaderColorSaturation(float4 pos : SV_POSITION, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
	color *= tex2D(ColorMapSampler, texCoord);
	color.r /= r;
	color.g /= g;
	color.b /= b;
	color.a /= a;
	return color;
}


technique colorSaturation
{
	pass P0
	{
		#if SM4
			PixelShader = compile ps_4_0_level_9_1 PixelShaderColorSaturation();
		#elif SM3
			PixelShader = compile ps_3_0 PixelShaderColorSaturation();
		#else
			PixelShader = compile ps_2_0 PixelShaderColorSaturation();
		#endif;
	}
}