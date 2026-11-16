namespace RawSalt.Graphics.Drawing;

public partial struct Color4
{
	public Color4 Invert()
	{
		return new(1f - Red, 1f - Green, 1f - Blue, Alpha);
	}

	/// <summary>
	/// Fast mathematical lerp between two colors.
	/// </summary>
	public static Color4 Lerp(Color4 from, Color4 to, float amount)
	{
		amount = Math.Clamp(amount, 0f, 1f);

		float r = float.Lerp(from.Red, to.Red, amount);
		float g = float.Lerp(from.Green, to.Green, amount);
		float b = float.Lerp(from.Blue, to.Blue, amount);
		float a = float.Lerp(from.Alpha, to.Alpha, amount);
		return new Color4(r, g, b, a);
	}

	public Color4 ShiftHue(float degrees)
	{
		ToHsv(out float hue, out float saturation, out float value);

		hue = (hue + degrees % 360f + 360f) % 360f;

		return FromHsv(hue, saturation, value, Alpha);
	}

	public Color4 WithHue(float degrees)
	{
		ToHsv(out _, out float saturation, out float value);

		return FromHsv(degrees, saturation, value, Alpha);
	}

	public Color4 WithSaturation(float saturation)
	{
		ToHsv(out float hue, out _, out float value);

		return FromHsv(hue, Math.Clamp(saturation, 0f, 1f), value, Alpha);
	}

	public Color4 WithValue(float value)
	{
		ToHsv(out float hue, out float saturation, out _);

		return FromHsv(hue, saturation, Math.Clamp(value, 0f, 1f), Alpha);
	}

	#region  Helpers
	public (float Hue, float Saturation, float Value) ToHsv()
	{
		ToHsv(out float hue, out float saturation, out float value);
		return (hue, saturation, value);
	}

	public void ToHsv(out float hue, out float saturation, out float value)
	{
		float max = Math.Max(Red, Math.Max(Green, Blue));
		float min = Math.Min(Red, Math.Min(Green, Blue));
		float delta = max - min;

		value = max;
		saturation = max == 0f ? 0f : delta / max;

		if (delta == 0f)
		{
			hue = 0f;
		}
		else if (max == Red)
		{
			hue = 60f * (((Green - Blue) / delta) % 6f);
		}
		else if (max == Green)
		{
			hue = 60f * ((Blue - Red) / delta + 2f);
		}
		else
		{
			hue = 60f * ((Red - Green) / delta + 4f);
		}

		if (hue < 0f)
		{
			hue += 360f;
		}
	}

	public static Color4 FromHsv(float hue, float saturation, float value, float alpha = 1f)
	{
		hue = (hue % 360f + 360f) % 360f;
		saturation = Math.Clamp(saturation, 0f, 1f);

		float chroma = value * saturation;
		float x = chroma * (1f - Math.Abs((hue / 60f) % 2f - 1f));
		float m = value - chroma;

		(float r, float g, float b) = hue switch
		{
			< 60f  => (chroma, x, 0f),
			< 120f => (x, chroma, 0f),
			< 180f => (0f, chroma, x),
			< 240f => (0f, x, chroma),
			< 300f => (x, 0f, chroma),
			_      => (chroma, 0f, x)
		};

		return new(r + m, g + m, b + m, alpha);
	}
	#endregion
}