using System;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows;

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
	}
}