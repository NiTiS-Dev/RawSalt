using System.Collections.Immutable;

namespace RawSalt.Graphics;

public sealed record VertexLayout(uint Stride, ImmutableArray<VertexAttribute> Attributes);