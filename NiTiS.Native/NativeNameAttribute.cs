using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiTiS.Native;

[AttributeUsage(AttributeTargets.Field)]
public sealed class NativeNameAttribute : Attribute
{
	public string EntryPoint { get; init; }
}