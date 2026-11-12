namespace RawSalt.Graphics.Devices;

/// <summary>
/// Limits of the <see cref="Adapter"/> device.
/// </summary>
public readonly struct DeviceLimits
{
	/// <summary>
	/// Max width of 1-dimensional images.
	/// </summary>
	public uint MaxImageDimension1D { get; init; }

	/// <summary>
	/// Max width and height of 2-dimensional images.
	/// </summary>
	public uint MaxImageDimension2D { get; init; }

	/// <summary>
	/// Max width, height and depth of 3-dimensional images.
	/// </summary>
	public uint MaxImageDimension3D { get; init; }

	/// <summary>
	/// Minimal required uniform buffer alignment.
	/// </summary>
	public uint MinUniformBufferOffsetAlignment { get; init; }
	// TODO: Add more limits, and actually use them!
}