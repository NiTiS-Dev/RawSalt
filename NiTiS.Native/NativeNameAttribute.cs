using System;

namespace NiTiS.Native;

[AttributeUsage(AttributeTargets.Field)]
public sealed class NativeNameAttribute : Attribute
{
	public string EntryPoint { get; init; }
}