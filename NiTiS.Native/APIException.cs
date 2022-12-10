using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NiTiS.Native;

public sealed class APIException : Exception
{
	public APIException()
	{
	}
	public APIException(string message) : base(message)
	{
	}
	public APIException(string message, Exception innerException) : base(message, innerException)
	{
	}
}