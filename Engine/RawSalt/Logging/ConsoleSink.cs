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

		for (int i = 0; i < t.Length;)
		{
			if (t[i] == '{' && i + 1 < t.Length && t[i + 1] == '{')
			{
				sb.Append('{');
				i += 2;
				continue;
			}

			if (t[i] == '}' && i + 1 < t.Length && t[i + 1] == '}')
			{
				sb.Append('}');
				i += 2;
				continue;
			}

			if (t[i] == '{')
			{
				int end = t.IndexOf('}', i + 1);

				if (end > i)
				{
					string placeholder = t.Substring(i + 1, end - i - 1);
					int colon = placeholder.IndexOf(':');
					string indexText = colon < 0 ? placeholder : placeholder[..colon];
					string? format = colon < 0 ? null : placeholder[(colon + 1)..];

					if (int.TryParse(indexText, NumberStyles.None, CultureInfo.InvariantCulture, out int index)
						&& (uint)index < (uint)args.Length)
					{
						object? v = args[index];
						(string text, string col) = v switch
						{
							null => ("null", Gray),
							string s => (s, Cyan),
							bool b => (b.ToString(), Green),
							IFormattable f when v.GetType().IsPrimitive => (f.ToString(format, CultureInfo.InvariantCulture), Green),
							IFormattable f when format != null => (f.ToString(format, CultureInfo.InvariantCulture), Cyan),
							_ => (v.ToString() ?? "", Cyan),
						};
						sb.Append(col).Append(text).Append(baseColor);
						i = end + 1;
						continue;
					}
				}

				sb.Append('{');
				i++;
				continue;
			}

			sb.Append(t[i]);
			i++;
		}
	}

	public void Flush()
	{
		_output.Flush();
	}
}