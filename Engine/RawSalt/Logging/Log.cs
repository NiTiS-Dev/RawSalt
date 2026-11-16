using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;

namespace RawSalt.Logging;

public static class Log
{
	// TODO: Add Logger: replacement of scopes
	private static readonly PendingSmallList<ILoggerSink> _sinks = new();
	private static readonly Thread _backgroundThread;
	private static readonly Channel<LogMessage> _messageChannel;

	/// <summary>
	/// Adds a <paramref name="sink"/> to the global logger sinks.
	/// </summary>
	/// <remarks>
	/// If <paramref name="sink"/> implements <see cref="IDisposable"/> interface,
	/// it will be disposed at application exit by application thread manually.
	/// </remarks>
	/// <param name="sink">Logger sink.</param>
	public static void AddSink(ILoggerSink sink)
	{
		_sinks.Add(sink);
	}

	/// <summary>
	/// Remove sink from global logger sinks.
	/// </summary>
	/// <remarks>
	/// Sink is not disposed on removal.
	/// </remarks>
	public static bool RemoveSink(ILoggerSink sink)
	{
		return _sinks.Remove(sink);
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

		AppDomain.CurrentDomain.ProcessExit += Stop;
	}

	private static void Run()
	{
		ChannelReader<LogMessage> reader = _messageChannel.Reader;

		while (reader.WaitToReadAsync().AsTask().GetAwaiter().GetResult())
		{
			while (reader.TryRead(out LogMessage? message))
			{
				foreach (var t in _sinks)
				{
					t.Write(message);
				}
			}

			foreach (var t in _sinks)
			{
				t.Flush();
			}
		}
	}

	private static void Stop(object? sender, EventArgs args)
	{
		try
		{
			_messageChannel.Writer.TryComplete();

			// Wait maximum 5 seconds
			_backgroundThread.Join(TimeSpan.FromSeconds(5));
		}
		finally
		{
			var sinksToDispose = _sinks.ClearAndGet();

			foreach (ILoggerSink sink in sinksToDispose)
			{
				if (sink is IDisposable disposable)
				{
					try
					{
						disposable.Dispose();
					}
					catch { } // Probably user was already disposed sink
				}
			}
		}
	}
}