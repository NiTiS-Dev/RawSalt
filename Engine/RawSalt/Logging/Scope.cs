using System.Diagnostics.CodeAnalysis;

namespace RawSalt.Logging;

public sealed record Scope(string Name, Scope? Parent = null) : IDisposable
{
	public Scope BeginScope(string name)
	{
		return new Scope(name, this);
	}

	public void Debug([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, params object?[] args)
	{
		Write(LogSeverity.Debug, message, args);
	}

	public void Info([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, params object?[] args)
	{
		Write(LogSeverity.Info, message, args);
	}

	public void Warning([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, params object?[] args)
	{
		Write(LogSeverity.Warning, message, args);
	}

	public void Error([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, params object?[] args)
	{
		Write(LogSeverity.Error, message, args);
	}

	public void Error(Exception exception, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, params object?[] args)
	{
		Write(LogSeverity.Error, exception, message, args);
	}

	public void Write(LogSeverity level, Exception? exception,
		[StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, params object?[] args)
	{
		Log.Write(level, exception, this, message, args);
	}

	public void Write(LogSeverity level, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message,
		params object?[] args)
	{
		Log.Write(level, null, this, message, args);
	}

	void IDisposable.Dispose()
	{ }
}