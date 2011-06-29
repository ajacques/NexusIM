using System.Windows.Threading;
using System;

namespace NexusIM
{
	internal static class MiscExtensions
	{
		public static void InvokeIfRequired(this Dispatcher dispatcher, GenericEvent target, bool useAsync = true)
		{
			if (!dispatcher.CheckAccess())
			{
				if (useAsync)
					dispatcher.BeginInvoke(target);
				else
					dispatcher.Invoke(target);
			} else
				target();
		}

		public static void InvokeIfRequired(this Dispatcher dispatcher, Delegate target, bool useAsync = true, params object[] args)
		{
			if (!dispatcher.CheckAccess())
			{
				if (useAsync)
					dispatcher.BeginInvoke(target, args);
				else
					dispatcher.Invoke(target, args);
			} else
				target.DynamicInvoke(args);
		}
	}
}