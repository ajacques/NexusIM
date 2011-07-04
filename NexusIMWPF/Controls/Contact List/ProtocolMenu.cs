using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using InstantMessage;
using NexusIM.Managers;

namespace NexusIM.Controls
{
	abstract class ProtocolMenu<T> : Button where T : IMProtocol
	{
		public ProtocolMenu()
		{
			this.ContextMenu = new CustomContextMenu(this);
			this.Width = 16;
			this.Height = 16;
			ToolTip tip = new ToolTip();
			tip.Content = new TextBlock() { Text = this.ProtocolName };

			this.ToolTip = tip;

			mImage = new Image();
			this.Content = mImage;
		}
		
		private class CustomContextMenu : ContextMenu
		{
			public CustomContextMenu(ProtocolMenu<T> button)
			{
				mButton = button;
			}

			protected override void OnOpened(RoutedEventArgs e)
			{
				base.OnOpened(e);

				this.Items.Clear();

				IEnumerable<IMProtocolWrapper> protocols = AccountManager.Accounts.Where(w => w.Protocol is T);
				int count = protocols.Count();

				if (count >= 2)
				{
					MenuItem header = new MenuItem();
					header.Header = mButton.ProtocolName;
					header.FontWeight = FontWeight.FromOpenTypeWeight(500);
					header.IsEnabled = false;
					this.Items.Add(header);
					this.Items.Add(new Separator());

					foreach (IMProtocolWrapper wrapper in protocols)
					{
						if (wrapper == null)
							continue;

						MenuItem main = new MenuItem();
						this.Items.Add(main);
						main.Header = wrapper.Protocol.ToString();
						mButton.GenerateItemSet(wrapper, main.Items);
					}
				} else if (count == 1)
					mButton.GenerateItemSet(protocols.FirstOrDefault(), this.Items);
			}

			private ProtocolMenu<T> mButton;
		}

		protected override void OnClick()
		{
			base.OnClick();

			this.ContextMenu.IsOpen = true;
		}
		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			mImage.Source = this.GetImage();
		}

		protected abstract void GenerateItemSet(IMProtocolWrapper wrapper, ItemCollection coll);
		protected abstract ImageSource GetImage();
		protected abstract string ProtocolName
		{
			get;
		}

		// Variables
		private Image mImage;
	}
}