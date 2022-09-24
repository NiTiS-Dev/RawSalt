using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawSalt.Core;

/// <summary>
/// Client platform (desktop, android, etc...)
/// </summary>
/// <remarks>
/// Uses only 5th to 8th bit<br/>
/// </remarks>
public enum Platform : byte
{
	Windows = 0b_0000_0001,
	Android = 0b_0000_0010,
	Linux =   0b_0000_0011,
}