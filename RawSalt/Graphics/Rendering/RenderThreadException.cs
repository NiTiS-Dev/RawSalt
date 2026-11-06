namespace RawSalt.Graphics.Rendering;

internal sealed class RenderThreadException(Exception cause) : Exception("Render thread has crashed.", cause);