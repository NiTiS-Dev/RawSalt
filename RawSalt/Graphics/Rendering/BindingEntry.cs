namespace RawSalt.Graphics.Rendering;

public readonly record struct BindingEntry(uint Binding, BindingType Type, ShaderStages Stages, uint Size, uint Count = 1);