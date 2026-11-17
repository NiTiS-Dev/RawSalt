using RawSalt.Graphics.Rendering;

namespace RawSalt.Graphics;

/// <summary>
/// Immutable mesh object.
/// </summary>
/// <typeparam name="T">Type of vertex.</typeparam>
public sealed class StaticMesh<T> : Mesh
	where T : unmanaged
{
	private Buffer Vertex { get; }
	private Buffer Indices { get; }
	private uint VertexCount { get; }
	private uint IndexCount { get; }

	/// <summary>
	/// Creates the buffers on the calling thread. Buffer creation is render-thread only.
	/// </summary>
	public StaticMesh(GraphicsDevice owner, ReadOnlySpan<T> vertices, ReadOnlySpan<uint> indices)
	{
		ArgumentNullException.ThrowIfNull(owner);

		Vertex = owner.CreateBuffer(BufferUsage.Vertex, vertices);
		VertexCount = (uint)vertices.Length;

		if (!indices.IsEmpty)
		{
			Indices = owner.CreateBuffer(BufferUsage.Index, indices);
			IndexCount = (uint)indices.Length;
		}
	}

	/// <inheritdoc/>
	public override void Render(CommandList cmd)
	{
		cmd.SetVertexBuffer(0, Vertex);
		if (IndexCount != 0)
		{
			cmd.SetIndexBuffer(Indices);
			cmd.DrawIndexed(IndexCount);
		}
		else
		{
			cmd.Draw(VertexCount);
		}
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if (!disposing) return;

		Vertex.Dispose();
		if (IndexCount != 0)
		{
			Indices.Dispose();
		}
	}
}