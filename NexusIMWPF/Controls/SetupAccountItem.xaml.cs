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
			Storyboard AnimFadeIn = FindResource("EditFadeIn") as Storyboard;
			if (mAnimReversed)
			{
				Reverse(AnimFadeIn);
				mAnimReversed = false;
			}

			AnimFadeIn.Begin();
		}
		public void Deselect()
		{
			Storyboard AnimFadeIn = FindResource("EditFadeIn") as Storyboard;
			if (mAnimReversed)
			{
				Reverse(AnimFadeIn);
				mAnimReversed = true;
			}

			AnimFadeIn.Begin();
		}

		public void Reverse(Storyboard storyboard)
		{
			foreach (var timeline in storyboard.Children)
			{
				if (timeline is ObjectAnimationUsingKeyFrames)
				{
					ObjectAnimationUsingKeyFrames objAnim = timeline as ObjectAnimationUsingKeyFrames;
					ObjectKeyFrame frame0 = objAnim.KeyFrames[0];
					ObjectKeyFrame frame1 = objAnim.KeyFrames[1];

					KeyTime temp = frame0.KeyTime;
					frame0.KeyTime = frame1.KeyTime;
					frame1.KeyTime = temp;
				} else if (timeline is DoubleAnimation) {
					DoubleAnimation anim = timeline as DoubleAnimation;
					double? temp = anim.From;
					anim.From = anim.To;
					anim.To = temp;
				}
			}
		}

		private bool mAnimReversed;
	}
}