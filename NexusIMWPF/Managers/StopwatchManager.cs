using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace NexusIM.Managers
{	
	static class StopwatchManager
	{
		[Conditional("DEBUG")]
		public static void Start(string name)
		{
			if (mStopwatches == null)
				mStopwatches = new Dictionary<string, Stopwatch>();

			Stopwatch stopwatch;
			if (mStopwatches.TryGetValue(name, out stopwatch))
			{
				if (stopwatch.IsRunning)
					stopwatch.Stop();
			} else {
				stopwatch = new Stopwatch();
				mStopwatches.Add(name, stopwatch);
			}

			stopwatch.Start();
		}

		[Conditional("DEBUG")]
		public static void TraceElapsed(string name, string format = "Stopwatch {0} trace point: {1}")
		{
			if (mStopwatches == null)
				mStopwatches = new Dictionary<string, Stopwatch>();

			Stopwatch stopwatch;
			if (!mStopwatches.TryGetValue(name, out stopwatch))
				throw new KeyNotFoundException();
			
			Debug.WriteLine(string.Format(format, name, stopwatch.Elapsed));
		}

		[Conditional("DEBUG")]
		public static void Stop(string name, string format = "Stopwatch {0} completed in: {1}")
		{
			if (mStopwatches == null)
				mStopwatches = new Dictionary<string, Stopwatch>();

			Stopwatch stopwatch;
			if (!mStopwatches.TryGetValue(name, out stopwatch))
				throw new KeyNotFoundException();

			stopwatch.Stop();
			Debug.WriteLine(string.Format(format, name, stopwatch.Elapsed));

			mStopwatches.Remove(name);
		}

		private static Dictionary<string, Stopwatch> mStopwatches;
	}
}