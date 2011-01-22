using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace NexusIM.Controls
{
	/// <summary>
	/// Interaction logic for SetupAccountItem.xaml
	/// </summary>
	public partial class SetupAccountItem : UserControl
	{
		public SetupAccountItem()
		{
			this.InitializeComponent();
		}

		public void Select()
		{
			Storyboard AnimFade = FindResource("EditFadeIn") as Storyboard;

			AnimFade.Begin();
			Selected = true;
		}
		public void Deselect()
		{
			Storyboard AnimFade = FindResource("EditFadeOut") as Storyboard;

			AnimFade.Begin();
			Selected = false;
		}

		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
		{
			return new PointHitTestResult(this, hitTestParameters.HitPoint);
		}


		public bool Selected
		{
			get;
			private set;
		}

		private void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			// TODO: Add event handler implementation here.
		}
	}
}