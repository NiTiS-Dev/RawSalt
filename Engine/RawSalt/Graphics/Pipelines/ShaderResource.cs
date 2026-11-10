using RawSalt.Graphics.Rendering;

namespace RawSalt.Graphics.Pipelines;

// Maybe move to the individual graphics API?
/// <summary>
/// A shader resource with its Slang-inferred location.
/// </summary>
public readonly record struct ShaderResource(string Name, uint Set, uint Binding, BindingType Type, uint Size);