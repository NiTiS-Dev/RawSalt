using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RawSalt;

#pragma warning disable CS8500
public static unsafe class SpanMarshal
{
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Span<T> MakeWritable<T>(ReadOnlySpan<T> span)
	{
		return new Span<T>(Unsafe.AsPointer(ref MemoryMarshal.GetReference(span)), span.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static T* GetPointer<T>(ReadOnlySpan<T> span)
	{
		return (T*)Unsafe.AsPointer<T>(ref MemoryMarshal.GetReference<T>(span));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static T* GetPointer<T>(Span<T> span)
	{
		return (T*)Unsafe.AsPointer<T>(ref MemoryMarshal.GetReference<T>(span));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Span<Target> FastCast<Source, Target>(Span<Source> span)
	{
		if (sizeof(Source) != sizeof(Target))
			throw new Exception("Generic arguments have different sizes.");

		return new Span<Target>(Unsafe.AsPointer(ref MemoryMarshal.GetReference(span)), span.Length);
	}
}
#pragma warning restore