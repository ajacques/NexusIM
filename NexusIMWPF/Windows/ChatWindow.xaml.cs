using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using NexusIM.Controls;

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

		private void ChatAreas_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			
		}

		public IEnumerable<ITabbedArea> TabAreas
		{
			get	{
				lock (mTabAreaSyncObject)
					return mTabAreas;
			}
		}

		protected override void OnClosed(System.EventArgs e)
		{
			base.OnClosed(e);
		}

		private List<ITabbedArea> mTabAreas;
		private object mTabAreaSyncObject;
	}
}