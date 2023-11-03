using System;
using System.Collections.Generic;

namespace RawSalt.Logging;

public abstract class Logger
{
	public abstract void Info(string message);
	public abstract void Warn(string message);
	public abstract void Error(string message);
	public abstract void Fatal(string message);


	public static Logger CreateLogger<T>()
	{
		return new WritterLogger(Console.Out);
	}

	[Obsolete("NotImplementYet")]
	public static void AppendLoggerSink(object sink)
	{
		throw new NotImplementedException();
	}
}