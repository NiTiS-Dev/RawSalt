namespace RawSalt.Native.Vulkan;

public partial struct Surface
{
	public Surface(ulong handle)
	{
		NativeHandle = handle;
	}
}