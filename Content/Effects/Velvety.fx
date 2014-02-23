/*********************************************************************NVMH3****
$Revision: #3 $

Copyright NVIDIA Corporation 2007
TO THE MAXIMUM EXTENT PERMITTED BY APPLICABLE LAW, THIS SOFTWARE IS PROVIDED
*AS IS* AND NVIDIA AND ITS SUPPLIERS DISCLAIM ALL WARRANTIES, EITHER EXPRESS
OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY
AND FITNESS FOR A PARTICULAR PURPOSE.  IN NO EVENT SHALL NVIDIA OR ITS SUPPLIERS
BE LIABLE FOR ANY SPECIAL, INCIDENTAL, INDIRECT, OR CONSEQUENTIAL DAMAGES
WHATSOEVER (INCLUDING, WITHOUT LIMITATION, DAMAGES FOR LOSS OF BUSINESS PROFITS,
BUSINESS INTERRUPTION, LOSS OF BUSINESS INFORMATION, OR ANY OTHER PECUNIARY
LOSS) ARISING OUT OF THE USE OF OR INABILITY TO USE THIS SOFTWARE, EVEN IF
NVIDIA HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.

% A simple combination of vertex and pixel shaders with velvety edge effects.
% Great for any model that needs that "feeling of softness."

keywords: material organic
date: 070304


To learn more about shading, shaders, and to bounce ideas off other shader
    authors and users, visit the NVIDIA Shader Library Forums at:

    http://developer.nvidia.com/forums/

******************************************************************************/


#ifdef _3DSMAX_
int ParamID = 0x0003; // Defined by Autodesk
#endif /* _3DSMAX_ */

float Script : STANDARDSGLOBAL <
    string UIWidget = "none";
    string ScriptClass = "object";
    string ScriptOrder = "standard";
    string ScriptOutput = "color";
    string Script = "Technique=Technique?Simple:Textured:VertexSimple:VertexTextured;";
> = 0.8;

//// UN-TWEAKABLES - AUTOMATICALLY-TRACKED TRANSFORMS ////////////////

float4x4 WorldITXf : WorldInverseTranspose < string UIWidget="None"; >;
float4x4 WvpXf : WorldViewProjection < string UIWidget="None"; >;
float4x4 WorldXf : World < string UIWidget="None"; >;
float4x4 ViewIXf : ViewInverse < string UIWidget="None"; >;

/************* TWEAKABLES **************/

/// Point Lamp 0 ////////////
float3 Lamp0Pos : Position <
    string Object = "PointLight0";
    string UIName =  "Lamp 0 Position";
    string Space = "World";
> = {-0.5f,2.0f,1.25f};
float3 Lamp0Color : Specular <
    string UIName =  "Lamp 0";
    string Object = "Pointlight0";
    string UIWidget = "Color";
> = {1.0f,1.0f,1.0f};

float3 DiffColor <
	string UIName = "Primary";
	string UIWidget = "Color";
> = {0.5f, 0.5f, 0.5f};

float3 SpecColor <
	string UIWidget = "Color";
	string UIName = "Fuzz";
> = {0.7f, 0.7f, 0.75f};

float3 SubColor <
	string UIWidget = "Color";
	string UIName = "Under-Color";
> = {0.2f, 0.2f, 1.0f};

float RollOff <
    string UIWidget = "slider";
    float UIMin = 0.0;
    float UIMax = 1.0;
    float UIStep = 0.05;
	string UIName = "Edge Rolloff";
> = 0.3;

///

texture ColorTexture : DIFFUSE <
    string ResourceName = "default_color.dds";
    string UIName =  "Diffuse Texture";
    string ResourceType = "2D";
>;

sampler2D ColorSampler = sampler_state {
    Texture = <ColorTexture>;
    MinFilter = Linear;
    MipFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};  

// shared shadow mapping supported in Cg version

/************* DATA STRUCTS **************/

/* data from application vertex buffer */
struct appdata {
    float3 Position	: POSITION;
    float4 UV		: TEXCOORD0;
    float4 Normal	: NORMAL;
    float4 Tangent	: TANGENT0;
    float4 Binormal	: BINORMAL0;
};

/* data passed from vertex shader to pixel shader */
struct vertexOutput {
    float4 HPosition	: POSITION;
    float2 UV		: TEXCOORD0;
    // The following values are passed in "World" coordinates since
    //   it tends to be the most flexible and easy for handling
    //   reflections, sky lighting, and other "global" effects.
    float3 LightVec	: TEXCOORD1;
    float3 WorldNormal	: TEXCOORD2;
    float3 WorldTangent	: TEXCOORD3;
    float3 WorldBinormal : TEXCOORD4;
    float3 WorldView	: TEXCOORD5;
};

/* data passed from vertex shader to pixel shader */
struct shadedVertexOutput {
    float4 HPosition	: POSITION;
    float2 UV		: TEXCOORD0;
    float4 diffCol	: COLOR0;
    float4 specCol	: COLOR1;
};

/*********** vertex shader ******/

shadedVertexOutput velvetVS(appdata IN)
{
    shadedVertexOutput OUT;
    float3 Nn = normalize(mul(IN.Normal,WorldITXf).xyz);
    float4 Po = float4(IN.Position.xyz,1);
    OUT.HPosition = mul(Po,WvpXf);
    float3 Pw = mul(Po,WorldXf).xyz;
    float3 Ln = normalize(Lamp0Pos - Pw);
    float ldn = dot(Ln,Nn);
    float diffComp = max(0,ldn);
    float3 diffContrib = diffComp * DiffColor;
    float subLamb = smoothstep(-RollOff,1.0,ldn) - smoothstep(0.0,1.0,ldn);
    subLamb = max(0.0,subLamb);
    float3 subContrib = subLamb * SubColor;
    OUT.UV = IN.UV.xy;
    float3 Vn = normalize(ViewIXf[3].xyz - Pw);
    float vdn = 1.0-dot(Vn,Nn);
    float3 vecColor = vdn.xxx;
    OUT.diffCol = float4((subContrib+diffContrib).xyz,1);
    OUT.specCol = float4((vecColor*SpecColor).xyz,1);
    return OUT;
}

/*********** Generic Vertex Shader ******/

vertexOutput std_VS(appdata IN) {
    vertexOutput OUT = (vertexOutput)0;
    OUT.WorldNormal = mul(IN.Normal,WorldITXf).xyz;
    OUT.WorldTangent = mul(IN.Tangent,WorldITXf).xyz;
    OUT.WorldBinormal = mul(IN.Binormal,WorldITXf).xyz;
    float4 Po = float4(IN.Position.xyz,1);
    float4 Pw = mul(Po,WorldXf);
    OUT.LightVec = (Lamp0Pos - Pw.xyz);
#ifdef FLIP_TEXTURE_Y
    OUT.UV = float2(IN.UV.x,(1.0-IN.UV.y));
#else /* !FLIP_TEXTURE_Y */
    OUT.UV = IN.UV.xy;
#endif /* !FLIP_TEXTURE_Y */
    OUT.WorldView = normalize(ViewIXf[3].xyz - Pw.xyz);
    OUT.HPosition = mul(Po,WvpXf);
    return OUT;
}

/** pixel shader  **/

void velvet_shared(vertexOutput IN,
			out float3 DiffuseContrib,
			out float3 SpecularContrib)
{
    float3 Ln = normalize(IN.LightVec.xyz);
    float3 Nn = normalize(IN.WorldNormal);
    float3 Vn = normalize(IN.WorldView);
    float3 Hn = normalize(Vn + Ln);
    float ldn = dot(Ln,Nn);
    float diffComp = max(0,ldn);
    float vdn = 1.0-dot(Vn,Nn);
    float3 diffContrib = diffComp * DiffColor;
    float subLamb = smoothstep(-RollOff,1.0,ldn) - smoothstep(0.0,1.0,ldn);
    subLamb = max(0.0,subLamb);
    float3 subContrib = subLamb * SubColor;
    float3 vecColor = vdn.xxx;
    DiffuseContrib = (subContrib+diffContrib).xyz;
    SpecularContrib = (vecColor*SpecColor).xyz;
}

float4 velvetPS(vertexOutput IN) : COLOR {
    float3 diffContrib;
    float3 specContrib;
	velvet_shared(IN,diffContrib,specContrib);
    float3 result = diffContrib + specContrib;
    return float4(result,1);
}

float4 velvetPS_t(vertexOutput IN) : COLOR {
    float3 diffContrib;
    float3 specContrib;
	velvet_shared(IN,diffContrib,specContrib);
    float3 map = tex2D(ColorSampler,IN.UV.xy).xyz;
    float3 result = specContrib + (map * diffContrib);
    return float4(result,1);
}

float4 velvetPS_pass(shadedVertexOutput IN) : COLOR {
    float4 result = IN.diffCol + IN.specCol;
    return float4(result.xyz,1);
}

float4 velvetPS_pass_t(shadedVertexOutput IN) : COLOR {
    float4 map = tex2D(ColorSampler,IN.UV.xy);
    float4 result = IN.specCol + (map * IN.diffCol);
    return float4(result.xyz,1);
}

/*************/

technique Simple <
	string Script = "Pass=p0;";
> {
    pass p0 <
	string Script = "Draw=geometry;";
> {		
	VertexShader = compile vs_2_0 std_VS();
	ZEnable = true;
		ZWriteEnable = true;
		ZFunc = LessEqual;
		AlphaBlendEnable = false;
		CullMode = None;
	PixelShader = compile ps_2_a velvetPS();
    }
}

technique Textured <
	string Script = "Pass=p0;";
> {
    pass p0 <
	string Script = "Draw=geometry;";
> {		
	VertexShader = compile vs_2_0 std_VS();
	ZEnable = true;
		ZWriteEnable = true;
		ZFunc = LessEqual;
		AlphaBlendEnable = false;
		CullMode = None;
	PixelShader = compile ps_2_a velvetPS_t();
    }
}

technique VertexSimple <
	string Script = "Pass=p0;";
> {
    pass p0 <
	string Script = "Draw=geometry;";
> {		
	VertexShader = compile vs_2_0 velvetVS();
	ZEnable = true;
		ZWriteEnable = true;
		ZFunc = LessEqual;
		AlphaBlendEnable = false;
		CullMode = None;
	PixelShader = compile ps_2_a velvetPS_pass();
    }
}

technique VertexTextured <
	string Script = "Pass=p0;";
> {
    pass p0 <
	string Script = "Draw=geometry;";
> {		
	VertexShader = compile vs_2_0 velvetVS();
	ZEnable = true;
		ZWriteEnable = true;
		ZFunc = LessEqual;
		AlphaBlendEnable = false;
		CullMode = None;
	PixelShader = compile ps_2_a velvetPS_pass_t();
    }
}

/***************************** eof ***/