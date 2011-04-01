using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using NexusIM.Controls;

namespace NexusIM.Windows
{
	/// <summary>
	/// Interaction logic for ChatWindow.xaml
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
			if (tabPage is ChatAreaHost)
			{
				ChatAreaHost host = (ChatAreaHost)tabPage;
				lock (mTabAreaSyncObject)
					mTabAreas.Add(host.HostedArea);
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

		public IEnumerable<ITabbedArea> TabAreas
		{
			get	{
				lock (mTabAreaSyncObject)
					return mTabAreas;
			}
		}

		private List<ITabbedArea> mTabAreas;
		private object mTabAreaSyncObject;
	}
}