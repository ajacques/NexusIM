using System;
using InstantMessage;
using InstantMessage.Events;

namespace NexusIM
{
	static class ErrorManager
	{
		public static void Setup()
		{
			IMProtocol.onError += new EventHandler<IMErrorEventArgs>(IMProtocol_onError);
		}

		private static void IMProtocol_onError(object sender, IMErrorEventArgs e)
		{
			if (e.Reason == IMErrorEventArgs.ErrorReason.INVALID_USERNAME)
			{
				//frmMain.Instance.ShowErrorMethod("Incorrect Username");
			}
		}
	}
}