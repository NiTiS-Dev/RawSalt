using System.Numerics;
using NiTiS.Mathematics;
using RawSalt;
using RawSalt.Graphics.Rendering;
using RawSalt.Graphics.Vulkan;
using RawSalt.Logging;
using RawSalt.Platforms.Win32;
using RawSalt.Scenes;

namespace NekoRotation;

internal sealed class App : Application
{
	private Camera _camera = new(CameraProjection.Perspective)
	{
		Position = new Vector3(0, 0, 2f)
	};

    public static void Main()
    {
	    Log.AddSink(new ConsoleSink());

        App app = new();

        var config = LaunchConfiguration.CreateBuilder()
	        .AddWin32Platform()
	        .AddVulkanBackend()
	        .Build();

        bool success = app.Launch(config);

        Environment.Exit(success ? 0 : -1);
    }

    private float _pos;
    protected override void Update(ulong updateId, float deltaTime)
    {
	    _pos += deltaTime;

	    float x = float.Cos(_pos * float.Pi);
	    float z = float.Sin(_pos * float.Pi);

	    _camera.Position = new Vector3(2f * x, x, 2f * z);
	    _camera.LookAt(Vector3.Zero);
    }

    protected override void Draw(ref CameraData camera, Vector2<int> viewport, ulong updateId, float deltaTime)
    {
	    _camera.SetViewport(viewport.X, viewport.Y);
	    camera.Projection = _camera.Projection;
	    camera.View = _camera.View;
    }
}
