uniform extern texture ScreenTexture;

float grayscaleAmount = 0.0f;

sampler ScreenS = sampler_state
{
	Texture = <ScreenTexture>;	
};

float4 PixelShaderFunction(float2 texCoord: TEXCOORD0) : COLOR0
{

	float4 color = tex2D(ScreenS, texCoord);

	float avg = (color[0] + color[1] + color[2]) / 3.0f;

	color[0] = (avg * grayscaleAmount) + (color[0] * (1.0f - grayscaleAmount));
	color[1] = (avg * grayscaleAmount) + (color[1] * (1.0f - grayscaleAmount));
	color[2] = (avg * grayscaleAmount) + (color[2] * (1.0f - grayscaleAmount));

	return color;

}
technique
{
	pass P0
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}
