using System.Diagnostics.CodeAnalysis;

namespace RawSalt.Logging;

public sealed record LogMessage(DateTimeOffset Time, LogSeverity Severity,
	[StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Message, object?[] Args,
	Scope? Scope = null, Exception? Exception = null);