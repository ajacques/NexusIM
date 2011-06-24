using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using InstantMessage;

namespace NexusIM.Controls
{
	class IrcChanUserContextMenu : ContextMenu
	{
		public IrcChanUserContextMenu()
		{
			SetupMenu();
		}

		public void PopulateMenu(string username)
		{
			char mode = username[0];
			IsNothing();

			switch (mode)
			{
				case '@':
					IsOperator();
					break;
			}
		}
		private void IsOperator()
		{
			Operator.Header = "Take Operator";
		}
		private void IsNothing()
		{
			Operator.Header = "Give Operator";
		}
		private void SetupMenu()
		{
			Operator = new MenuItem();

			this.Items.Add(Operator);
		}

		public MenuItem Operator
		{
			get;
			private set;
		}
	}
}