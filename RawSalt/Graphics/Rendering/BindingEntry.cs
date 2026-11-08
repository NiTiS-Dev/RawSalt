using RawSalt.Graphics.Pipelines;

namespace RawSalt.Graphics.Rendering;

public readonly record struct BindingEntry(uint Binding, BindingType Type, ShaderStages Stages, uint Size = 0, uint Count = 1);