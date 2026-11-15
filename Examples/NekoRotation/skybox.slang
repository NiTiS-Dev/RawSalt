struct SkyData
{
    float4x4 inverseViewProjection;
};

ConstantBuffer<SkyData> sky;
SamplerCube skybox;

struct VSInput
{
    float2 position : POSITION;
};

struct VSOutput
{
    float4 position : SV_Position;
    float3 direction : TEXCOORD0;
};

[shader("vertex")]
VSOutput vsMain(VSInput input)
{
    // Oversized triangle in clip space, at the far plane.
    float4 clip = float4(input.position, 1.0, 1.0);

    VSOutput output;
    output.position = clip;
    float4 world = mul(clip, sky.inverseViewProjection);
    output.direction = world.xyz / world.w;
    return output;
}

[shader("fragment")]
float4 fsMain(VSOutput input) : SV_Target
{
    return skybox.Sample(normalize(input.direction));
}
