using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;

namespace RawSalt.Logging;

public static class Log
{
	// TODO: Add Logger: replacement of scopes
	private static readonly Lock _sinkLock = new();
	private static readonly List<ILoggerSink> _sinks = [];
	private static readonly Thread _backgroundThread;
	private static readonly Channel<LogMessage> _messageChannel;

	public static void AddSink(ILoggerSink sink)
	{
		using (_sinkLock.EnterScope())
		{
			_sinks.Add(sink);
		}
	}

	public static bool RemoveSink(ILoggerSink sink)
	{
		using (_sinkLock.EnterScope())
		{
			return _sinks.Remove(sink);
		}
	}

	public static Scope BeginScope(string name)
	{
		return new Scope(name);
	}

	public static LogSeverity MinimumSeverity { get; set; } = LogSeverity.Debug;

	public static void Debug([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, params object?[] args)
	{
		Write(LogSeverity.Debug, message, args);
	}

	public static void Info([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, params object?[] args)
	{
		Write(LogSeverity.Info, message, args);
	}

	public static void Warning([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, params object?[] args)
	{
		Write(LogSeverity.Warning, message, args);
	}

	public static void Error([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, params object?[] args)
	{
		Write(LogSeverity.Error, message, args);
	}

	public static void Error(Exception exception, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, params object?[] args)
	{
		Write(LogSeverity.Error, exception, null, message, args);
	}

	public static void Write(LogSeverity severity, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, params object?[] args)
	{
		Write(severity, null, null, message, args);
	}

	public static void Write(LogSeverity level, Exception? exception, Scope? scope, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, params object?[] args)
	{
		if (level < MinimumSeverity) return;

		_messageChannel.Writer.TryWrite(new LogMessage(
			DateTimeOffset.Now, level,
			message, args,
			scope, exception)
		);
	}

	static Log()
	{
		_messageChannel = Channel.CreateBounded<LogMessage>(new BoundedChannelOptions(1024)
		{
			SingleReader = true,
			FullMode = BoundedChannelFullMode.Wait
		});

		_backgroundThread = new Thread(Run)
		{
			Name = "RawSalt/LoggerThread",
			Priority =  ThreadPriority.BelowNormal,
			IsBackground = true,
		};
		_backgroundThread.Start();
	}

	private static void Run()
	{
		ChannelReader<LogMessage> reader = _messageChannel.Reader;
		while (reader.WaitToReadAsync().AsTask().GetAwaiter().GetResult())
		{
			using (_sinkLock.EnterScope())
			{
				while (reader.TryRead(out LogMessage? message))
				{
					foreach (ILoggerSink sink in _sinks)
					{
						sink.Write(message);
					}
				}

				foreach (ILoggerSink s in _sinks)
				{
					s.Flush();
				}
			}
		}
	}
}