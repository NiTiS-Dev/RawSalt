using System;

namespace NiTiS.Native;

[AttributeUsage(AttributeTargets.Class)]
public class LibraryContainerAttribute : Attribute
{
	public Type Container { get; init; }
}