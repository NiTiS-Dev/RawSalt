using System.Diagnostics;

namespace RawSalt.Diagnostics;

public class RateStatistics
{
	private readonly Stopwatch _clock;
	private readonly double[] _updateTimes;

	private int _historyIndex;
	private int _historyCount;

	private long _lastUpdateTimestamp;
	private long _lastSecondTimestamp;

	private int _updatesThisSecond;
	private int _updatesPerSecond;

	private double _lastUpdateTime;
	private double _totalUpdateTime;
	private long _totalUpdates;

	private readonly Lock _lock = new();

	public RateStatistics(int historySize = 1248)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(historySize);

		_clock = new Stopwatch();

		_updateTimes = new double[historySize];
		_lastUpdateTimestamp = Stopwatch.GetTimestamp();
		_lastSecondTimestamp = _lastUpdateTimestamp;
	}

	/// <summary>
	/// Start the update rate measurement.
	/// </summary>
	public void Start()
	{
		_clock.Restart();
	}

	/// <summary>
	/// Call once per update.
	/// </summary>
	public void Update()
	{
		long timestamp = Stopwatch.GetTimestamp();
		double frameTime = (double)(timestamp - _lastUpdateTimestamp) / Stopwatch.Frequency;

		lock (_lock)
		{
			_lastUpdateTimestamp = timestamp;

			_lastUpdateTime = frameTime;
			_totalUpdateTime += frameTime;
			_totalUpdates++;

			_updateTimes[_historyIndex] = frameTime * 1000.0;

			_historyIndex = (_historyIndex + 1) % _updateTimes.Length;
			_historyCount = Math.Min(_historyCount + 1, _updateTimes.Length);

			_updatesThisSecond++;

			double elapsed = (double)(timestamp - _lastSecondTimestamp) / Stopwatch.Frequency;

			if (elapsed >= 1.0)
			{
				_updatesPerSecond = (int)Math.Round(_updatesThisSecond / elapsed);

				_updatesThisSecond = 0;
				_lastSecondTimestamp = timestamp;
			}
		}
	}

	/// <summary>
	/// Updates per approximately one-second.
	/// </summary>
	public int UpdatesPerSecond
	{
		get
		{
			lock (_lock)
			{
				return _updatesPerSecond;
			}
		}
	}

	/// <summary>
	/// Total number of updates.
	/// </summary>
	public long TotalUpdates
	{
		get
		{
			lock (_lock)
			{
				return _totalUpdates;
			}
		}
	}

	/// <summary>
	/// 1% low update rate, based on the slowest 1% of recent frames.
	/// </summary>
	public double OnePercentLow
	{
		get
		{
			lock (_lock)
			{
				if (_historyCount == 0)
				{
					return 0;
				}

				double[] sorted = GetSortedUpdateTimes();

				int index = (int)Math.Floor((sorted.Length - 1) * 0.99);
				return 1000.0 / sorted[index];
			}
		}
	}

	/// <summary>
	/// 0.1% low update rate, based on the slowest 0.1% of recent frames.
	/// </summary>
	public double PointOnePercentLow
	{
		get
		{
			lock (_lock)
			{
				if (_historyCount == 0)
				{
					return 0;
				}

				double[] sorted = GetSortedUpdateTimes();

				int index = (int)Math.Floor((sorted.Length - 1) * 0.999);
				return 1000.0 / sorted[index];
			}
		}
	}

	/// <summary>
	/// Duration of the last update in milliseconds.
	/// </summary>
	public double LastUpdateTime
	{
		get
		{
			lock (_lock)
			{
				return _lastUpdateTime * 1000.0;
			}
		}
	}

	/// <summary>
	/// Average update interval in milliseconds.
	/// </summary>
	public double AverageUpdateTime
	{
		get
		{
			lock (_lock)
			{
				return _totalUpdates == 0 ? 0 : _totalUpdateTime * 1000.0 / _totalUpdates;
			}
		}
	}

	public double MaximumUpdateTime
	{
		get
		{
			lock (_lock)
			{
				if (_historyCount == 0)
				{
					return 0;
				}

				double maximum = 0;

				for (int i = 0; i < _historyCount; i++)
				{
					maximum = Math.Max(maximum, _updateTimes[i]);
				}

				return maximum;
			}
		}
	}

	private double[] GetSortedUpdateTimes()
	{
		double[] result = new double[_historyCount];

		Array.Copy(_updateTimes, result, _historyCount);
		Array.Sort(result);

		return result;
	}
}