uniform extern texture ScreenTexture;

sampler ScreenS = sampler_state
{
	Texture = <ScreenTexture>;	
};

float2 center;
float radius = .4;

float4 PixelShaderFunction(float2 texCoord: TEXCOORD0) : COLOR0
{

	float4 color = tex2D(ScreenS, texCoord);

    float circleMult = ( 4.0f / 3.0f );

    float2 newCoord = texCoord - center;

    float xPart = pow(newCoord.x * circleMult, 2);
    float yPart = pow(newCoord.y, 2);
	
	float distance = clamp(sqrt(xPart + yPart) / radius, 0, 1);

    float mult = 1 - clamp(distance, 0, 0.7);

    if (mult > 0)
        mult *= 1.5;
	
    color.rgb *= 1.1 * mult;

	return color;

}
technique
{
	pass P0
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}
