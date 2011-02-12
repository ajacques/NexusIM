using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace NexusIM.Controls
{
	/// <summary>
	/// Interaction logic for ContactListItem.xaml
	/// </summary>
	partial class ContactListItem : UserControl
	{
		public ContactListItem()
		{
			this.InitializeComponent();
		}

		public void Select()
		{
			Storyboard AnimFade = FindResource("SelectAnimation") as Storyboard;

			AnimFade.Begin();
			Selected = true;
		}
		public void Deselect()
		{
			Storyboard AnimFade = FindResource("DeselectAnimation") as Storyboard;

			AnimFade.Begin();
			Selected = false;
		}

		public bool Selected
		{
			get;
			private set;
		}

		private void LayoutRoot_DragOver(object sender, DragEventArgs e)
		{
			e.Effects = DragDropEffects.Link;
		}
		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
		{
			return new PointHitTestResult(this, hitTestParameters.HitPoint);
		}
	}
}