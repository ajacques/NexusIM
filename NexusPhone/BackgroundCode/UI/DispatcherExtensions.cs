using System;
using System.Net;
using System.Windows;

namespace NexusPhone
{
	static class DispatcherExtensions
	{
		public static void Dispatch(this UIElement source, Action func)
		{
			if (source.Dispatcher.CheckAccess())
				func();
			else
				source.Dispatcher.BeginInvoke(func);
		}
	}
}