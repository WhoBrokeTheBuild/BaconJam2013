
texture ColorTexture;

sampler2D ColorMap = sampler_state
{
	Texture = <ColorTexture>;	
};

float fadeAmount = 1.0f;

float4 PixelShaderFunction(float2 texCoord: TEXCOORD0) : COLOR0
{

	float4 color = tex2D(ColorMap, texCoord);

	color.rgb = color.rgb * (1.0f - fadeAmount);

	return color;

}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
