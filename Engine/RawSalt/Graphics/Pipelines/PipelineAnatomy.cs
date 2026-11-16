using System.Collections.Immutable;

namespace RawSalt.Graphics.Pipelines;

/// <summary>
/// Shader sources and the compiler that turns them into a pipeline interface.
/// </summary>
public sealed record PipelineAnatomy(SlangCompiler Compiler, params ImmutableArray<string> Sources);
