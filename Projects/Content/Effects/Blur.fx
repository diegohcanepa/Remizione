/* 
A blur effect can be added by averaging each pixel’s color using the colors of the surrounding pixels, e.g. for explosion effects, etc.
*/


sampler ColorMapSampler : register(s0);

float offset = 0.01f;

float4 PixelShaderBlur(float4 pos : SV_POSITION, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
	color *= tex2D( ColorMapSampler, float2(texCoord.x+offset, texCoord.y+offset));
	color += tex2D( ColorMapSampler, float2(texCoord.x-offset, texCoord.y-offset));
	color += tex2D( ColorMapSampler, float2(texCoord.x+offset, texCoord.y-offset));
	color += tex2D( ColorMapSampler, float2(texCoord.x-offset, texCoord.y+offset));
	color = color / 4;
	return color;
}


technique blur
{
	pass P0
	{
		#if SM4
			PixelShader = compile ps_4_0_level_9_1 PixelShaderBlur();
		#elif SM3
			PixelShader = compile ps_3_0 PixelShaderBlur();
		#else
			PixelShader = compile ps_2_0 PixelShaderBlur();
		#endif;
	}
}