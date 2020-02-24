void CheckIfEdgeCoord_float(Texture2D txt, float Width, float2 uv, float2 Texel, SamplerState Sampler, out float IsEdge, out float edgeDistance)
{

	float x = 1 / Texel.x;
	float y = 1 / Texel.y;

	bool hasHitEdge = false;
	edgeDistance = 0;
	
	for (float i = 1; i <= Width; i++)
	{
		float l = SAMPLE_TEXTURE2D(txt, Sampler, uv + float2(-x, 0) * i).a;
		float r = SAMPLE_TEXTURE2D(txt, Sampler, uv + float2(x, 0) * i).a;
		float u = SAMPLE_TEXTURE2D(txt, Sampler, uv + float2(0, y) * i).a;
		float d = SAMPLE_TEXTURE2D(txt, Sampler, uv + float2(0, -y) * i).a;

		float dTR = SAMPLE_TEXTURE2D(txt, Sampler, uv + float2(x, y) * i).a;
		float dTL = SAMPLE_TEXTURE2D(txt, Sampler, uv + float2(-x, y) * i).a;
		float dBR = SAMPLE_TEXTURE2D(txt, Sampler, uv + float2(x, -y) * i).a;
		float dBL = SAMPLE_TEXTURE2D(txt, Sampler, uv + float2(-x, -y) * i).a;
		
		float edge = l + r + u + d + dTR + dTL + dBR + dBL;

		if (edge >= 0.1 && !hasHitEdge)
		{
			hasHitEdge = true;
			edgeDistance = i;
		}
	}
	
	IsEdge = (hasHitEdge ? 1 : 0);	
}

float2 RemapUV(float2 size1, float2 scale, float2 uv)
{
    
    float2 size2 = size1 / scale;
    
    float2 ratio = size1 / size2;
    float2 minUV = (size1 - size2) / size1 / 2;
    
    float2 tarUV = (uv - minUV) * ratio;
    
    return tarUV;
}

void CheckIfShouldDraw_float(Texture2D txt, float OutlineWidth, float2 uv, float2 size1, float2 scale, SamplerState Sampler, out float ShouldDraw, out float edgeDistance)
{
    float edge = 0;
    bool hasHitEdge;
    
    float2 uvStep = 1 / size1;
    
    for (float i = 0; i <= OutlineWidth; i++)
    {
        for (float x = -1; x <= 1; x++)
        {
            for (float y = -1; y <= 1; y++)
            {
                float2 checkUV = uv + float2(x, y) * uvStep * i;
                float2 remappedUV = RemapUV(size1, scale, checkUV);
                float4 clr = SAMPLE_TEXTURE2D(txt, Sampler, remappedUV);
                
                bool isClamped = clamp(remappedUV.x, 0, 1/scale.x) == remappedUV.x && clamp(remappedUV.y, 0, 1/scale.y) == remappedUV.y;
                
                if (isClamped && clr.a >= 0.8 && !hasHitEdge)
                {
                    edgeDistance = i;
                    hasHitEdge = true;
                }
            }
        }
    }
    
    ShouldDraw = (hasHitEdge ? 1 : 0);
    
}



void DrawRemapped_float(Texture2D txt, float Width, float2 uv, float2 size1, float2 size2, SamplerState Sampler, out float4 color)
{
    float edge = 0;
    bool hasHitEdge;
    
    float2 uvStep = 1 / size2;
    
    float2 remappedUV = RemapUV(size1, size2, uv);
    float4 clr = SAMPLE_TEXTURE2D(txt, Sampler, remappedUV);
    
    color = clr;
    
}