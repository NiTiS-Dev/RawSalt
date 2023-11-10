using System;

namespace NiTiS.Annotations;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class ExportAttribute : Attribute
{
	public ExportAttribute()
	{
		
	}
}
