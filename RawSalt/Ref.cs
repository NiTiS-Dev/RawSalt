namespace RawSalt;

public sealed class Ref<STRUCT> where STRUCT : struct
{
	public Ref(STRUCT value)
	{
		Value = value;
	}
	public STRUCT Value;
}
public sealed class ReadonlyRef<STRUCT> where STRUCT : struct
{
	public ReadonlyRef(STRUCT value)
	{
		Value = value;
	}
	public readonly STRUCT Value;
}