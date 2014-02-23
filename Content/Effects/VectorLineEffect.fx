float4x4 mWorld; 
float4x4 mVP; 

struct VINPUT 
{ 
    float3 pos : POSITION; 
    float4 color : COLOR0; 
};

struct VOUTPUT 
{
    float4 pos : POSITION; 
    float4 color : COLOR0; 
};

VOUTPUT VS_Line(VINPUT _IN) 
{ 
    VOUTPUT OUT; 
    OUT.pos = mul(mul(float4(_IN.pos,1),mWorld),mVP); 
    OUT.color = _IN.color; 
    return OUT; 
}

VOUTPUT VS_TLine(VINPUT _IN) 
{ 
    VOUTPUT OUT;  
    OUT.pos = float4(_IN.pos.xyz,1); 
    OUT.color = _IN.color; 
    return OUT; 
}

float4 PS_Line(VOUTPUT _IN) : COLOR0 { return _IN.color; }

Technique T0 
{
    Pass P0 
    { 
        VertexShader=compile vs_2_0 VS_Line(); 
        PixelShader=compile ps_2_0 PS_Line(); 
    } 
    Pass P1 
    { 
        VertexShader=compile vs_2_0 VS_TLine(); 
        PixelShader=compile ps_2_0 PS_Line(); 
    } 
}
