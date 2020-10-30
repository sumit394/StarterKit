using System;
using System.Diagnostics;
using Serilog;

namespace StarterKit.Logging
{
	public class MetricTimer : IDisposable
	{
		public Stopwatch StopWatch { get; protected set; }
		public string MetricName { get; }
		private readonly ILogger Logger;
		private bool _success;

		public MetricTimer(ILogger logger, string metricName)
		{
			Logger = logger;
			MetricName = metricName;
			StopWatch = new Stopwatch();
			StopWatch.Start();
		}

		public void Success()
		{
			_success = true;
		}

		public void Dispose()
		{
			StopWatch.Stop();
			const string mt = "metric: '{MetricName}' took '{ElapsedMilliseconds}'ms";
			if (_success)
				Logger.Information(mt, MetricName, StopWatch.ElapsedMilliseconds);
			else
				Logger.Error(mt, MetricName, StopWatch.ElapsedMilliseconds);
		}
	}
}