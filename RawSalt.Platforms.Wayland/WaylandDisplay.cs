using NiTiS.Mathematics;
using NWayland.Protocols.Wayland;
using RawSalt.Platforms;

namespace RawSalt.Windowing.Wayland;

public sealed class WaylandDisplay : Display
{
	private readonly WlOutput.Listener.Relay _listener;
	private int _x;
	private int _y;
	private string? _name;
	private string? _make;
	private string? _model;

	internal uint RegistryName { get; }
	internal WlOutput? Output { get; set; }
	internal int Scale { get; private set; } = 1;
	internal int ModeWidth { get; private set; }
	internal int ModeHeight { get; private set; }

	internal WaylandDisplay(uint registryName)
	{
		RegistryName = registryName;
		_listener = new WlOutput.Listener.Relay
		{
			OnGeometry = (_, x, y, _, _, _, make, model, _) =>
			{
				_x = x;
				_y = y;
				_make = make;
				_model = model;
			},
			OnMode = (_, flags, width, height, _) =>
			{
				if (flags.HasFlag(WlOutput.ModeEnum.Current))
				{
					ModeWidth = width;
					ModeHeight = height;
				}
			},
			OnScale = (_, factor) => Scale = Math.Max(1, factor),
			OnName = (_, name) => _name = name,
			OnDescription = (_, description) => _name ??= description,
		};
	}

	internal WlOutput.Listener Listener => _listener;

	public override string Name
	{
		get
		{
			if (_name != null)
			{
				return _name;
			}

			string combined = $"{_make} {_model}".Trim();
			return combined.Length > 0 ? combined : $"Display {RegistryName}";
		}
	}

	public override Region2<int> Region
	{
		get
		{
			// wl_output geometry is in compositor coordinates (logical),
			// while mode size is in physical pixels.
			int width = ModeWidth > 0 ? Math.Max(1, ModeWidth / Scale) : 0;
			int height = ModeHeight > 0 ? Math.Max(1, ModeHeight / Scale) : 0;
			return new Region2<int>(_x, _y, width, height);
		}
	}
}
