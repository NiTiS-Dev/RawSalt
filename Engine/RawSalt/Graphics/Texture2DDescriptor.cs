using RawSalt.Graphics.Drawing;

namespace RawSalt.Graphics;

public readonly record struct Texture2DDescriptor(Vec2i Size, PixelFormat Format, Sampling Sampling = Sampling.Repeat);