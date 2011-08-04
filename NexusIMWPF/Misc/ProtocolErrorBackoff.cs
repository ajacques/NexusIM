using System;
using System.Diagnostics;
using System.Text;
using System.Timers;
using InstantMessage;
using InstantMessage.Events;
using NexusIM.Managers;
using System.Windows;
using NexusIM.Controls;
using System.Net.Sockets;

namespace NexusIM.Misc
{
	internal class ProtocolErrorBackoff : IDisposable
	{
		public ProtocolErrorBackoff(IMProtocolWrapper protocol)
		{
			if (protocol == null)
				throw new ArgumentNullException("protocol");

			mAttempts = 1;
			mInterval = TimeSpan.FromSeconds(3);

			mProtocol = protocol;
			mTimer = new Timer();
			mTimer.Elapsed += new ElapsedEventHandler(Timer_Tick);
			mTimer.AutoReset = false;
			mTimer.Interval = mInterval.TotalMilliseconds;
			mTimer.Start();

			protocol.Protocol.LoginCompleted += new EventHandler(Protocol_LoginSuccess);
			protocol.Protocol.ErrorOccurred += new EventHandler<IMErrorEventArgs>(Protocol_ErrorOccurred);
			Trace.WriteLine("Protocol encountered an error. Re-attempting login in " + mInterval.TotalSeconds + " seconds");
		}

		public void Cancel()
		{
			Cleanup();
		}

		public void Dispose()
		{
			Cleanup();

			if (mTimer != null)
				mTimer.Dispose();

			mTimer = null;
			GC.SuppressFinalize(this);
		}
		private void Cleanup()
		{
			if (mTimer != null)
			{
				mTimer.Stop();
				mTimer.Elapsed -= new ElapsedEventHandler(Timer_Tick);
			}
			if (mProtocol != null)
				mProtocol.ErrorBackoff = null;
		}
		
		// Event Handlers
		private void Protocol_LoginSuccess(object sender, EventArgs e)
		{
			Cleanup();
		}
		private void Protocol_ErrorOccurred(object sender, IMErrorEventArgs e)
		{
			if (e.GetType() != typeof(SocketErrorEventArgs))
				return;

			if (mAttempts >= 5)
			{
				// Quit after 5 attempts
				Trace.WriteLine("Protocol failed reconnect test. Alerting user");

				WindowSystem.DispatcherInvoke(() =>
				{
					SocketErrorTrayTip tip = new SocketErrorTrayTip();
					//tip.PopulateControls(exception, protocol);
					WindowSystem.SysTrayIcon.ShowCustomBalloon(tip, System.Windows.Controls.Primitives.PopupAnimation.Slide, null);
				});

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

				WindowSystem.SysTrayIcon.TrayBalloonTipClicked += new RoutedEventHandler(SysTrayIcon_TrayBalloonTipClicked);
				WindowSystem.SysTrayIcon.ShowBalloonTip("Account Error", message.ToString(), Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Warning);

				return;
			}

			mTimer.Start();
			Trace.WriteLine("Protocol encountered an error. Re-attempting login in " + mInterval.TotalSeconds + " seconds");
		}
		private void Timer_Tick(object sender, ElapsedEventArgs e)
		{
			mTimer.Stop();

			Trace.WriteLine(String.Format("ErrorHandler: Re-attemping login now for: (Username: {0}, Type: {1})", mProtocol.Protocol, mProtocol.Protocol.Protocol));

			mInterval = mInterval.Add(mInterval); // Double it
			mAttempts++;
			mTimer.Interval = mInterval.TotalMilliseconds;
			mProtocol.Protocol.BeginLogin();
		}
		private void SysTrayIcon_TrayBalloonTipClicked(object sender, RoutedEventArgs e)
		{
			WindowSystem.SysTrayIcon.TrayBalloonTipClicked -= new RoutedEventHandler(SysTrayIcon_TrayBalloonTipClicked);

			MessageBox.Show("hi");
		}

		private Timer mTimer;
		private TimeSpan mInterval;
		private ushort mAttempts;
		private IMProtocolWrapper mProtocol;
	}
}