using System;

namespace NiTiS.Native;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class NativeAPIAttribute : Attribute
{
	public string MethodPrefix { get; init; }
	public Type ContainerType { get; init; }
	public APIType APIType { get; init; }
}
