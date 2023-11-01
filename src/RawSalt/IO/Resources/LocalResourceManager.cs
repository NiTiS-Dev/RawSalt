using System;

namespace RawSalt.IO.Resources;
public class LocalResourceManager : ResourceManager
{
	public override IReadStream? Open(Uri location)
	{
		if (location.IsFile == true)
		{
			// Open file
			return null;
		}

		return null;
	}
}
