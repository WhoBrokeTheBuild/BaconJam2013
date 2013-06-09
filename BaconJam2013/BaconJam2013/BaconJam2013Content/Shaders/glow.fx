uniform extern texture ScreenTexture;

float2 distance;
float2 cardPos;
float2 cardSize;
float4 glowColor;

sampler ScreenS = sampler_state
{
	Texture = <ScreenTexture>;	
};

float4 PixelShaderFunction(float2 texCoord: TEXCOORD0) : COLOR0
{

	float4 color = tex2D(ScreenS, texCoord);

	if (
		(
		 texCoord[0] > cardPos[0] - distance[0] &&
		 texCoord[1] > cardPos[1] - distance[1] &&
		 texCoord[0] < cardPos[0] + cardSize[0] + distance[0] &&
		 texCoord[1] < cardPos[1] + cardSize[1] + distance[1]
		)
		&&
		!(
		 texCoord[0] > cardPos[0] &&
		 texCoord[1] > cardPos[1] &&
		 texCoord[0] < cardPos[0] + cardSize[0] &&
		 texCoord[1] < cardPos[1] + cardSize[1]
		)
	)
	{

		float distAmount = 1.0f;

		if (texCoord[0] < cardPos[0]) // Left
		{
			
			if (texCoord[1] < cardPos[1]) // Top-Left Corner
			{
				distAmount = ((cardPos[0] - texCoord[0]) / distance[0]) + ((cardPos[1] - texCoord[1]) / distance[1]);
			}
			else if (texCoord[1] > cardPos[1] + cardSize[1]) // Bottom-Left Corner
			{
				distAmount = ((cardPos[0] - texCoord[0]) / distance[0]) + ((texCoord[1] - (cardPos[1] + cardSize[1])) / distance[1]);
			}
			else // Left-Side
			{
				distAmount = (cardPos[0] - texCoord[0]) / distance[0];
			}

		}
		else if (texCoord[0] > cardPos[0] + cardSize[0]) // Right
		{
		
			if (texCoord[1] < cardPos[1]) // Top-Right Corner
			{
				distAmount = ((cardPos[1] - texCoord[1]) / distance[1]) + ((texCoord[0] - (cardPos[0] + cardSize[0])) / distance[0]);
			}
			else if (texCoord[1] > cardPos[1] + cardSize[1]) // Bottom-Right Corner
			{
				distAmount = ((texCoord[1] - (cardPos[1] + cardSize[1])) / distance[1]) + ((texCoord[0] - (cardPos[0] + cardSize[0])) / distance[0]);
			}
			else // Right-Side
			{
				distAmount = (texCoord[0] - (cardPos[0] + cardSize[0])) / distance[0];
			}

		}
		else // Middle
		{
			if (texCoord[1] < cardPos[1]) // Top-Side
			{
				distAmount = (cardPos[1] - texCoord[1]) / distance[1];
			}
			else if (texCoord[1] > cardPos[1] + cardSize[1]) // Bottom-Side
			{
				distAmount = (texCoord[1] - (cardPos[1] + cardSize[1])) / distance[1];
			}
			else // Center
			{

			}
		}

		distAmount = clamp(distAmount, 0.0f, 1.0f);

		float oneMinus = (1.0f - distAmount);
		
		color[0] = (glowColor[0] * oneMinus) + (color[0] * distAmount);
		color[1] = (glowColor[1] * oneMinus) + (color[1] * distAmount);
		color[2] = (glowColor[2] * oneMinus) + (color[2] * distAmount);

	}

	return color;

}
technique
{
	pass P0
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}
