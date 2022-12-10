using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NiTiS.Native;

[DebuggerDisplay($"{{{nameof(ToString)}(),nq}}")]
public unsafe readonly struct CString : IEnumerable<char>
{
	private const byte NullChar = 0;
	private readonly byte* pFirst;
	public CString(char* pChar)
	{
		this.pFirst = (byte*)pChar;
	}
	public CString(byte* pChar)
	{
		this.pFirst = pChar;
	}
	public IEnumerator<char> GetEnumerator()
		=> new Enumerable(this.pFirst);
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public static implicit operator char*(CString self)
		=> (char*)self.pFirst;
	public static implicit operator byte*(CString self)
		=> self.pFirst;
	public static explicit operator nint(CString self)
		=> (nint)(void*)self.pFirst;
	public static explicit operator nuint(CString self)
		=> (nuint)(void*)self.pFirst;
	public static explicit operator void*(CString self)
		=> self.pFirst;
	public static implicit operator CString(char* self)
		=> new(self);
	public static implicit operator CString(byte* self)
		=> new(self);

	public override readonly string ToString()
		=> ToString(Encoding.UTF8);
	[DebuggerStepThrough]
	public readonly string ToString(Encoding encoding)
	{
		if (pFirst == null || pFirst[0] == NullChar)
			return string.Empty;

		int size = 0;
		for (;true;)
		{
			byte c = pFirst[size];

			if (c == NullChar)
				break;

			size++;
		}

		return encoding.GetString(pFirst, size);
	}
	private struct Enumerable : IEnumerator<char>
	{
		private char* p;
		private nint index;
		public Enumerable(char* ptr)
		{
			this.p = ptr;
		}
		public Enumerable(byte* ptr)
		{
			this.p = (char*)ptr;
		}
		public char Current => p[index];
		object IEnumerator.Current => this.Current;
		public void Dispose()
		{

		}
		public bool MoveNext()
		{
			if (p != null && p[index++] == NullChar)
				return false;

			return true;
		}
		public void Reset()
		{
			index = 0;
		}
	}
}