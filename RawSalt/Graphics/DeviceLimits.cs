namespace RawSalt.Graphics;

public readonly struct DeviceLimits
{
	public uint MaxImageDimension1D { get; init; }
	public uint MaxImageDimension2D { get; init; }
	public uint MaxImageDimension3D { get; init; }
	public uint MinUniformBufferOffsetAlignment { get; init; }
	// TODO: Add more limits, and actually use them!
}