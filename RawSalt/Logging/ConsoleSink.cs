using System.Globalization;
using System.Text;

namespace RawSalt.Logging;

public class ConsoleSink : ILoggerSink
{
	protected const string Reset = "\e[0m", Gray = "\e[90m", Cyan = "\e[36m", Green = "\e[32m", Yellow = "\e[33m", Red = "\e[31m", White = "\e[97m";
	protected readonly TextWriter _output;

	static ConsoleSink()
	{
		WindowsConsole.EnableVirtualTerminalProcessing();
	}

	public ConsoleSink()
	{
		_output = new StreamWriter(Console.OpenStandardOutput(), new UTF8Encoding(false), 4096)
		{
			AutoFlush = false
		};
	}

	private StringBuilder _messageBuilder = new();
	public void Write(LogMessage message)
	{
		var sb = _messageBuilder.Clear();
		var (tag, color) = message.Severity switch
		{
			LogSeverity.Debug => ("DBG", Gray),
			LogSeverity.Info => ("INF", White),
			LogSeverity.Warning => ("WRN", Yellow),
			_ => ("ERR", Red),
		};

		sb.Append(Gray).Append('[').Append(message.Time.ToString("HH:mm:ss")).Append(' ')
			.Append(color).Append(tag).Append(Gray).Append("] ").Append(Reset);

		RenderTemplate(sb, message.Message, message.Args, color);
		sb.Append(Reset).AppendLine();
		if (message.Exception != null) sb.Append(message.Exception).AppendLine();
		_output.Write(sb);
	}

	private static void RenderTemplate(StringBuilder sb, string t, object?[] args, string baseColor)
	{
		sb.Append(baseColor);
		int arg = 0;
		for (int i = 0; i < t.Length; i++)
		{
			if (t[i] == '{' && i + 1 < t.Length && t[i + 1] == '{')
			{
				sb.Append('{');
				i++;
				continue;
			}

			if (t[i] == '{')
			{
				int end = t.IndexOf('}', i);

				if (end < 0)
				{
					sb.Append(t, i, t.Length - i);
					break;
				}

				object? v = arg < args.Length ? args[arg++] : null;
				(string text, string col) = v switch
				{
					null => ("null", Gray),
					string s => (s, Cyan),
					bool b => (b.ToString(), Green),
					IFormattable f when v.GetType().IsPrimitive => (f.ToString(null, CultureInfo.InvariantCulture), Green),
					_ => (v.ToString() ?? "", Cyan),
				};
				sb.Append(col).Append(text).Append(baseColor);
				i = end;
			}
			else sb.Append(t[i]);
		}
	}

	public void Flush()
	{
		_output.Flush();
	}
}