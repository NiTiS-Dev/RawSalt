using System;

namespace RawSalt.Core;

/// <summary>
/// Client side (server|user)
/// </summary>
/// <remarks>
/// Uses only 3th and 4th bit<br/>
/// </remarks>
[Flags]
public enum Side : byte
{
	Server = 0b_0001_0000,
	User =   0b_0010_0000,
	Both =   0b_0011_0000,
}
