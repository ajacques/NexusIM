using System.Windows;
using System.Windows.Controls;

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
		}

		public void AttachAreaAndShow(TabItem tabPage)
		{
			Dispatcher.BeginInvoke(new GenericEvent(() => {
				ChatAreas.Items.Add(tabPage);
				ChatAreas.SelectedItem = tabPage;
			}));
		}
	}
}