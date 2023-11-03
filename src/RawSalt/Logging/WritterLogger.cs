using System.IO;

namespace RawSalt.Logging;

internal sealed class WritterLogger : Logger
{
	private readonly TextWriter outWritter;
	public WritterLogger(TextWriter outWritter)
	{
		this.outWritter = outWritter;
	}

	public override void Error(string message)
	{
		outWritter.Write("[ERR] ");
		outWritter.WriteLine(message);
	}

	public override void Fatal(string message)
	{
		outWritter.Write("[FTL] ");
		outWritter.WriteLine(message);
	}

	public override void Info(string message)
	{
		outWritter.Write("[INF] ");
		outWritter.WriteLine(message);
	}

	public override void Warn(string message)
	{
		outWritter.Write("[WRN] ");
		outWritter.WriteLine(message);
	}
}
