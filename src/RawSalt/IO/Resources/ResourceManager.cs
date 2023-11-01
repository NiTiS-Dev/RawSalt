using System;

namespace RawSalt.IO.Resources;

public abstract class ResourceManager
{
	public abstract IReadStream? Open(Uri location);
}