using System.Windows.Threading;

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
	}
}