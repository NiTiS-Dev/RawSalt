struct CameraData
{
    float4x4 view;
    float4x4 projection;
};

struct DrawData
{
    float4x4 model;
};

ConstantBuffer<CameraData> camera;
ConstantBuffer<DrawData> draw;
Sampler2D tex;

struct VSInput
{
    float3 position : POSITION;
    float3 color : COLOR;
    float2 uv : TEXCOORD0;
};

struct VSOutput
{
    float4 position : SV_Position;
    float3 color : COLOR;
    float2 uv : TEXCOORD0;
};

[shader("vertex")]
VSOutput vsMain(VSInput input)
{
    VSOutput output;
    output.position = mul(mul(mul(float4(input.position, 1.0), draw.model), camera.view), camera.projection);
    output.color = input.color;
    output.uv = input.uv;
    return output;
}

[shader("fragment")]
float4 fsMain(VSOutput input) : SV_Target
{
    return tex.Sample(input.uv) * float4(input.color, 1.0);
}
