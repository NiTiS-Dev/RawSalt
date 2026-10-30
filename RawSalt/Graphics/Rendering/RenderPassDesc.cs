using RawSalt.Graphics.Drawing;

namespace RawSalt.Graphics.Rendering;

public readonly record struct RenderPassDesc(RenderTarget Target, Color4? Clear = null);