//
// Lightweight, minimal shading for solid primitives.
// This shader will transform, and draw using the specified transparency.
//

float4x4 WorldViewProj : WorldViewProjection;

float Alpha = 1.0;

// this structure is sent to the pixel shader
struct VS_OUTPUT 
{
    float4 Position : POSITION;
    float4 Color : COLOR;
};

VS_OUTPUT mainVS(float4 pos : POSITION, float4 color : COLOR)
{
    VS_OUTPUT Out;
    
    // transform the position
    Out.Position = mul(pos, WorldViewProj);
    // assign the specified color, manipulation occurs in the pixelshader
    Out.Color = color;
    
    return Out;
}

float4 mainPS(float4 color : COLOR) : COLOR 
{
    // simply return the color provided, with the specified alpha (transparency)
    return float4(color.xyz, Alpha);
}

technique Minimal 
{
    pass p0 
    {
        VertexShader = compile vs_1_1 mainVS();
        PixelShader = compile ps_1_1 mainPS();
    }
}
