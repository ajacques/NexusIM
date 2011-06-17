using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace NexusIM.Misc
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