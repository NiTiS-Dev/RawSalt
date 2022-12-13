using NiTiS.Math;
using System.Reflection;

namespace NiTiS.Windowing;

public record struct WindowOptions
	(
		string Title,
		string? ClassName,
		bool IsVisible,
		bool SwapAutomatically,
		int? PreferredDepthBufferBits,
		int? PreferredStencilBufferBits,
		Vector4D<int>? PreferredBitDepth,
		int? Samples,
		Vector2D<int> Size,
		GraphicsAPIOptions Graphics,
		bool VSync,
		WindowState State,
		WindowBorder Border,
		bool TransparentFramebuffer,
		bool AlwaysOnTop
	)
{
	public static WindowOptions Default
		=> new(
			"",
			Assembly.GetEntryAssembly()?.EntryPoint?.DeclaringType?.Name ?? "NiTiS.Window",
			true,
			true,
			null,
			null,
			null,
			null,
			new Vector2D<int>(1280, 720),
			GraphicsAPIOptions.DefaultOpenGL,
			false,
			WindowState.Default,
			WindowBorder.Resizable,
			false,
			false
			);
}