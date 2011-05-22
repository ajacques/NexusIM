using System;
using System.Diagnostics;
using System.Text;
using System.Timers;
using InstantMessage;
using InstantMessage.Events;
using NexusIM.Managers;

namespace NexusIM.Misc
{
	internal class ProtocolErrorBackoff : IDisposable
	{
		public ProtocolErrorBackoff(IMProtocolWrapper protocol)
		{
			mAttempts = 1;
			mInterval = TimeSpan.FromSeconds(3);
			protocol.Protocol.LoginCompleted += new EventHandler(Protocol_LoginSuccess);
			protocol.Protocol.ErrorOccurred += new EventHandler<IMErrorEventArgs>(Protocol_ErrorOccurred);
			protocol.InErrorState = true;

			mProtocol = protocol;
			mTimer = new Timer();
			mTimer.Elapsed += new ElapsedEventHandler(Timer_Tick);
			mTimer.AutoReset = false;
			mTimer.Interval = mInterval.TotalMilliseconds;
			mTimer.Start();

			Trace.WriteLine("Protocol encountered an error. Re-attempting login in " + mInterval.TotalSeconds + " seconds");
		}

		public void Dispose()
		{
			if (mTimer != null)
				mTimer.Dispose();

			mTimer = null;
			GC.SuppressFinalize(this);
		}
		private void Cleanup()
		{
			mTimer.Stop();
			mTimer.Elapsed -= new ElapsedEventHandler(Timer_Tick);
			mProtocol.InErrorState = false;

			Dispose();
		}
		
		// Event Handlers
		private void Protocol_LoginSuccess(object sender, EventArgs e)
		{
			Cleanup();
		}
		private void Protocol_ErrorOccurred(object sender, IMErrorEventArgs e)
		{
			if (mAttempts >= 5)
			{
				// Quit after 5 attempts

				StringBuilder message = new StringBuilder();
				message.AppendFormat("The account {0} ({1}) is currently experiencing problems.", mProtocol.Protocol.Username, mProtocol.Protocol.Protocol);
				if (!String.IsNullOrEmpty(e.Message))
				{
					message.AppendLine();
					message.Append("Message: ");
					message.Append(e.Message);
				}

				message.AppendLine();
				message.Append("Click for more information.");

				WindowSystem.SysTrayIcon.ShowBalloonTip("Account Error", message.ToString(), Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Warning);

				return;
			}

			mTimer.Start();
			Trace.WriteLine("Protocol encountered an error. Re-attempting login in " + mInterval.TotalSeconds + " seconds");
		}
		private void Timer_Tick(object sender, ElapsedEventArgs e)
		{
			mInterval = mInterval.Add(mInterval); // Double it
			mAttempts++;
			mTimer.Interval = mInterval.TotalMilliseconds;
			mProtocol.Protocol.BeginLogin();
		}

		private Timer mTimer;
		private TimeSpan mInterval;
		private ushort mAttempts;
		private IMProtocolWrapper mProtocol;
	}
}