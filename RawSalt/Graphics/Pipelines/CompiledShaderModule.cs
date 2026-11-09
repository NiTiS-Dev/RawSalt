namespace RawSalt.Graphics.Pipelines;

public readonly record struct CompiledShaderModule(ShaderStages Stage, ReadOnlyMemory<byte> Code);