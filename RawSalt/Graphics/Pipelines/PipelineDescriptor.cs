using System.Collections.Immutable;
using RawSalt.Graphics.Drawing;
using RawSalt.Graphics.Rendering;

namespace RawSalt.Graphics.Pipelines;

public sealed record PipelineDescriptor
{
	public required ShaderModule VertexShader { get; init; }
	public required ShaderModule FragmentShader { get; init; }
	public required PixelFormat ColorFormat { get; init; }
	public required ImmutableArray<BindGroupLayout> BindGroupLayout { get; init; }
	public VertexLayout? VertexLayout { get; init; }
	public Topology Topology { get; init; } = Topology.TriangleList;
	public CullMode CullMode { get; init; } = CullMode.Back;
	public FrontFace FrontFace { get; init; } = FrontFace.CounterClockWise;
}