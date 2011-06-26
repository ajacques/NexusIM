using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using NexusIM.Controls;
using NexusIM.Misc;

namespace NexusIM.Windows
{
	/// <summary>
	/// A container window that hosts the actual areas (ContactChatArea, etc).
	/// Can use tabs, or the tabs can be hidden.
	/// </summary>
	public partial class ChatWindow : Window
	{
		public ChatWindow()
		{
			this.InitializeComponent();

			mTabAreas = new List<ITabbedArea>();
			mTabAreaSyncObject = new object();
		}

		public void AttachAreaAndShow(TabItem tabPage)
		{
			if (tabPage is ContactChatAreaHost)
			{
				ContactChatAreaHost host = (ContactChatAreaHost)tabPage;
				
				host.HostWindow = this;
			}
			Dispatcher.BeginInvoke(new GenericEvent(() =>
			{
				ChatAreas.Items.Add(tabPage);
				ChatAreas.SelectedItem = tabPage;
			}));
		}
		public void HandleTabClose(TabItem tabPage)
		{
			ChatAreas.Items.Remove(tabPage);

			if (ChatAreas.Items.Count == 0)
				this.Close();
		}
		public void IncrementUnread(int step = 1)
		{
			Dispatcher.InvokeIfRequired(() =>
			{
				if (this.IsFocused)
					return;

				mUnread += step;

				UpdateWindowTitle();
			});
		}

		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);

			mUnread = 0;
			UpdateWindowTitle();
		}
		protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);

			//base.DragMove();
		}
		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			Aero.ExtendGlass(this, new Thickness(1, 33, 1, 0));
		}

		private void ChatAreas_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.Title = ChatAreas.SelectedItem.ToString();

			UpdateWindowTitle();
		}

		// Private Functions
		private void UpdateWindowTitle()
		{
			Dispatcher.InvokeIfRequired(() => {
				string msg;

				if (mUnread == 0)
					msg = ChatAreas.SelectedItem.ToString();
				else
					msg = String.Format("[{0}] {1}", mUnread, ChatAreas.SelectedItem.ToString());

				Title = msg;
			});
		}
		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == Aero.WM_DWMCOMPOSITIONCHANGED)
			{
				Aero.ExtendGlass(this, new Thickness(1, 33, 1, 0));
				handled = true;
			}
			return IntPtr.Zero;
		}

		public IEnumerable<ITabbedArea> TabAreas
		{
			get	{
				lock (mTabAreaSyncObject)
					return mTabAreas;
			}
		}

		private int mUnread;
		private List<ITabbedArea> mTabAreas;
		private object mTabAreaSyncObject;
	}
}