using Microsoft.CodeAnalysis;

namespace RawSalt.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public sealed class VertexGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
	}
}