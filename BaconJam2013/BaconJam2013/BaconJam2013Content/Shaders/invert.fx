
texture ColorTexture;

sampler2D ColorMap = sampler_state
{
	Texture = <ColorTexture>;	
};

float fadeAmount = 1.0f;

float4 PixelShaderFunction(float2 texCoord: TEXCOORD0) : COLOR0
{

	float4 color = tex2D(ColorMap, texCoord);

	color.r = 1.0f - color.r;
	color.g = 1.0f - color.g;
	color.b = 1.0f - color.b;

	return color;

}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
