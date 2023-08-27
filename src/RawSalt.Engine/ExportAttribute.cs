namespace RawSalt.Engine;

/// <summary>
/// Indicate field or property as visible on editor
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ExportAttribute : Attribute
{

}
