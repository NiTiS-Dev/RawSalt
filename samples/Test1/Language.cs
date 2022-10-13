using RawSalt.Resources;
using System;

namespace Test1;

public class Language : IResourceType, IIdentity
{
	public readonly bool ReverseWriting;
	public readonly string LanguageCode;
	public readonly string EnglishName;
	public Language(string code, string englishName, bool rightToLeft)
	{
		ReverseWriting = rightToLeft;
		LanguageCode = code;
		EnglishName = englishName;
	}
	public static string ResourceType => "language";
	public string Identity => LanguageCode;
	public void DisposeResource()
	{
        Console.WriteLine($"Language: {EnglishName} is unloaded");
    }
}
